namespace csly.generator.model.parser.attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class SubNodeNamesAttribute : Attribute
{
    public  string[] Names { get;  } = null;

    public SubNodeNamesAttribute(params string[] names)
    {
        Names = names;
    }
}