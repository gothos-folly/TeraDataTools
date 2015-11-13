using System;
using DCTools.Structures;

namespace DCTools
{
    public class DCT
    {
        public static GothosDC.DataCenter DataCenter { get; set; }

        public static DataCenter GetDataCenter()
        {
            return new DataCenter(DataCenter);
        }
    }
}