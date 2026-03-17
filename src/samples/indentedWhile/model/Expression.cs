using System.Collections.Generic;
using csly.indented.whileLang.compiler;

namespace csly.indented.whileLang.model
{
    public interface Expression : WhileAST
    {
        WhileType Whiletype { get; set; }
        
    }
}