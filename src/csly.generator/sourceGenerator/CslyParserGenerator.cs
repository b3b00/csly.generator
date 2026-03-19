using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace csly.generator.sourceGenerator;

public class ParserGeneratorTriplet
{
    private readonly ClassDeclarationSyntax _generatorClass;
    private readonly ClassDeclarationSyntax _parserClass;
    private readonly EnumDeclarationSyntax _lexerEnum;
    private readonly string _nameSpace;
    

    
    public ClassDeclarationSyntax GeneratorClass => _generatorClass;

    public ClassDeclarationSyntax ParserClass => _parserClass;

    public EnumDeclarationSyntax LexerEnum => _lexerEnum;
    
    public string NameSpace => _nameSpace;
    
    private string _outputType;
    public string OutputType   => _outputType;


    public ParserGeneratorTriplet(ClassDeclarationSyntax generatorClass, ClassDeclarationSyntax parserClass,
        EnumDeclarationSyntax lexerEnum, string outputType)
    {
        _generatorClass = generatorClass;
        _parserClass = parserClass;
        _lexerEnum = lexerEnum;
        _nameSpace = generatorClass.GetNameSpace();
        _outputType = outputType;
    }
}

[Generator]
public class CslyParserGenerator : IIncrementalGenerator
{

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        Dictionary<ClassDeclarationSyntax, (string lexerType, string parserType)> _lexerAndParserTypes = new();

        // Filter classes annotated with the [Report] attribute. Only filtered Syntax Nodes can trigger code generation.
        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(
                (s, _) => s is ClassDeclarationSyntax,
                (ctx, _) => GetClassDeclarationForSourceGen(ctx))
            .Where(t => t.parserGeneratorAttributeFound)
            .Select((t, _) =>
            {
                _lexerAndParserTypes[t.classDeclarationSyntax] = (t.lexerType, t.parserType);
                return t.classDeclarationSyntax;
            });

        var provider2 = context.SyntaxProvider.CreateSyntaxProvider((s, _) => s is ClassDeclarationSyntax || s is EnumDeclarationSyntax,
            ((ctx, _) => ctx.Node));


