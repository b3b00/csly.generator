using csly.models;

namespace sourceGenerationTester.visitor;

internal class <#PARSER#>Visitor
{
    private readonly <#PARSER#> _instance;

    public <#PARSER#>Visitor(<#PARSER#> instance)    {
        _instance = instance;
    }

    <#VISITORS#>

}