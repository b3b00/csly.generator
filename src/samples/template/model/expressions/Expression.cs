using System.Collections.Generic;
using template.model;
using csly.template.models;

namespace template.model.expressions
{
    public interface Expression : ITemplate
    {

        object Evaluate(Dictionary<string, object> context);

        

    }
}