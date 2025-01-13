using System.Collections.Generic;
using System.Xml.Linq;

namespace ByteDBServer.Core.Server.Tables
{
    internal class ByteDBTableEntry
    {
        public const string EntryName = "Entry";
        public List<XAttribute> Attributes { get; } = new List<XAttribute>();

        public ByteDBTableEntry(List<string> columns, List<string> values)
        {
            for (int i = 0; i < columns.Count; i++)
                Attributes.Add(new XAttribute(columns[i], values[i]));
        }

        public XElement GetElement()
        {
            XElement element = new XElement(EntryName);

            foreach (var attr in Attributes)
                element.SetAttributeValue(attr.Name, attr.Value);

            return element;
        }
    }
}