        // Generate the source code.
        context.RegisterSourceOutput(context.CompilationProvider.Combine(provider2.Collect()),
             ((ctx, t) => GenerateCode(ctx, t.Left, t.Right)));
    }



    public List<string> GetUsings(SyntaxNode syntaxNode)
    {
        if (syntaxNode != null)
        {
            var unit = syntaxNode.GetCompilationUnit();
            return unit.Usings.Select(x => x.ToString()).Where(x => !x.ToString().Contains("csly.generator")).ToList();
        }
        return new List<string>();
    }


    private List<ParserGeneratorTriplet> GetClassDeclarationsForSourceGen(SourceProductionContext context, Compilation compilation, ImmutableArray<SyntaxNode> declarations)
    {
        Func<SyntaxNode, string> getName = (node) =>
        {
            if (node is ClassDeclarationSyntax classDeclarationSyntax)
            {
                return classDeclarationSyntax.Identifier.ToString();
            }

            if (node is EnumDeclarationSyntax enumDeclarationSyntax)
            {
                return enumDeclarationSyntax.Identifier.ToString();
            }

            return "";
        };

        var declarationsByName = declarations.ToDictionary(x => getName(x));
        List<ParserGeneratorTriplet> generators = new List<ParserGeneratorTriplet>();
        foreach (var declaration in declarations)
        {
            if (declaration is ClassDeclarationSyntax classDeclarationSyntax)
            {
                
                
                var (lexerType, parserType, outputType, isParserGenerator) =
                    GetClassDeclaration(classDeclarationSyntax);

                if (isParserGenerator)
                {
                    bool hasError = false;
                    ClassDeclarationSyntax generatorClass = null;
                    ClassDeclarationSyntax parserClass = null;
                    EnumDeclarationSyntax lexerEnum = null;
                    
                    var isPartial =
                        classDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword));
                    if (!isPartial)
                    {
                        hasError = true;
                        context.ReportDiagnostic(Diagnostic.Create(
                            new DiagnosticDescriptor(
                                CslyGeneratorErrors.NOT_PARTIAL,
                                "Parser generator is not partial",
                                "Parser generator {0} is not partial",
                                "csly",
                                DiagnosticSeverity.Error,
                                true), classDeclarationSyntax.GetLocation(), classDeclarationSyntax.Identifier.Text));
                        continue;
                    }

                    if (!classDeclarationSyntax.BaseList.Types.Any(x => x.Type.ToString().StartsWith("AbstractParserGenerator<")))
                    {
                        hasError = true;
                        string inheritance = string.Join(", ", classDeclarationSyntax.BaseList.Types.Select(x => $"[{x.Type.ToString()}]"));
                        context.ReportDiagnostic(Diagnostic.Create(
                            new DiagnosticDescriptor(
                                CslyGeneratorErrors.MISSING_INHERITANCE,
                                "Parser generator does not inherit from AbstractParserGenerator [2]",
                                "Parser generator {0} does not inherit from AbstractParserGenerator [2] : {1}",
                                "csly",
                                DiagnosticSeverity.Error,
                                true), classDeclarationSyntax.GetLocation(), classDeclarationSyntax.Identifier.Text, inheritance));
                        continue;
                    }

                    
                    if (declarationsByName.TryGetValue(lexerType, out var lexerDeclaration)) {
                        lexerEnum = lexerDeclaration as EnumDeclarationSyntax;
                    }
                    else
                    {
                        hasError = true;
                        context.ReportDiagnostic(Diagnostic.Create(
                            new DiagnosticDescriptor(
                                CslyGeneratorErrors.LEXER_NOT_FOUND,
                                "missing lexer enum declaration",
                                "Lexer Enum {0} not found.",
                                "csly",
                                DiagnosticSeverity.Error,
                                true), classDeclarationSyntax.GetLocation(), lexerType));
                        continue;
                    }

                    if (declarationsByName.TryGetValue(parserType, out var parserDeclaration))
                    {
                        parserClass = parserDeclaration as ClassDeclarationSyntax;
                    }
                    else
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            new DiagnosticDescriptor(
                                CslyGeneratorErrors.LEXER_NOT_FOUND,
                                "missing lexer enum declaration",
                                "Lexer Enum {0} not found.",
                                "csly",
                                DiagnosticSeverity.Error,
                                true), classDeclarationSyntax.GetLocation(), lexerType));
                        continue;
                    }

                    if (!hasError)
                    {
                        var generator = new ParserGeneratorTriplet(declaration as ClassDeclarationSyntax, parserClass, lexerEnum, outputType);
                        generators.Add(generator);
                    }
                }
            }
        }
        return generators;
    }

    private HashSet<string> hints = new HashSet<string>();
    
    private void GenerateCodeForGenerator(ParserGeneratorTriplet generator, Dictionary<string,SyntaxNode> declarationsByName, SourceProductionContext context,
        Compilation compilation)
    {
        string ns = generator.GeneratorClass.GetNameSpace();
        string lexerName = generator.LexerEnum.Identifier.ToString();
        string assemblyName = compilation.AssemblyName;

        string parserSubNameSpace = generator.ParserClass.Identifier.ToString().ToLower();
            
        string thisParserBaseNameSpace = $"csly.{assemblyName}.{parserSubNameSpace}";
        
        
        
        var usings = GetUsings(generator.LexerEnum);
        usings.AddRange(GetUsings(generator.ParserClass));
        usings.Add($"using {generator.LexerEnum.GetNameSpace()};");
        usings.Add($"using {generator.ParserClass.GetNameSpace()};");
        usings.AddRange(new[]
        {
            "using System;", $"using {thisParserBaseNameSpace}.models;",
            "using System.Collections.Generic;"
        });
        usings = usings.Distinct().ToList();
        
        StaticLexerBuilder staticLexerBuilder = new StaticLexerBuilder(lexerName, ns);
        
        ;
        TemplateEngine templateEngine = new TemplateEngine("", "", "", "");
        var models = templateEngine.GetAllTemplateNamesForFolder("model");
        foreach (var model in models)
        {
            var content = templateEngine.ApplyTemplate(model, additional: new Dictionary<string, string>() { { "NS", $"{thisParserBaseNameSpace}.models" } });
            string filename = $"{model}.{generator.ParserClass.Identifier.ToString()}.g.cs";
            if (hints.Contains(filename))
            {
                ;
            }
            hints.Add(filename);
            context.AddSource(filename, SourceText.From(content, Encoding.UTF8));
        }
        
                    LexerBuilderGenerator lexerGenerator = new LexerBuilderGenerator(staticLexerBuilder, assemblyName);
                    try
                    {
                        lexerGenerator.AnalyseLexer(generator.LexerEnum, parserSubNameSpace, declarationsByName);
                    }
                    catch (Exception e)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            new DiagnosticDescriptor(
                                CslyGeneratorErrors.LEXER_GENERATION_FAILED,
                                "lexer generation failed",
                                "lexer generation failed for {0} : {1}, {2}",
                                "csly",
                                DiagnosticSeverity.Error,
                                true), generator.GeneratorClass.GetLocation(), lexerName, e.Message, e.StackTrace));
                        return;
                    }

                    ParserBuilderGenerator parserBuilderGenerator =
                        new ParserBuilderGenerator(lexerName, generator.ParserClass.Identifier.ToString(), generator.OutputType, thisParserBaseNameSpace, lexerGenerator.Tokens);
                    try
                    {
                        var staticParser = parserBuilderGenerator.GenerateParser(generator.ParserClass);
                        string parserCode = $@"

{string.Join(Environment.NewLine, usings)}


    {staticParser}

";

                        context.AddSource($"{generator.ParserClass.Identifier.ToString()}.g.cs", SourceText.From(parserCode, Encoding.UTF8));

                        var staticVisitor2 = parserBuilderGenerator.GenerateVisitor2();
                        string visitorCode = $@"

{string.Join(Environment.NewLine, usings)}

    {staticVisitor2}

";

                        context.AddSource($"{generator.ParserClass.Identifier.ToString()}Visitor2.g.cs", SourceText.From(visitorCode, Encoding.UTF8));

                    }
                    catch (Exception e)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            new DiagnosticDescriptor(
                                CslyGeneratorErrors.PARSER_GENERATION_FAILED,
                                "parser generation failed",
                                "parser generation failed for {0} : {1}, {2}",
                                "csly",
                                DiagnosticSeverity.Error,
                                true), generator.GeneratorClass.GetLocation(), generator.ParserClass.Identifier.ToString(), e.Message, e.StackTrace));
                        return;
                    }

                    try
                    {
                        // TODO : add explicit tokens if needed : ⚠️ explicit tokens are only in default mode !
                        var explicitTokens = parserBuilderGenerator.GetExplicitTokens();
                        staticLexerBuilder.SetExplicitTokens(explicitTokens);
                        List<(string mode, string lexer, string fsm)> subLexers = lexerGenerator.GenerateSubLexers();

                        foreach (var sublexer in subLexers)
                        {

                            var staticLexer = @$"
{string.Join(Environment.NewLine, usings)}


   {sublexer.lexer}
";

                            context.AddSource($"Static{lexerName}_{sublexer.mode}.g.cs", SourceText.From(staticLexer, Encoding.UTF8));



                            string fsmDumpCode = $@"
/****************************
/** this the state machine dump for lexer {lexerName}_{sublexer.mode}
/****************************

{sublexer.fsm}
*/";

                            context.AddSource($"FSM_{lexerName}_{sublexer.mode}.g.cs", SourceText.From(fsmDumpCode, Encoding.UTF8));

                        }
                        templateEngine = new TemplateEngine(lexerName, generator.ParserClass.Identifier.ToString(), generator.OutputType, ns);
                        var iSubLexer = templateEngine.ApplyTemplate("ISubLexerTemplate", additional: new Dictionary<string, string>
                        {
                            {"ASSEMBLY", assemblyName },
                            {"LEXER",generator.LexerEnum.Identifier.ToString()},
                        });
                        context.AddSource($"ISubLexer.{parserSubNameSpace}.g.cs", SourceText.From(iSubLexer, Encoding.UTF8));

                        var autoCloseIndentations = templateEngine.ApplyTemplate("AutoCloseIndentationsTemplate", additional : new Dictionary<string, string>()
                        {
                            {"AUTO_CLOSE_INDENTATIONS",staticLexerBuilder.AutoCloseIndentations.ToString().ToLower()},
                            {"IS_INDENTATION_AWARE", staticLexerBuilder.IsIndentationAware.ToString().ToLower()}
                        });
                        
                        var mainLexer = templateEngine.ApplyTemplate("MainLexerTemplate", additional : new Dictionary<string, string>
                        {
                            {"LEXER", lexerName },
                            {"ASSEMBLY", assemblyName },
                            {"NAMESPACE", ns },
                            {"AUTO_CLOSE", autoCloseIndentations},
                            {"SUB_LEXERS", string.Join($",{Environment.NewLine}", subLexers.Select(x => $"{{ \"{x.mode}\", new {lexerName}_FsmLexer_{x.mode}() }}")) }
                        });

                        context.AddSource($"{lexerName}_MainLexer.cs", SourceText.From(mainLexer, Encoding.UTF8));

                    }
                    catch (Exception e)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            new DiagnosticDescriptor(
                                CslyGeneratorErrors.LEXER_GENERATION_FAILED,
                                "lexer generation failed",
                                "lexer generation failed for {0} : {1}, {2}",
                                "csly",
                                DiagnosticSeverity.Error,
                                true), generator.GeneratorClass.GetLocation(), lexerName, e.Message, e.StackTrace));
                        return;
                    }

                    // ***********
                    // entry point

                    var main = parserBuilderGenerator.GenerateEntryPoint(ns);
                    string code = $@"

