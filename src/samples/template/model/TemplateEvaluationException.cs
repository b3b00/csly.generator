using System;

namespace template.model
{
    public class TemplateEvaluationException : Exception
    {
        public TemplateEvaluationException(string error) : base(error)
        {
        }
    }
}