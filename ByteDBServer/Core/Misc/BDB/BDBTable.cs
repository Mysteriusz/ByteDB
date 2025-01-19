using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System;
using ByteDBServer.Core.Server.Tables;
using ByteDBServer.Core.Server.Databases;

namespace ByteDBServer.Core.Misc.BDB
{
    public class BDBTable
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //

        public const char StartValueChar = '['; // Character marking the start of a value.
        public const char EndValueChar = ']';   // Character marking the end of a value.
        public const char ValueSeparatorChar = ':'; // Character separating values.

        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        /// <summary>
        /// A list containing the names of all columns in the table.
        /// </summary>
        public List<string> Columns { get; set; } = new List<string>();

        /// <summary>
        /// A list containing the data types of each column in the table.
        /// </summary>
        public List<string> ColumnsTypes { get; set; } = new List<string>();

        /// <summary>
        /// A list of table entries, each representing a row in the table.
        /// </summary>
        public List<BDBEntry> Entries { get; set; } = new List<BDBEntry>();

        /// <summary>
        /// The full path (including the file name) of the current table file.
        /// </summary>
        public string FullPath { get; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public BDBTable() { }
        public BDBTable(string path) { FullPath = path; }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        /// <summary>
        /// Adds a column to the table with a specified name and data type.
        /// </summary>
        /// <param name="columnName">The name of the new column.</param>
        /// <param name="columnType">The data type of the new column.</param>
        public void AddColumn(string columnName, string columnType)
        {
            Columns.Add(columnName);        // Add column name.
            ColumnsTypes.Add(columnType);   // Add column data type.

            // Add empty values for each existing entry in the table.
            foreach (BDBEntry entry in Entries)
                entry.Values.Add("");
        }

        /// <summary>
        /// Removes a column by its name from the table and all entries.
        /// </summary>
        /// <param name="columnName">The name of the column to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the column does not exist.</exception>
        public void RemoveColumn(string columnName)
        {
            Columns.Remove(columnName);      // Remove column name.
            ColumnsTypes.Remove(columnName); // Remove column data type.

            // Remove the column from all existing entries.
            foreach (BDBEntry entry in Entries)
                entry.Values.Remove(columnName);
        }

        /// <summary>
        /// Adds a new entry to the table with specified values.
        /// </summary>
        /// <param name="values">Values to be added to the new entry.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the number of values exceeds the column count.</exception>
        public void AddEntry(params string[] values)
        {
            if (values.Length > Columns.Count)
                throw new ArgumentOutOfRangeException("The number of values exceeds the number of columns.");

            int newEntryIndex = Entries.Count > 0 ? Entries.Last().Index + 1 : 0;
            int newEntryLineIndex = Entries.Count > 0 ? Entries.Last().LineIndex + 1 : 0;

            // Create new entry and add to the table.
            var newEntry = new BDBEntry(this, newEntryIndex, newEntryLineIndex, MergeValuesAndColumns(this, values));
            Entries.Add(newEntry);

            // Write the new entry to the file.
            using (StreamWriter writer = new StreamWriter(FullPath, append: true))
                writer.WriteLine(newEntry.ToString());
        }

        /// <summary>
        /// Removes an entry by its index from the table and the file.
        /// </summary>
        /// <param name="index">The index of the entry to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is out of range.</exception>
        public void RemoveEntry(int index)
        {
            if (index >= 0 && index < Entries.Count)
            {
                // Adjust indices for all entries after the removed entry.
                foreach (var entry in Entries.Where(e => e.LineIndex > Entries[index].LineIndex))
                {
                    entry.Index--;
                    entry.LineIndex--;
                }
            }
            else
                throw new ArgumentOutOfRangeException("The specified index is out of range.");

            // Remove entry from the list.
            Entries.RemoveAt(index);
        }

        /// <summary>
        /// Updates the values of an existing entry at a specific index.
        /// </summary>
        /// <param name="index">The index of the entry to update.</param>
        /// <param name="columns">The column names to update.</param>
        /// <param name="values">The new values for the specified columns.</param>
        public void UpdateEntry(int index, string[] columns, string[] values)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                if (ByteDBTableConstrains.Validate(values[i], ColumnsTypes[i]))
                    Entries[index].UpdateValue(columns[i], values[i]);
            }
        }

        /// <summary>
        /// Replaces an entry at a specific index with a new entry.
        /// </summary>
        /// <param name="entryIndex">The index of the entry to replace.</param>
        /// <param name="newEntry">The new entry to replace the existing one.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the new entry has more values than there are columns.</exception>
        public void ChangeEntry(int entryIndex, BDBEntry newEntry)
        {
            if (newEntry.Values.Count > Columns.Count)
                throw new ArgumentOutOfRangeException("The number of values exceeds the column count.");

            // Ensure the new entry has empty values for any missing columns.
            for (int i = newEntry.Values.Count; i < Columns.Count; i++)
                newEntry.Values.Add("");

            // Replace the existing entry with the new one.
            try
            {
                Entries[entryIndex] = newEntry;
            }
            catch
            {
                throw new ArgumentOutOfRangeException("Entry index does not exist.");
            }
        }

        /// <summary>
        /// Saves the table to its file, overwriting the existing contents.
        /// </summary>
        public void Save()
        {
            var sb = new StringBuilder();

            // Write columns and their types to the file.
            sb.AppendLine($"<c>{string.Join(":", Columns)}");
            sb.AppendLine($"<t>{string.Join(":", ColumnsTypes)}");

            // Write all entries to the file.
            foreach (BDBEntry entry in Entries)
                sb.AppendLine(entry.ToString());

            // Overwrite the file with the updated table.
            File.WriteAllText(FullPath, sb.ToString());
        }

        /// <summary>
        /// Loads a table from a file.
        /// </summary>
        /// <exception cref="DirectoryNotFoundException">Thrown if the file or directory is not found.</exception>
        public static BDBTable Load(string path)
        {
            var table = new BDBTable(path);

            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                int entryIndex = 0;
                int lineIndex = -1;

                while ((line = reader.ReadLine()?.Trim()) != null)
                {
                    lineIndex++;

                    // Parse entry line
                    if (line[1] == 'e')
                    {
                        int index = 3;
                        string content = line.Substring(index).Trim();
                        table.Entries.Add(new BDBEntry(table, entryIndex, lineIndex, MergeValuesAndColumns(table, content.Split(ValueSeparatorChar))));
                        entryIndex++;
                        continue;
                    }

                    // Parse column names
                    if (line[1] == 'c')
                    {
                        int index = 3;
                        string content = line.Substring(index).Trim();
                        table.Columns = content.Split(ValueSeparatorChar).ToList();
                        continue;
                    }

                    // Parse column types
                    if (line[1] == 't')
                    {
                        int index = 3;
                        string content = line.Substring(index).Trim();
                        table.ColumnsTypes = content.Split(ValueSeparatorChar).ToList();
                        continue;
                    }
                }
            }

            return table;
        }

        /// <summary>
        /// Creates a table file.
        /// </summary>
        /// <param name="path">Full path to the table.</param>
        /// <param name="columns">Column names of the table.</param>
        /// <param name="columnTypes">Column types of the table.</param>
        /// <exception cref="Exception"></exception>
        public static BDBTable Create(string path, string[] columns, string[] columnTypes)
        {
            using (var fs = File.Create(path)) { }
            
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < columns.Length; i++)
                if (!ByteDBTableConstrains.TypeDefinitions.ContainsKey(columnTypes[i]))
                    throw new Exception();

            sb.AppendLine($"<c>{string.Join(":", columns)}");
            sb.AppendLine($"<t>{string.Join(":", columnTypes)}");


            File.WriteAllText(path, sb.ToString());

            return new BDBTable(path);
        }

        /// <summary>
        /// Merges column names and values into a list of key-value pairs.
        /// </summary>
        /// <param name="table">The table containing column names.</param>
        /// <param name="values">The values to be merged with the columns.</param>
        /// <returns>A list of key-value pairs representing columns and their corresponding values.</returns>
        private static string[] MergeValuesAndColumns(BDBTable table, params string[] values)
        {
            List<string> result = new List<string>();

            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (ByteDBTableConstrains.Validate(values[i], table.ColumnsTypes[i]))
                    result.Add(values[i]);
            }

            return result.ToArray();
        }
    }
}