{string.Join(Environment.NewLine, usings)}

    {main}
";
                    context.AddSource($"Main{generator.ParserClass.Identifier.ToString()}.g.cs", SourceText.From(code, Encoding.UTF8));
    }
    
    
    private void GenerateCode(SourceProductionContext context, Compilation compilation,    
        ImmutableArray<SyntaxNode> declarations)
    {
        var generators = GetClassDeclarationsForSourceGen(context, compilation, declarations);
        var assemblyName = compilation.AssemblyName;
        var declarationsByName = declarations.ToDictionary(x => GetName(x));
        foreach (var generator in generators)
        {
            string thisParserBaseNameSpace = $"csly.{assemblyName}.{generator.ParserClass.Identifier.ToString().ToLower()}" ;
            TemplateEngine templateEngine = new TemplateEngine("", "", "", "");
            
            GenerateCodeForGenerator(generator,declarationsByName, context, compilation);
           
        } 
        
    }
    
    private string GetName (SyntaxNode node) 
    {
        if (node is ClassDeclarationSyntax classDeclarationSyntax)
        {
            return classDeclarationSyntax.Identifier.ToString();
        }

        if (node is EnumDeclarationSyntax enumDeclarationSyntax)
        {
            return enumDeclarationSyntax.Identifier.ToString();
        }

        return "";
    }
    
   

    private static (ClassDeclarationSyntax classDeclarationSyntax, string lexerType, string parserType, bool
        parserGeneratorAttributeFound) GetClassDeclarationForSourceGen(
            GeneratorSyntaxContext context)
    {

        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        // Go through all attributes of the class.
        foreach (AttributeListSyntax attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
            {
                string name = attributeSyntax.Name.ToString();
                if (name == "ParserGenerator")
                {
                    if (attributeSyntax.ArgumentList != null && attributeSyntax.ArgumentList.Arguments.Count == 2)
                    {
                        var arg1 = attributeSyntax.ArgumentList.Arguments[0];
                        var arg2 = attributeSyntax.ArgumentList.Arguments[1];
                        if (arg1.Expression is TypeOfExpressionSyntax typeOfLexer &&
                            arg2.Expression is TypeOfExpressionSyntax typeOfParser)
                        {
                            return (classDeclarationSyntax, typeOfLexer.Type.ToString(), typeOfParser.Type.ToString(),
                                true);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
        }

        return (classDeclarationSyntax, null, null, false);
    }

    private static (string lexerType, string parserType, string outputType, bool parserGeneratorAttributeFound)
        GetClassDeclaration(
            ClassDeclarationSyntax classDeclarationSyntax)
    {
        Console.WriteLine($"Analyzing class {classDeclarationSyntax.Identifier.Text}");
        // Go through all attributes of the class.
        foreach (AttributeListSyntax attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
            {
                string name = attributeSyntax.Name.ToString();
                if (name == "ParserGenerator")
                {
                    Console.WriteLine($"  Found ParserGenerator attribute :)");
                    if (classDeclarationSyntax?.BaseList?.Types != null)
                    {
                        var abstractParser = classDeclarationSyntax.BaseList.Types.FirstOrDefault(x => x.Type.ToString().Contains("AbstractParserGenerator"));

                        if (abstractParser != null)
                        {
                            var args = (abstractParser.Type as GenericNameSyntax)?.TypeArgumentList?.Arguments;
                            Console.WriteLine($"  Found AbstractParserGenerator base class :) with {args.Value.Count} : <{string.Join(", ",args.Value.Select(x => x.ToString()))}>");
                            
                            if (args != null && args.HasValue && args.Value.Count == 3)
                            {
                                var l = args.Value[0];
                                var p = args.Value[1];
                                var o = args.Value[2];
                                return (l.ToString(), p.ToString(),
                                    o.ToString(),
                                    true);

                            }
                        }
                    }

                    if (attributeSyntax.ArgumentList != null && attributeSyntax.ArgumentList.Arguments.Count == 3)
                    {
                        var arg1 = attributeSyntax.ArgumentList.Arguments[0];
                        var arg2 = attributeSyntax.ArgumentList.Arguments[1];
                        var arg3 = attributeSyntax.ArgumentList.Arguments[2];
                        if (arg1.Expression is TypeOfExpressionSyntax typeOfLexer &&
                            arg2.Expression is TypeOfExpressionSyntax typeOfParser &&
                            arg3.Expression is TypeOfExpressionSyntax typeOfOutput)
                        {
                            return (typeOfLexer.Type.ToString(), typeOfParser.Type.ToString(),
                                typeOfOutput.Type.ToString(),
                                true);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
        }

        return (null, null, null, false);
    }
}
