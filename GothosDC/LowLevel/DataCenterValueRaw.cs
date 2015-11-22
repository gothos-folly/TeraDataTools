using System;
using System.Globalization;

namespace GothosDC.LowLevel
{
    public struct DataCenterValueRaw
    {
        public ushort Key { get; private set; }
        public TypeCode TypeCode { get; private set; }
        private readonly uint _value;

        public object Value
        {
            get
            {
                switch (TypeCode)
                {
                    case TypeCode.Int:
                        return ToInt();
                    case TypeCode.Float:
                        return ToFloat();
                    case TypeCode.Bool:
                        return ToBool();
                    default:
                        return ToSegmentAddress();
                }
            }
        }

        public int ToInt()
        {
            return unchecked((int)_value);
        }

        public unsafe float ToFloat()
        {
            var raw = _value;
            var f = *((float*)&raw);
            if (float.IsInfinity(f) || float.IsNaN(f))
                throw new FormatException("Expected only finite floats");
            return f;
        }

        public bool ToBool()
        {
            switch (_value)
            {
                case 0:
                    return false;
                case 1:
                    return true;
                default:
                    throw new FormatException("Invalid bool value");
            }
        }

        public SegmentAddress ToSegmentAddress()
        {
            return new SegmentAddress(unchecked((ushort)_value), (ushort)(_value >> 16));
        }

        public DataCenterValueRaw(TypeCode typeCode, ushort key, uint value)
            : this()
        {
            TypeCode = typeCode;
            Key = key;
            _value = value;
        }

        public override string ToString()
        {
            var typeCodeString = Enum.IsDefined(typeof(TypeCode), TypeCode) ? TypeCode.ToString() : ((ushort)TypeCode).ToString("x4");
            var valueString = ToString(Value, CultureInfo.InvariantCulture);
            return string.Format("{0:X4} {1} = {2}", Key, typeCodeString, valueString);
        }

        public static string ToString(object o, CultureInfo cultureInfo)
        {
            if (o is float)
            {
                var f = (float)o;
                var s = f.ToString("R", cultureInfo);

                long integerValue;
                bool isValidInteger = long.TryParse(s, out integerValue);

                if (isValidInteger)
                    s += cultureInfo.NumberFormat.NumberDecimalSeparator + "0";
                return s;
            }
            else
                return string.Format(cultureInfo, "{0}", o);
        }
    }
}
