using csly.ebnf.builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csly.generator.sourceGenerator
{
    internal class ExpressionRulesGenerator
    {

        public ExpressionRulesGenerator()
        {
        }

        public void Generate(ParserModel model)
        {
            var operationsByPrecedence = model.Operations.GroupBy(x => x.Precedence).OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.ToList());
        }

    }
}
