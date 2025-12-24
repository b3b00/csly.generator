using System.Collections.Generic;

namespace template.model
{
    public interface ITemplate
    {
        string GetValue(Dictionary<string,object> context);
    }
}