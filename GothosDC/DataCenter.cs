using System;
using System.Collections.Generic;
using System.Linq;
using GothosDC.LowLevel;
using TypeCode = GothosDC.LowLevel.TypeCode;

namespace GothosDC
{
    public class DataCenter
    {
        private readonly Dictionary<SegmentAddress, string> _strings;
        private readonly string[] _names;
        private readonly SegmentAddressDictionary<DataCenterValueRaw> _values;
        private readonly SegmentAddressDictionary<DataCenterElementRaw> _elements;

        public int Revision { get; private set; }

        internal string GetArgumentString(ushort index)
        {
            return _names[index];
        }

        internal string GetString(SegmentAddress address)
        {
            return _strings[address];
        }

        internal DataCenterValueRaw GetValue(SegmentAddress address)
        {
            return _values[address];
        }

        internal DataCenterElementRaw GetElement(SegmentAddress address)
        {
            return _elements[address];
        }

        public IEnumerable<DataCenterElement> AllElements
        {
            get { return _elements.Select(x => new DataCenterElement(this, x.Value)); }
        }

        public DataCenterElement Root
        {
            get { return new DataCenterElement(this, _elements[new SegmentAddress()]); }
        }

        internal object ValueToObject(DataCenterValueRaw value)
        {
            switch (value.TypeCode)
            {
                case TypeCode.Int:
                    return value.ToInt();
                case TypeCode.Float:
                    return value.ToFloat();
                case TypeCode.Bool:
                    return value.ToBool();
                default:
                    var address = value.ToSegmentAddress();
                    return GetString(address);
            }
        }

        public DataCenter(DataCenterRaw lowLevel)
        {
            _strings = lowLevel.Strings.CollectionToDictionary(x => x.Key, x => x.Value);
            
            var nameMapping = new SegmentAddressDictionary<string>(lowLevel.Names, lowLevel.Names.Last().Key);
            _names = new[] { "__placeholder__" }.Concat(lowLevel.NameIds.Select(x => nameMapping[x])).ToArray();
            
            _values = new SegmentAddressDictionary<DataCenterValueRaw>(lowLevel.Values, lowLevel.Values.Last().Key);
            _elements = new SegmentAddressDictionary<DataCenterElementRaw>(lowLevel.Elements, lowLevel.Elements.Last().Key);
            Revision = lowLevel.Revision;

            var referencedObjects = _elements.Values.SelectMany(x => x.SubAddresses());
            if (!_elements.Keys.Except(referencedObjects).ToList().SequenceEqual(new[] { new SegmentAddress() }))
                throw new Exception("Only the first object should be unreferenced");
        }

        public static DataCenter Load(string filename)
        {
            var lowLevel = DataCenterRaw.Load(filename);
            return new DataCenter(lowLevel);
        }
    }
}
