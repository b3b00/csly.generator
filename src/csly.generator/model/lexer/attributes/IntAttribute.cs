namespace csly.generator.model.lexer.attributes
{
    
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class IntAttribute : LexemeAttribute
    {
        public IntAttribute() : base(GenericToken.Int)
        {
            
        }
    }
}