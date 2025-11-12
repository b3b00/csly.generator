namespace csly.generator.model.lexer.attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class DoubleAttribute : LexemeAttribute
    {
        public DoubleAttribute(string decimalDelimiter = ".", int channel = Channels.Main) : base(GenericToken.Double,
            channel:channel, parameters:decimalDelimiter)
        {
        }
    }
}