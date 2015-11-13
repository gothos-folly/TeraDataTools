using System;
using System.Collections.Generic;
using System.Linq;
using GothosDC.LowLevel;

namespace GothosDC
{
    public class DataCenterElement
    {
        private readonly DataCenter _dataCenter;
        private readonly DataCenterElementRaw _raw;

        public string Name
        {
            get { return _dataCenter.GetArgumentString(_raw.NameKey); }
        }

        public DataCenterElement(DataCenter dataCenter, DataCenterElementRaw raw)
        {
            _dataCenter = dataCenter;
            _raw = raw;
        }

        public IEnumerable<DataCenterValue> Attributes
        {
            get
            {
                foreach (var argAddress in _raw.ArgAddresses())
                {
                    var value = _dataCenter.GetValue(argAddress);
                    var name = _dataCenter.GetArgumentString(value.Key);
                    var valueObject = _dataCenter.ValueToObject(value);
                    yield return new DataCenterValue(name, value.TypeCode, valueObject);
                }
            }
        }

        public object Attribute(string name)
        {
            return Attributes.SingleOrDefault(x => x.Name == name);
        }

        public IEnumerable<DataCenterElement> Children
        {
            get
            {
                var rawObjects = _raw.SubAddresses().Select(_dataCenter.GetElement);
                return rawObjects.Select(x => new DataCenterElement(_dataCenter, x));
            }
        }

        public IEnumerable<DataCenterElement> ChildrenByName(string name)
        {
            return Children.Where(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public DataCenterElement ChildByName(string name)
        {
            return ChildrenByName(name).Single();
        }

        public override string ToString()
        {
            return string.Format("{0} ({1} args, {2} children)", Name, _raw.ArgsCount, _raw.SubCount);
        }
    }
}
