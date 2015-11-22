using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GothosDC;
using GothosDC.LowLevel;

namespace DCTools.Structures
{
    public class DataCenter
    {
        private readonly GothosDC.DataCenter _dataCenter;

        public DataCenter(GothosDC.DataCenter dataCenter)
        {
            _dataCenter = dataCenter;
        }

        public IEnumerable<DataCenterElement> GetObjectsByName(string name)
        {
            return _dataCenter.AllElements.Where(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<DataCenterElement> GetMainObjectsByName(string name)
        {
            return _dataCenter.Root.ChildrenByName(name);
        }

        public Dictionary<string, object> GetValues(DataCenterElement dcObject)
        {
            return GetValues(dcObject, x => (object) x.ValueToString(CultureInfo.CurrentCulture));
        }

        public Dictionary<string, object> GetValuesAsObjects(DataCenterElement dcObject)
        {
            return GetValues(dcObject, x => x);
        }

        public Dictionary<string, object> GetValues(DataCenterElement dcObject, Func<DataCenterValue, object> projection)
        {
            Dictionary<string, object> values = dcObject.Attributes.ToDictionary(x => x.Name, projection);

            foreach (var child in dcObject.Children)
            {
                if (!values.ContainsKey(child.Name))
                    values.Add(child.Name, new List<Dictionary<string, object>>());

                var childredValues = (List<Dictionary<string, object>>)values[child.Name];
                childredValues.Add(GetValues(child, projection));
            }

            return values;
        }
    }
}
