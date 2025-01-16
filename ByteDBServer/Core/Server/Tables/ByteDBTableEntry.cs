using System.Collections.Generic;
using System.Xml.Linq;
using System;

namespace ByteDBServer.Core.Server.Tables
{
    internal class ByteDBTableEntry : IDisposable
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
        public Dictionary<string, string> Columns { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Entry xElement.
        /// </summary>
        public XElement Element { get; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBTableEntry(XElement entryElement)
        {
            Element = entryElement;

            foreach (XAttribute attr in entryElement.Attributes())
                Columns.Add(attr.Name.LocalName, attr.Value);
        }
        public ByteDBTableEntry(string[] columns, string[] values)
        {
            Element = ToElement();

            for (int i = 0; i < columns.Length; i++)
                Columns.Add(columns[i], values[i]);
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        /// <summary>
        /// Converts table entry to <see cref="XElement"/>.
        /// </summary>
        /// <returns>Entry as <see cref="XElement"/>.</returns>
        public XElement ToElement()
        {
            XElement element = new XElement(EntryName);

            foreach (var attr in Columns)
                element.SetAttributeValue(attr.Key, attr.Value);

            return element;
        }

        //
        // ----------------------------- DISPOSAL ----------------------------- 
        //

        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed)
                return;

            Columns.Clear();

            _disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}
