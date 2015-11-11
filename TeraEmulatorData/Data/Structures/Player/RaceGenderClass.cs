using System;
using Data.Enums;

namespace Data.Structures.Player
{
    [ProtoBuf.ProtoContract]
    public class RaceGenderClass
    {
        [ProtoBuf.ProtoMember(1)]
        public Gender Gender;

        [ProtoBuf.ProtoMember(2)]
        public Race Race;

        [ProtoBuf.ProtoMember(3)]
        public PlayerClass Class;

        private int _hash;

        public int Hash
        {
            get
            {
                if (_hash == 0)
                    _hash = 10101
                            + 200 * Race.GetHashCode()
                            + 100 * Gender.GetHashCode() +
                            Class.GetHashCode();

                return _hash;
            }
        }

        public RaceGenderClass(string race, string gender, string @class)
        {
            Race = (Race)Enum.Parse(typeof(Race), race, true);
            Gender = (Gender)Enum.Parse(typeof(Gender), gender);
            Class = PlayerClassHelper.Parse(@class);
        }

        public override int GetHashCode()
        {
            return Hash;
        }

        public override bool Equals(object obj)
        {
            var other = obj as RaceGenderClass;
            if (other == null)
                return false;
            return (Gender == other.Gender) && (Race == other.Race) && (Class == other.Class);
        }
    }
}
