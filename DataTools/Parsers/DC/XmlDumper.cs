using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using DCTools;
using GothosDC;

namespace DataTools.Parsers.DC
{
    public class XmlDumper
    {
        public static void Parse()
        {
            var dc = DCT.GetDataCenter();
            foreach (var group in DCT.DataCenter.Root.Children.GroupBy(x => x.Name))
            {
                string dir2, format;
                if (group.Count() > 1)
                {
                    dir2 = "xml/" + group.Key + "/";
                    format = "{0}-{1}.xml";
                }
                else
                {
                    dir2 = "xml/";
                    format = "{0}.xml";
                }

                Directory.CreateDirectory(dir2);
                int i = 0;
                foreach (var mainObject in group)
                {
                    var element = ConvertToXElement(mainObject);
                    element.Save(dir2 + string.Format(format, mainObject.Name, i));
                    i++;
                }
            }
        }

        private static XElement ConvertToXElement(DataCenterElement obj)
        {
            var element = new XElement(obj.Name);
            foreach (var arg in obj.Attributes)
            {
                element.SetAttributeValue(arg.Name, arg.ValueToString(CultureInfo.InvariantCulture));
            }
            foreach (var child in obj.Children)
            {
                var childElement = ConvertToXElement(child);
                element.Add(childElement);
            }
            return element;
        }
    }
}
