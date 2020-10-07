using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D3_Sqex03DataMessage
{
    class DataMessage
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public List<string> Strings { get; set; }
        public DataMessage (string name, int index, List<string> strings)
        {
            Name = name;
            Index = index;
            Strings = strings;
        }
    }
}
