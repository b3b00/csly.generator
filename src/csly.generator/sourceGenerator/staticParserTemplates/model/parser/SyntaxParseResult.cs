using System;
using System.Collections.Generic;
using System.Linq;

namespace <#NS#>;

public class SyntaxParseResult<IN, OUT> where IN : struct, Enum
{
    public ISyntaxNode<IN, OUT> Root { get; set; }

    public bool IsError { get; set; }

    public bool IsOk => !IsError;

    
    
    public HashSet<UnexpectedTokenSyntaxError<IN>> Errors { get; set; } 

    public int EndingPosition { get; set; }

    public bool IsEnded { get; set; }

    private void InitErrors()
    {
        if (Errors == null)
        {
            Errors = new HashSet<UnexpectedTokenSyntaxError<IN>>();
        }
    }
    
    public void AddErrors(IList<UnexpectedTokenSyntaxError<IN>> errors)
    {
        InitErrors();
        if (errors == null) return;
        if (errors.Count == 0) return;
        foreach (var error in errors)
        {
            AddError(error);
        }
    }

    public void AddErrors(HashSet<UnexpectedTokenSyntaxError<IN>> errors)
    {
        InitErrors();
        if (errors == null) return;
        if (errors.Count == 0) return;
        foreach (var error in errors.ToList())
        {
            AddError(error);
        }
    }

    public void AddError(UnexpectedTokenSyntaxError<IN> error)
    {
        InitErrors();
        Errors.Add(error);
    }

    public IList<UnexpectedTokenSyntaxError<IN>> GetErrors() => Errors?.ToList();

    public List<ParseError> GetParseErrors()
    {
        if (Errors == null) return new List<ParseError>();

        return Errors?.Cast<ParseError>().ToList() ?? new List<ParseError>();
    }
    
public List<ParseError> GetCompactedParseErrors()
    {
        if (Errors == null) return new List<ParseError>();
        
        var byEnding = Errors.GroupBy(x => x.UnexpectedToken.Position).OrderBy(x => x.Key);
        var errors = new List<ParseError>();  
        foreach (var expecting in byEnding)
        {
            var expectingTokens = expecting.SelectMany(x => x.ExpectedTokens ?? new List<LeadingToken<IN>>()).Distinct();
            var expectedTokens =  expectingTokens?.ToArray();
            if (expectedTokens != null)
            {
                var expected = new UnexpectedTokenSyntaxError<IN>(expecting.First().UnexpectedToken, null, "en",
                    expectedTokens);
                errors.Add(expected);
            }
            else
            {
                var expected = new UnexpectedTokenSyntaxError<IN>(expecting.First().UnexpectedToken, null, "en",
                    new LeadingToken<IN>[]{});
                errors.Add(expected);
            }
        }
        return errors;
    }

    public List<LeadingToken<IN>> Expecting {get; set;}

    public bool HasByPassNodes { get; set; } = false;
    public bool UsesOperations { get; set; }
}