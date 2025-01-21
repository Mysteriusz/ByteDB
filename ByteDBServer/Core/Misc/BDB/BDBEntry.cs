using System.Collections.Generic;
using System;

namespace ByteDBServer.Core.Misc.BDB
{
    public class BDBEntry : IDisposable
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        /// <summary>
        /// The parent table to which this entry belongs.
        /// </summary>
        public BDBTable Table { get; }

        /// <summary>
        /// A set containing the values associated with this entry.
        /// </summary>
        public List<string> Values { get; } = new List<string>();

        /// <summary>
        /// The unique index of the entry within its table.
        /// </summary>
        public int Index { get; set; }

        /// <summary>s
        /// The line index of the entry in the <see cref="Table"/> file.
        /// </summary>
        public int LineIndex { get; set; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public BDBEntry() { }
        public BDBEntry(BDBTable table, int index, int lineIndex, params string[] values) 
        {
            Index = index;
            LineIndex = lineIndex;
            Table = table;

            Values.AddRange(values);
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        /// <summary>
        /// Saves the entry to the table file.
        /// </summary>
        public void Save() => Table.ChangeEntry(Index, this);

        public string GetValue(string columnName)
        {
            for (int i = 0; i < Table.Columns.Count; i++)
                if (Table.Columns[i].Name == columnName)
                    return Values[i];

            return null;
        }
        public void UpdateValue(string columnName, string newValue)
        {
            for (int i = 0; i < Table.Columns.Count; i++)
                if (Table.Columns[i].Name == columnName)
                    Values[i] = newValue;
        }

        //
        // ----------------------------- OVERRIDES ----------------------------- 
        //

        public override string ToString()
        {
            return "<e>" + string.Join(":", Values);
        }

        //
        // ----------------------------- DISPOSAL ----------------------------- 
        //
        
        public void Dispose()
        {
            Values.Clear();
        }
    }
}
