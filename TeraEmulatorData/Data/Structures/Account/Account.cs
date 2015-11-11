using System.Collections.Generic;
using Data.Enums.Item;
using Data.Interfaces;
using Data.Structures.Player;
using Utils;

namespace Data.Structures.Account
{
    [ProtoBuf.ProtoContract]
    public class Account : TeraObject
    {
        public IConnection Connection;

        public bool IsOnline
        {
            get { return Connection != null; }
        }

        [ProtoBuf.ProtoMember(1)]
        public int AccountId;
        public string AccountName;
        public string AccountPassword;
        public string AccountEmail;
        public byte AccessLevel;
        public int Membership;
        public bool isGM;
        public int Coins;
        public long LastOnlineUtc;
        public byte[] UiSettings;

        [ProtoBuf.ProtoMember(3)]
        public List<Player.Player> Players = new List<Player.Player>();

        [ProtoBuf.ProtoMember(10)]
        public Storage AccountWarehouse = new Storage{StorageType = StorageType.AccountWarehouse};

        public DelayedAction ExitAction;

        public override void Release()
        {
            base.Release();

            for (int i = 0; i < Players.Count; i++)
                Players[i].Release();

            Players = null;
        }
    }
}