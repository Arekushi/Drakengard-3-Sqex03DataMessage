using System;
using System.Collections.Generic;

namespace D3_Sqex03DataMessage
{
    public class DataMessage
    {
        public string Name { get; set; }
        public List<string> Strings { get; set; }
        public bool IsSpeakerName { get; set; }
        public List<Speaker> Speakers { get; set; }
        public DataMessage (string name, List<string> strings, bool isSpeakerName)
        {
            Name = name;
            Strings = strings;
            IsSpeakerName = isSpeakerName;
        }
    }
}
