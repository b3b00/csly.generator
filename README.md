## Goal

csly.generator aims at clone [CSLY](https://github.com/b3b00/csly) features using C# source generators.

## How to generate a parser

API is mostly identical to CSLY. Only `using` are different, more on this below. 

### Add reference to generator

Add the following to your csproj :
```xml
<ItemGroup>
  <PackageReference Include="csly.generator" Version="0.9.5" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
</ItemGroup>
```

or 
```csharp
dotnet add package csly.generator --version 0.9.5
```

### create a parser generator

A parser generator must :
  - be decorated with the `[ParserGenerator]` attribute
  - be a `partial` class
  - inherit from `AbstractParserGenerator<LexerType, ParserType, OutputType>` where 
    - LexerType is the lexer definition enum
    - ParserType is the parser definition class
    - Outputtype is the output type of the parser
  
```csharp
[ParserGenerator]
public partial class TheParserGenerator : AbstractParserGenerator<TheLexer, TheParser, string>{
    
}
```

### On `usings`

All the types needed to define a parser are generated under the `csly.<ASSEMBLY_NAME>.<LOWER_PARSER_NAME>.models` namespace,where
   - `<ASSEMBLY_NAME>` is the name of the assembly 
   -  `<LOWER_PARSER_NAME>` is the lower cased class name of the parser 

So for the example above is assembly is `testAssembly`, you'll need to import namespace `using csly.testAssembly.theparser.models`
This may seem cumbersome, but it prevents any name conflicts between two parsers, whether they reside in the same assembly or not.


### Define your lexer and parser.

Just use the same syntax as for [CSLY](https://www.github.com/b3b00/csly), just replace all using sly.* with `using csly.<ASSEMBLY_NAME>.<LOWER_PARSER_NAME>.models`.

### build and use a parser

The generator will generate a class named `<PARSER_NAME>Main` as an entrypoint (where PARSER_NAME is the parser class name so for `TheParser` it will be `TheParserMain`)
You can instantiate the entrypoint class passing it an instance of your parser : 

```csharp
var instance = new TheParser();
var parser = new TheParserMain(instance);
```
the entrypoint expose a `Parse(string)` method to .... parse strings. this method returns a `ParseResult<Lexer,outputtype>` just as with CSLY.

So a complete example is :

```csharp
using csly.testAssembly.theparser.models;

namespace test;

[ParserGenerator]
public partial class TheParserGenerator : AbstractParserGenerator<TheLexer, TheParser, string>{
    
}


[ParserRoot("root")] // do not forget !
public class TheParser
{
    [Production("root : ID COLON[d] ints")]
    public string Root(Token<TheLexer> id, string ints)
    {
        return $"{id.Value} : {ints}";
    }

    [Production("ints : INT*")]
    public string ints(List<Token<TheLexer>> ints)
    {
        StringBuilder builder = new();
        builder.Append(string.Join("+", ints.Select(x => $"{x.Value}")));
        builder.Append(" = ");
        var sum = ints.Select(x => x.IntValue).Sum();
        builder.Append(sum);
        return builder.ToString();
    }
}

public enum TheLexer
{
    [AlphaId]
    ID,
    
    [Int]
    INT,
    
    [Sugar(":")]
    COLON,
}

public static void Main(string[] args)
{
    TheParser instance =  new TheParser();
    TheParserMain parser = new TheParserMain(instance);

    var result = parser.Parse("sum : 1 2 3 4 5 6 7 8 9 10");
    if (result.IsOk)
    {
        Console.WriteLine("parse OK ! ");
        Console.WriteLine(result.Result);
    }
    else 
    {
        Console.WriteLine("parse FAIL ! ");
        foreach (var error in result.Errors)
        {
            Console.WriteLine(error);
        }
    }
    
}
```












## State of the API

For now not all the CSLY features. 
Here is the list of missing features :
 - ❌ regex lexer : will not be implemented
 - generic lexer :
   - ✅ indentations 
   - ✅ comments
   - ✅ lexer modes (and related Upto token)
 - EBNF parser
   - ✅ indentation aware parser (ala Python)
   - ❌ left recursion detection (will come later)
   - ✅ memoization optimization
   - ✅ expression parsing
  
Error reporting is also still a work in progress.        

   
