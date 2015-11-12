using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GothosDC
{
    class Program
    {
        static void Main(string[] args)
        {
            var dc = DataCenterRegions.Load(args[0]);
            foreach (var s in dc.PrintLegacyRegions())
            {
                Console.WriteLine(s);
            }
        }
    }
}
