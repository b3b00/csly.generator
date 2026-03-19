using System.Collections.Generic;

namespace template.model.expressions
{
    public interface Expression : ITemplate
    {

        object Evaluate(Dictionary<string, object> context);

        

    }
}