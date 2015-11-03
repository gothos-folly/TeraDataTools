using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTools.Parsers.DC
{
    public class MyRaceGenderClass
    {
        public string Race { get; set; }
        public string Gender { get; set; }
        public string Class { get; set; }

        public static string NormalizeClass(string s)
        {
            s = s.ToLowerInvariant();
            s = s.Substring(0, 1).ToUpperInvariant() + s.Substring(1);
            if (s == "Elementalist")
                s = "Mystic";
            if (s == "Engineer")
                s = "Gunner";
            if (s == "Soulless")
                s = "Reaper";
            return s;
        }

        public MyRaceGenderClass(string race, string gender, string @class)
        {
            Race = race;
            Gender = gender;
            Class = NormalizeClass(@class);
        }

        private Tuple<string, string, string> Tuplify()
        {
            return Tuple.Create(Race, Gender, Class);
        }

        public override bool Equals(object obj)
        {
            return Tuplify().Equals(((MyRaceGenderClass)obj).Tuplify());
        }

        public override int GetHashCode()
        {
            return Tuplify().GetHashCode();
        }

        public override string ToString()
        {
            return Race + " " + Gender + " " + Class;
        }
    }
}
