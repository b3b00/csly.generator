using System.Collections.Generic;
using System.Text;

namespace template.model
{
    public class Template : Block
    {
        public Template(List<ITemplate> items) : base(items)
        {
        }
    }
}