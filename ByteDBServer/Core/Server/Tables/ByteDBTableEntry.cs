using System.Collections.Generic;
using System.Xml.Linq;

namespace ByteDBServer.Core.Server.Tables
{
    internal class ByteDBTableEntry
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //

        /// <summary>
        /// Entry name.
        /// </summary>
        public const string EntryName = "Entry";

        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        /// <summary>
        /// List of entry`s attributes.
        /// </summary>
        public List<XAttribute> Attributes { get; } = new List<XAttribute>();

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBTableEntry(string[] columns, string[] values)
        {
            for (int i = 0; i < columns.Length; i++)
                Attributes.Add(new XAttribute(columns[i], values[i]));
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        /// <summary>
        /// Converts table entry to <see cref="XElement"/>.
        /// </summary>
        /// <returns>Entry as <see cref="XElement"/>.</returns>
        public XElement GetElement()
        {
            XElement element = new XElement(EntryName);

            foreach (var attr in Attributes)
                element.SetAttributeValue(attr.Name, attr.Value);

            return element;
        }
    }
}
