using System;
using System.Collections.Generic;
using System.Linq;

namespace DCTools.Structures
{
    [ProtoBuf.ProtoContract]
    public class DataCenter
    {
        [ProtoBuf.ProtoMember(1)]
        public List<KeyValuePair<string, string>> Values { get; set; }

        [ProtoBuf.ProtoMember(2)]
        public List<DcObject> Objects { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public List<DcObject> MainObjects { get; set; }

        public IEnumerable<DcObject> GetObjectsByName(string name)
        {
            return Objects.Where(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<DcObject> GetMainObjectsByName(string name)
        {
            return MainObjects.Where(o => o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public Dictionary<string, object> GetValues(DcObject dcObject)
        {
            Dictionary<string, object> values = new Dictionary<string, object>();

            for (int i = 0; i < dcObject.ArgsCount; i++)
            {
                var val = Values[dcObject.ArgsShift+i];

                    values.Add(val.Key, val.Value);
            }

            for (int i = 0; i < dcObject.SubCount; i++)
            {
                DcObject val = Objects[dcObject.SubShift + i];

                if (!values.ContainsKey(val.Name))
                    values.Add(val.Name, new List<Dictionary<string, object>>());

                List<Dictionary<string, object>> vals = (List<Dictionary<string, object>>) values[val.Name];

                vals.Add(GetValues(val));
            }

            return values;
        }
    }
}