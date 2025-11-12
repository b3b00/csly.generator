namespace csly.generator.model.lexer.attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PushAttribute : Attribute
    {
        public string TargetMode { get; }

        public PushAttribute(string mode)
        {
            TargetMode = mode;
        }
    }
}