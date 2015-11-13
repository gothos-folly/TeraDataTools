using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GothosDC.LowLevel;
using TypeCode = GothosDC.LowLevel.TypeCode;

namespace GothosDC
{
    public class DataCenter
    {
        private readonly Dictionary<SegmentAddress, string> _strings;
        private readonly List<string> _names;
        private readonly Dictionary<SegmentAddress, DataCenterValueRaw> _values;
        private readonly Dictionary<SegmentAddress, DataCenterElementRaw> _elements;

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
            _strings = lowLevel.Strings.ToDictionary(x => x.Key, x => x.Value);
            _names = new[] { "__placeholder__" }.Concat(lowLevel.Names.Select(x => x.Value)).ToList();
            _values = lowLevel.Values.ToDictionary(x => x.Key, x => x.Value);
            _elements = lowLevel.Elements.ToDictionary(x => x.Key, x => x.Value);
            var referencesObjects = _elements.Values.SelectMany(x => x.SubAddresses());

            if (!_elements.Keys.Except(referencesObjects).ToList().SequenceEqual(new[] { new SegmentAddress() }))
                throw new Exception("Only the first object should be unreferenced");
        }

        public static DataCenter Load(string filename)
        {
            var lowLevel = DataCenterRaw.Load(filename);
            return new DataCenter(lowLevel);
        }
    }
}
