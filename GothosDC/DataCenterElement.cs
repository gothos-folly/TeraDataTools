using System;
using System.Collections.Generic;
using System.Globalization;
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

        public DataCenterValue? Attribute(string name)
        {
            return Attributes.Where(x => x.Name == name).Cast<DataCenterValue?>().SingleOrDefault();
        }

        public T Attribute<T>(string name)
        {
            var dcValue = Attribute(name);

            if (dcValue == null)
            {
                try
                {
                    return (T)(object)null;
                }
                catch (InvalidCastException ex)
                {
                    throw new KeyNotFoundException(string.Format("Key '{0}' not found", name), ex);
                }
            }
            else
            {
                object value = dcValue.Value.Value;
                try
                {
                    return (T)value;
                }
                catch (InvalidCastException ex)
                {
                    throw new InvalidCastException(string.Format("Could not cast {0}[\"{1}\"] = {2}  to '{3}'", Name, name, dcValue.Value.ValueToPrintString(CultureInfo.InvariantCulture), typeof(T).Name), ex);
                }
            }
        }

        public object this[string name]
        {
            get { return Attribute(name); }
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
