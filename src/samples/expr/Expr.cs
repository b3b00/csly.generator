
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using csly.models;

namespace expr;

[ParserGenerator]
public partial class Expr : AbstractParserGenerator<ExprToken, ExprParser, string>
{
}

