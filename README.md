## Goal

csly.generator aims at clone [CSLY](https://github.com/b3b00/csly) features using C# source generators.

## Usage

API is mostly identical to CSLY. All `sly.*` using must simply be replaced by `using csl.models`.

Add the folliwing to your csproj :
```xml
<ItemGroup>
    <ProjectReference Include="..\..\csly.generator\csly.generator.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
</ItemGroup>
```

## State of the API

For now not all the CSLY features. 
Here is the list of missing features :
 - regex lexer : wwill not be implemented
 - generic lexer :
   - indentations
   - comments
   - lexer modes (and related Upto token)
 - EBNF parser
   - indentation aware parser (ala Python)
   - left recursion detection
   - memoization optimization
  
Error reporting is also still a work in progress.        

   
