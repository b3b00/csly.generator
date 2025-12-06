using System.Collections.Generic;
using csly.whileLang.compiler;

namespace csly.whileLang
{
    public interface Expression : WhileAST
    {
        WhileType Whiletype { get; set; }
        
    }
}