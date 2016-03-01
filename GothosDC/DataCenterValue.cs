using System;
using System.Collections.Generic;
using System.Globalization;

namespace GothosDC.LowLevel
{
    public struct DataCenterValue
    {
        public string Name { get; private set; }
        public TypeCode TypeCode { get; private set; }
        public object Value { get; private set; }

        public DataCenterValue(string name, TypeCode typeCode, object value)
            : this()
        {
            Name = name;
            TypeCode = typeCode;
            Value = value;
        }

        public string ValueToString(CultureInfo cultureInfo)
        {
            return DataCenterValueRaw.ToString(Value, cultureInfo);
        }

        public string ValueToPrintString(CultureInfo cultureInfo)
        {
            switch (TypeCode)
            {
                case TypeCode.Int:
                case TypeCode.Float:
                case TypeCode.Bool:
                    return ValueToString(cultureInfo);
                default:
                    return "\"" + ValueToString(cultureInfo) + "\"";
            }
        }

        public override string ToString()
        {
            var typeCodeString = Enum.IsDefined(typeof(TypeCode), TypeCode) ? TypeCode.ToString() : ((ushort)TypeCode).ToString("x4");
            var valueString = ValueToString(CultureInfo.InvariantCulture);
            return string.Format("{0:X4} {1} = {2}", Name, typeCodeString, valueString);
        }
    }
}
