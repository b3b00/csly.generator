public List<<#CLAUSE_OUTPUT#>> Visit<#NAME#>(SyntaxNode<<#LEXER#>, <#OUTPUT#>> node)
{
    
        if (node.Children.Count == 0)
        {
            return new List<<#CLAUSE_OUTPUT#>>();
        }
        else
        {

            var results = new List<<#CLAUSE_OUTPUT#>>();
            for (int i = 0; i < node.Children.Count; i++)
            {
                <#VISITOR#>
                results.Add(argi);
            }
            return results;
        }        
    
    return new List<<#CLAUSE_OUTPUT#>>();
}