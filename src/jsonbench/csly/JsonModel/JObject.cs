namespace jsonbench.csly.JsonModel
{
    public class JObject : jsonbench.csly.JsonModel.JSon
    {
        public JObject(string key, jsonbench.csly.JsonModel.JSon value)
        {
            Values = new Dictionary<string, jsonbench.csly.JsonModel.JSon>();
            Values[key] = value;
        }

        public JObject()
        {
            Values = new Dictionary<string, jsonbench.csly.JsonModel.JSon>();
        }

        public JObject(Dictionary<string, jsonbench.csly.JsonModel.JSon> dic)
        {
            Values = dic;
        }

        public override bool IsObject => true;

        public override bool IsList => true;

        private Dictionary<string, jsonbench.csly.JsonModel.JSon> Values { get; }

        public int Count => Values.Count;

        public jsonbench.csly.JsonModel.JSon this[string key]
        {
            get => Values[key];
            set => Values[key] = value;
        }


        public void Merge(JObject obj)
        {
            foreach (var pair in obj.Values) this[pair.Key] = pair.Value;
        }

        public bool ContainsKey(string key)
        {
            return Values.ContainsKey(key);
        }
    }
}