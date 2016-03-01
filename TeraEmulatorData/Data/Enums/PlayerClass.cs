using System;

namespace Data.Enums
{
    public enum PlayerClass
    {
        Common = -1,

        Warrior = 0,
        Lancer = 1,
        Slayer = 2,
        Berserker = 3,
        Sorcerer = 4,
        Archer = 5,
        Priest = 6,
        Mystic = 7,
        Reaper = 8,
        Gunner = 9,
        Brawler = 10
    }

    public static class PlayerClassHelper
    {
        public static PlayerClass Parse(string s)
        {
            switch (s.ToLowerInvariant())
            {
                case "elementalist":
                    return PlayerClass.Mystic;
                case "soulless":
                    return PlayerClass.Reaper;
                case "engineer":
                    return PlayerClass.Gunner;
                case "fighter":
                    return PlayerClass.Brawler;
                default:
                    return (PlayerClass)Enum.Parse(typeof(PlayerClass), s);
            }
        }
    }
}