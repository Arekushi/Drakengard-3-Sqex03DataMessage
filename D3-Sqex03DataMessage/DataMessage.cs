using System;
using System.Collections.Generic;

namespace D3_Sqex03DataMessage
{
    public class DataMessage
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
