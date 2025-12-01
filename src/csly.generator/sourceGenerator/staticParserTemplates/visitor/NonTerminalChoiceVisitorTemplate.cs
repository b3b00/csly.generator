public <#OUTPUT#> VisitChoice_<#NAME#>(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node)
{
    <#CHOICES#>
    throw new NotImplementedException($" unknown operand {node.Name}");
}