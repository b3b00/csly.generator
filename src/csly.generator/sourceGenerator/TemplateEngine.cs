using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace csly.generator.sourceGenerator;

public class TemplateEngine
{
    // private readonly EmbeddedResourceFileSystem _resourceFileSystem = null;

    private readonly string _lexerName;
    
    private readonly string _parserName;
    
    private readonly string _outputType;

    private const string LEXER_PLACEHOLDER = "<#LEXER#>";
    private const string PARSER_PLACEHOLDER = "<#PARSER#>";
    private const string OUTPUT_PLACEHOLDER = "<#OUTPUT#>";
    private const string NAME_PLACEHOLDER = "<#NAME#>";
    
    public TemplateEngine(string lexerName, string parserName, string outputType)
    {
        _lexerName = lexerName;
        _parserName = parserName;
        _outputType = outputType;
        //_resourceFileSystem =  new EmbeddedResourceFileSystem(typeof(TemplateEngine).Assembly);
    }

    
    private const string regex = @"^csly\.generator\.sourceGenerator\.staticParserTemplates\.(lexer|parser)\.(.*)\.cs$";
    private ImmutableDictionary<string, string> FullyQualifiedTypeNamesToResourceNames = ImmutableDictionary.CreateRange(
    from string resource in typeof(TemplateEngine).Assembly.GetManifestResourceNames()
    select new KeyValuePair<string, string>(Regex.Match(resource, regex).Groups[2].Value, resource));

    public string GetTemplate(string name)
    {
        
        string resourceName = FullyQualifiedTypeNamesToResourceNames[name];

        using Stream stream = typeof(TemplateEngine).Assembly.GetManifestResourceStream(resourceName);
        if (stream != null)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                var content = reader.ReadToEnd();
                return content;
            }
        }


        return $"!!!!!!!!!!!!!!!!!!!!!! unable to find template >{name}< !!!!!!!!!!!!!!!!!!!!";
    }


    public string ApplyTemplate(string templateName, string name = null, bool substitute = true, Dictionary<string, string> additional = null)
    {
        var template = GetTemplate(templateName);
        if (substitute)
        {
            template = Substitute(template, LEXER_PLACEHOLDER, _lexerName);
            template = Substitute(template, PARSER_PLACEHOLDER, _parserName);
            template = Substitute(template, OUTPUT_PLACEHOLDER, _outputType);
            if (name != null)
            {
                template = Substitute(template, NAME_PLACEHOLDER, name);
            }

            if (additional != null)
            {
                foreach (var pair in additional)
                {
                    template = Substitute(template, $"<#{pair.Key}#>", pair.Value);
                }
            }
        }
        return template;
    }

    private string Substitute(string content, string placeholder, string value)
    {
        return content.Replace(placeholder, value);
    }
}