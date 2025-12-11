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

[Generator]
public class CslyParserGenerator : IIncrementalGenerator
{
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        Dictionary<ClassDeclarationSyntax,(string lexerType, string parserType)> _lexerAndParserTypes = new();
        
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
            ((ctx,_) =>  ctx.Node ));

        
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
    
    private void GenerateCode(SourceProductionContext context, Compilation compilation, ImmutableArray<SyntaxNode> declarations)
    {

        //GeneratorLogger.Clean();
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


        // generate models

        TemplateEngine templateEngine = new TemplateEngine("", "", "", "");
        var models = templateEngine.GetAllTemplateNamesForFolder("model");
        foreach (var model in models)
        {
            var content = templateEngine.ApplyTemplate(model, additional: new Dictionary<string, string>() { { "NS", "csly.models"} });
            context.AddSource($"{model}.g.cs", SourceText.From(content, Encoding.UTF8));
        }


        Dictionary<string, SyntaxNode> declarationsByName = declarations.ToDictionary(x => getName(x));
        
        foreach (var declarationSyntax in declarations)
        {
            if (declarationSyntax is ClassDeclarationSyntax classDeclarationSyntax)
            {

                var className = classDeclarationSyntax.Identifier.Text;
               

                var (lexerType, parserType, outputType, isParserGenerator) = GetClassDeclaration(classDeclarationSyntax);

                if (isParserGenerator)
                {
                    var isPartial =
                        classDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword));
                    if (!isPartial)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            new DiagnosticDescriptor(
                                CslyGeneratorErrors.NOT_PARTIAL,
                                "Parser generator is not partial",
                                "Parser generator {0} is not partial",
                                "csly",
                                DiagnosticSeverity.Error,
                                true), classDeclarationSyntax.GetLocation(), classDeclarationSyntax.Identifier.Text));
                    }

                    if (!classDeclarationSyntax.BaseList.Types.Any(x => x.Type.ToString().StartsWith("AbstractParserGenerator<")))
                    {
                        string inheritance = string.Join(", ",classDeclarationSyntax.BaseList.Types.Select(x => $"[{x.Type.ToString()}]"));
                        context.ReportDiagnostic(Diagnostic.Create(
                            new DiagnosticDescriptor(
                                CslyGeneratorErrors.MISSING_INHERITANCE,
                                "Parser generator does not inherit from AbstractParserGenerator [2]",
                                "Parser generator {0} does not inherit from AbstractParserGenerator [2] : {1}",
                                "csly",
                                DiagnosticSeverity.Error,
                                true), classDeclarationSyntax.GetLocation(), classDeclarationSyntax.Identifier.Text, inheritance));
                    }
                    
                    
                    
                    string ns = declarationSyntax.GetNameSpace();
                
                    

                    if (!declarationsByName.ContainsKey(lexerType))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            new DiagnosticDescriptor(
                                CslyGeneratorErrors.LEXER_NOT_FOUND,
                                "missing lexer enum declaration",
                                "Lexer Enum {0} not found.",
                                "csly",
                                DiagnosticSeverity.Error,
                                true), classDeclarationSyntax.GetLocation(), lexerType));
                                return;
                    }
                    
                    EnumDeclarationSyntax lexerDecl = declarationsByName[lexerType] as EnumDeclarationSyntax;
                    
                   
                    
                    if (!declarationsByName.ContainsKey(parserType))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            new DiagnosticDescriptor(
                                CslyGeneratorErrors.PARSER_NOT_FOUND,
                                "missing parser class declaration",
                                "Parser class {0} not found.",
                                "csly",
                                DiagnosticSeverity.Error,
                                true), classDeclarationSyntax.GetLocation(), parserType));
                        return;
                    }
                    
                    var parserDecl = declarationsByName[parserType] as ClassDeclarationSyntax;
                    
                    string lexerName = lexerDecl.Identifier.ToString();
                   
                    string modifiers = string.Join(" ", classDeclarationSyntax.Modifiers.Select(x => x.ToString()));

                    var usings = GetUsings(lexerDecl);
                    usings.AddRange(GetUsings(parserDecl));
                    usings.Add($"using {lexerDecl.GetNameSpace()};");
                    usings.Add($"using {parserDecl.GetNameSpace()};");
                    usings.AddRange(new[]
                    {
                        "using System;", "using csly.models;",
                        "using System.Collections.Generic;"
                    }); 
                    usings = usings.Distinct().ToList();
                    
                    StaticLexerBuilder staticLexerBuilder = new StaticLexerBuilder(lexerName, ns);
                    LexerBuilderGenerator lexerGenerator = new LexerBuilderGenerator(staticLexerBuilder);
                    try
                    {                                                
                        lexerGenerator.AnalyseLexer(lexerDecl as EnumDeclarationSyntax, declarationsByName);
                    }
                    catch(Exception e) {                        
                        context.ReportDiagnostic(Diagnostic.Create(
                            new DiagnosticDescriptor(
                                CslyGeneratorErrors.LEXER_GENERATION_FAILED,
                                "lexer generation failed",
                                "lexer generation failed for {0} : {1}, {2}",
                                "csly",
                                DiagnosticSeverity.Error,
                                true), classDeclarationSyntax.GetLocation(), lexerName, e.Message, e.StackTrace));
                        return;
                    }

                    ParserBuilderGenerator parserBuilderGenerator =
                        new ParserBuilderGenerator(lexerName, parserType, outputType, ns, lexerGenerator.Tokens);
                    try
                    {
                        var staticParser = parserBuilderGenerator.GenerateParser(parserDecl as ClassDeclarationSyntax);
                        string parserCode = $@"

{string.Join(Environment.NewLine, usings)}


    {staticParser}

";

                        context.AddSource($"{className}.g.cs", SourceText.From(parserCode, Encoding.UTF8));

                        var staticVisitor2 = parserBuilderGenerator.GenerateVisitor2();
                        string visitorCode = $@"

{string.Join(Environment.NewLine, usings)}

    {staticVisitor2}

";

                        context.AddSource($"{className}Visitor2.g.cs", SourceText.From(visitorCode, Encoding.UTF8));

                    }
                    catch(Exception e)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            new DiagnosticDescriptor(
                                CslyGeneratorErrors.PARSER_GENERATION_FAILED,
                                "parser generation failed",
                                "parser generation failed for {0} : {1}, {2}",
                                "csly",
                                DiagnosticSeverity.Error,
                                true), classDeclarationSyntax.GetLocation(), parserType, e.Message, e.StackTrace));
                        return;
                    }

                    try
                    {
                        // TODO : add explicit tokens if needed
                        var explicitTokens = parserBuilderGenerator.GetExplicitTokens();
                        staticLexerBuilder.SetExplicitTokens(explicitTokens);
                        var t = lexerGenerator.GenerateLexer();
                        var staticLexer = @$"
{string.Join(Environment.NewLine, usings)}


   {t}
";

                        context.AddSource($"Static{lexerName}.g.cs", SourceText.From(staticLexer, Encoding.UTF8));

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
                                true), classDeclarationSyntax.GetLocation(), lexerName, e.Message, e.StackTrace));
                        return;
                    }

                    // ***********
                    // entry point

                    var main = parserBuilderGenerator.GenerateEntryPoint(ns);
                    string code = $@"

{string.Join(Environment.NewLine, usings)}

    {main}
";
                    context.AddSource($"Main{className}.g.cs", SourceText.From(code, Encoding.UTF8));
                }
            }
        }
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

        // Go through all attributes of the class.
        foreach (AttributeListSyntax attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
            {
                string name = attributeSyntax.Name.ToString();
                if (name == "ParserGenerator")
                {
                    if (classDeclarationSyntax?.BaseList?.Types != null)
                    {
                        var abstractParser = classDeclarationSyntax.BaseList.Types.FirstOrDefault(x => x.Type.ToString().Contains("AbstractParserGenerator"));
                        if (abstractParser != null)
                        {
                            var args = (abstractParser.Type as GenericNameSyntax)?.TypeArgumentList?.Arguments;
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
