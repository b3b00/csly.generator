using csly.indented.whileLang.compiler;
using csly.indentedWhile.indentedwhileparsergeneric.models;
using Sigil;

namespace csly.indented.whileLang.model
{
    public interface WhileAST
    {
        LexerPosition Position { get; set; }

        Scope CompilerScope { get; set; }
        string Dump(string tab);

        string Transpile(CompilerContext context);
        Emit<Func<int>> EmitByteCode(CompilerContext context, Emit<Func<int>> emiter);
        
        void AppendTernaries(List<TernaryExpression> ternaries);
    }
}