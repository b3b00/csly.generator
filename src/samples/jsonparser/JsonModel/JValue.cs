namespace jsonparser.JsonModel
{
    public class JValue : JSon
    {
        private readonly object value;

        public JValue(object val)
        {
            value = val;
        }

        public override bool IsValue => true;

        public bool IsString => value is string;

        public bool IsInt => value is int;

        public bool IsDouble => value is double;

        public bool IsBool => value is bool;

        public override int GetDepth()
        {
            return 0;
        }

        public T GetValue<T>()
        {
            return (T) value;
        }

        public override string Dump()
        {
            if (IsString)
            {
                return $"\"{value}\"";
            }
            if (IsDouble) {
                return value.ToString().Replace(",",".");
            }
            return value.ToString();
        } 
    }
}