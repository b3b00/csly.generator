namespace jsonparser.JsonModel
{
    public class JNull : JSon
    {
        public override bool IsNull => true;

        public override string Dump() => "null";

        public override int GetDepth()
        {
            return 0;
        }
    }
}