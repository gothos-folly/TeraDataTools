using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GothosDC.LowLevel;

namespace GothosDC
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintLegacyRegions(args[0]);

            var dataCenter = DataCenter.Load(args[0]);
        }

        private static void PrintLegacyRegions(string filename)
        {
            var dataCenterRegions = DataCenterRegions.Load(filename);
            foreach (var s in dataCenterRegions.PrintLegacyRegions())
            {
                Console.WriteLine(s);
            }
        }
    }
}
