using System.Collections.Generic;
using System.Text;

namespace jsonparser.JsonModel
{
    public class JList : JSon

    {
        public JList()
        {
            Items = new List<JSon>();
        }

        public JList(List<JSon> lst)
        {
            Items = lst;
        }


        public JList(JSon item)
        {
            Items = new List<JSon>();
            Items.Add(item);
        }

        public override bool IsList => true;

        public List<JSon> Items { get; }

        public int Count => Items.Count;

        public JSon this[int index]
        {
            get => Items[index];
            set => Items[index] = value;
        }

        public void Add(JSon item)
        {
            Items.Add(item);
        }

        public void AddRange(JList items)
        {
            Items.AddRange(items.Items);
        }

        public void AddRange(List<JSon> items)
        {
            Items.AddRange(items);
        }

        public override string Dump()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[");
            for (int i = 0; i < Items.Count; i++)   
            {
                sb.Append("  ");
                sb.Append(Items[i].Dump());
                if (i < Items.Count - 1)
                {
                    sb.Append(",");
                }
                sb.AppendLine();
            }
            sb.AppendLine("]");
            return sb.ToString();
        }
    }
}