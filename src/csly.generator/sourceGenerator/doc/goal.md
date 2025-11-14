# Goal

the goal of the parser source generator is to produce a standlaone (0 dependency) parser.
That is to say that the parser **MUST NOT** depend on the legacy CSLy runtime.


# sample

## definition

you need to define : 
	- a lexer enum as with CSLY
	- a parser class as with CSLY
	- a generator class that will trigger the lexer/paser/visitor source generation

the inheritage from AbstractParserGenerator is mandatory to define :
	- the token enum
	- the parser class
	- the parser output type

```csharp
using csly.models;

namespace sourceGenerationTester.expressionParser;

[ParserGenerator]
public partial class Expression : AbstractParserGenerator<ExpressionToken, ExpressionParser, int>
{
        
}
```


## usage

The generated lexer is named after the enum lexer with a `Static` prefix

```csharp
StaticExpressionToken scanner = new StaticExpressionToken();
var lexerResult = scanner.Scan(choice.AsSpan());
```

The generated parser is named after the parser class with a `Static` prefix.
the generated parser exposes methods named after the non-terminals prefixed with `ParseNonTerminal_`
```csharp
var parser = new StaticExpressionParser();
var result = parser.ParseNonTerminal_expression(lexerResult.Tokens, 0);
```