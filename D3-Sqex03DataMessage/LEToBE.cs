using System;

namespace D3_Sqex03DataMessage
{
    class LEToBE
    {
        public static byte[] Convert(byte[] bytes)
        {
            Array.Reverse(bytes);
            return bytes;
        }
    }
}
