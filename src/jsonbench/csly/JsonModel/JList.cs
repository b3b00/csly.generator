namespace jsonbench.csly.JsonModel
{
    public class JList : jsonbench.csly.JsonModel.JSon

    {
        public JList()
        {
            Items = new List<jsonbench.csly.JsonModel.JSon>();
        }

        public JList(List<jsonbench.csly.JsonModel.JSon> lst)
        {
            Items = lst;
        }


        public JList(jsonbench.csly.JsonModel.JSon item)
        {
            Items = new List<jsonbench.csly.JsonModel.JSon>();
            Items.Add(item);
        }

        public override bool IsList => true;

        public List<jsonbench.csly.JsonModel.JSon> Items { get; }

        public int Count => Items.Count;

        public jsonbench.csly.JsonModel.JSon this[int index]
        {
            get => Items[index];
            set => Items[index] = value;
        }

        public void Add(jsonbench.csly.JsonModel.JSon item)
        {
            Items.Add(item);
        }

        public void AddRange(JList items)
        {
            Items.AddRange(items.Items);
        }

        public void AddRange(List<jsonbench.csly.JsonModel.JSon> items)
        {
            Items.AddRange(items);
        }
    }
}