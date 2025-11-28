public <#CLAUSE_OUTPUT#> VisitOption_<#NAME#>(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node)
{
                if (node.Children.Count == 0)
    {
        return <#EMPTY_VALUE#>;

    }
                else
    {        
            <#VISITOR#>
            return new <#CLAUSE_OUTPUT#>(arg0);
    }
}