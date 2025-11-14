using System;

namespace <#NS#>;


[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public class KeywordAttribute : LexemeAttribute
{
    public KeywordAttribute(string keyword, int channel = Channels.Main) : base(GenericToken.KeyWord, keyword)
    {
        
    }
}