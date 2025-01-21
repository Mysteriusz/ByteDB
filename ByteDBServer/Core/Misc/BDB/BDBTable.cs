using ByteDBServer.Core.Server.Tables;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System;

namespace ByteDBServer.Core.Misc.BDB
{
    public class BDBTable : IDisposable
    {
        //
        // ----------------------------- CONSTANTS ----------------------------- 
        //

        public const char StartValueChar = '['; // Character marking the start of a value.
        public const char EndValueChar = ']';   // Character marking the end of a value.
        public const char ValueSeparatorChar = ':'; // Character separating values.
        public const char ConstrainSeparatorChar = ','; // Character separating constraints.

        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        /// <summary>
        /// A list of all columns in the table.
        /// </summary>
        public List<BDBColumn> Columns { get; set; } = new List<BDBColumn>();

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
        public void AddColumn(string columnName, string columnType, string[] constraints)
        {
            if (!ByteDBTableConstraints.ValidateConstraints(constraints) || ByteDBTableConstraints.ValidateType(columnType))
                throw new ByteDBQueryException("INCORRECT CONSTRAINS OR TYPE");

            Columns.Add(new BDBColumn(columnName, columnType, constraints));

            // Add empty values for each existing entry in the table.
            foreach (BDBEntry entry in Entries)
                entry.Values.Add(Columns.Last().DefaultValue);
        }

        /// <summary>
        /// Removes a column by its name from the table and all entries.
        /// </summary>
        /// <param name="columnName">The name of the column to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the column does not exist.</exception>
        public void RemoveColumn(string columnName)
        {
            Columns.Remove(Columns.Find(c => c.Name == columnName));

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

            if (!ValidateValues(values))
                throw new Exception();

            // Create new entry and add to the table.
            var newEntry = new BDBEntry(this, newEntryIndex, newEntryLineIndex, values);
            Entries.Add(newEntry);
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
                if (!ValidateValue(columns[i], values[i]))
                    throw new Exception();
                
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
            sb.AppendLine($"<c>{string.Join($"{ValueSeparatorChar}", Columns.Select(c => c.Name))}");
            sb.AppendLine($"{string.Join($"{ValueSeparatorChar}", Columns.Select(c => c.Type))}");
            sb.AppendLine($"{string.Join($"{ValueSeparatorChar}", Columns.Select(c => string.Join($"{ConstrainSeparatorChar}", c.Constraints)))}");

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
                        string content = line.Trim().Substring(index);
                        table.Entries.Add(new BDBEntry(table, entryIndex, lineIndex, content.Split(ValueSeparatorChar)));
                        entryIndex++;
                        continue;
                    }

                    // Parse column names
                    if (line[1] == 'c')
                    {
                        int index = 3;
                        string columnsNames = line.Trim().Substring(index);
                        string columnsTypes = reader.ReadLine()?.Trim();
                        string columnsConstraints = reader.ReadLine()?.Trim();

                        string[] columnNamesArray = columnsNames.Split(ValueSeparatorChar);
                        string[] columnTypesArray = columnsTypes.Split(ValueSeparatorChar);
                        string[] columnConstraintsArray = columnsConstraints.Split(ValueSeparatorChar);

                        for (int i = 0; i < columnNamesArray.Length; i++)
                        {
                            string columnName = columnNamesArray[i].Trim();
                            string columnType = columnTypesArray[i].Trim();
                            string[] columnConstraints = columnConstraintsArray[i].Trim().Split(ConstrainSeparatorChar);

                            if (!ByteDBTableConstraints.ValidateConstraints(columnConstraints) || !ByteDBTableConstraints.ValidateType(columnType))
                                throw new ByteDBQueryException("INCORRECT CONSTRAINS OR TYPE");

                            table.Columns.Add(new BDBColumn(columnName, columnType, columnConstraints));
                        }

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
        /// <exception cref="ByteDBQueryException"></exception>
        public static BDBTable Create(string path, string[] columns, string[] columnTypes, string[][] columnConstraints)
        {
            if (columnConstraints.Length != columns.Length || columnTypes.Length != columns.Length)
                throw new ByteDBQueryException("INCORRECT ARGUMENT COUNT");

            if (!ByteDBTableConstraints.ValidateConstraints(columnConstraints) || !ByteDBTableConstraints.ValidateTypes(columnTypes))
                throw new ByteDBQueryException("INCORRECT CONSTRAINS OR TYPES");

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"<c>{string.Join($"{ValueSeparatorChar}", columns)}");
            sb.AppendLine($"{string.Join($"{ValueSeparatorChar}", columnTypes)}");
            sb.AppendLine($"{string.Join($"{ValueSeparatorChar}", columnConstraints.Select(c => string.Join($"{ConstrainSeparatorChar}", c)))}");

            using (var fs = File.Create(path)) { }
            File.WriteAllText(path, sb.ToString());

            return Load(path);
        }

        /// <summary>
        /// Validates values to current <see cref="Columns"/> list.
        /// </summary>
        /// <param name="values">Values to validate.</param>
        /// <returns>True if validation is correct; False if not.</returns>
        public bool ValidateValues(string[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (Columns[i].IsUnique && !IsUnique(values[i]))
                    return false;
                if (!Columns[i].IsNullable && IsNull(values[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Validates value to current the columnName column.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>True if validation is correct; False if not.</returns>
        public bool ValidateValue(string columnName, string value)
        {
            var column = Columns.Find(c => c.Name == columnName);

            if (column.IsUnique && !IsUnique(value))
                return false;
            if (!column.IsNullable && IsNull(value))
                return false;
        
            return true;
        }

        /// <summary>
        /// Checks if value is unique to table.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True if it`s unique; False if not.</returns>
        private bool IsUnique(string value)
        {
            foreach (var entry in Entries)
            {
                if (entry.Values.Contains(value))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if value is null or empty.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True if it`s unique; False if not.</returns>
        private bool IsNull(string value)
        {
            return string.IsNullOrEmpty(value);
        }

        //
        // ----------------------------- DISPOSAL ----------------------------- 
        //

        private protected bool _disposed = false;

        public void Dispose()
        {
            if (_disposed)
                return;

            foreach (var entry in Entries)
                entry?.Dispose();

            Columns.Clear();
            Entries.Clear();

            _disposed = true;
        }
    }
}