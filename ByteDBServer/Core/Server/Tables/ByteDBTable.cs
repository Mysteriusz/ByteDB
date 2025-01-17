using ByteDBServer.Core.Server.Networking.Querying.Models;
using ByteDBServer.Core.Server.Tables;
using System.Collections.Generic;
using ByteDBServer.Core.Misc;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Linq;
using System.Linq;
using System;

namespace ByteDBServer.Core.Server.Databases
{
    internal class ByteDBTable
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        /// <summary>
        /// Table name.
        /// </summary>
        public string TableName => Document.Root.Attribute("Name").Value;

        /// <summary>
        /// Table file`s path.
        /// </summary>
        public string TableFullPath { get; private set; }

        /// <summary>
        /// Table columns and it`s value type.
        /// </summary>
        public Dictionary<string, Type> Columns 
        {
            get
            {
                Dictionary<string, Type> columns = new Dictionary<string, Type>();

                var columnNames = Document.Root.Attribute("ColumnNames").Value.Split(',').ToList();
                var columnTypes = Document.Root.Attribute("ColumnTypes").Value.Split(',').Select(t => ByteDBServerInstance.TableValueTypes[t]).ToList();

                for (int i = 0; i < columnNames.Count; i++)
                    columns.Add(columnNames[i], columnTypes[i]);

                return columns;
            }
        }

        /// <summary>
        /// All table entries as <see cref="ByteDBTableEntry"/>. 
        /// </summary>
        public HashSet<ByteDBTableEntry> Entries { get; private set; } = new HashSet<ByteDBTableEntry>();

        /// <summary>
        /// Table document.
        /// </summary>        
        public XDocument Document { get; private set; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBTable() { }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        private static readonly SemaphoreSlim _fileLock = new SemaphoreSlim(1, 1); // Allows only 1 thread to access at a time

        /// <summary>
        /// Adds an entry to the table and saves it to file.
        /// </summary>
        /// <param name="columns">Columns to which values should be assigned.</param>
        /// <param name="values">Values that are assigned to columns.</param>
        /// <param name="conditions">Conditions that table have to meet.</param>
        public void AddRow(ByteDBArgumentCollection columns, ByteDBArgumentCollection values, List<ByteDBQueryFunction> conditions)
        {
            _fileLock.Wait();

            try
            {
                List<string> tableColumns = Columns.Keys.ToList();
                List<string> toAddValues = new List<string>(Enumerable.Repeat("", tableColumns.Count));

                if (!ConditionsMet(conditions))
                    throw new ByteDBQueryConditionException(ByteDBQueryConditionException.DefaultMessage);

                // Start interating and assigning values to columns that are referenced
                for (int rIndex = 0; rIndex < columns.Count; rIndex++)
                {
                    // Find the column index in the table columns
                    int columnIndex = tableColumns.IndexOf(columns[rIndex]);

                    // If the column exists in the table, add the corresponding value at the correct position
                    if (columnIndex >= 0)
                        toAddValues[columnIndex] = values[rIndex];
                }

                var newEntry = new ByteDBTableEntry(tableColumns.ToArray(), toAddValues.ToArray());

                // Add Entry
                Document.Root.Add(newEntry.Element);
                Entries.Add(newEntry);

                // Save the file
                Document.Save(TableFullPath);
            }
            catch
            {
                throw;
            }
            finally { _fileLock.Release(); }
        }

        /// <summary>
        /// Adds an entry to the table and saves it to file.
        /// </summary>
        /// <param name="columns">Columns to which values should be assigned.</param>
        /// <param name="values">Values that are assigned to columns.</param>
        /// <param name="conditions">Conditions that table have to meet.</param>
        public async Task AddRowAsync(ByteDBArgumentCollection columns, ByteDBArgumentCollection values, List<ByteDBQueryFunction> conditions)
        {
            await _fileLock.WaitAsync();

            try
            {
                List<string> tableColumns = Columns.Keys.ToList();
                List<string> toAddValues = new List<string>(Enumerable.Repeat("", tableColumns.Count));

                if (!ConditionsMet(conditions))
                    throw new ByteDBQueryConditionException(ByteDBQueryConditionException.DefaultMessage);

                // Start interating and assigning values to columns that are referenced
                for (int rIndex = 0; rIndex < columns.Count; rIndex++)
                {
                    // Find the column index in the table columns
                    int columnIndex = tableColumns.IndexOf(columns[rIndex]);

                    // If the column exists in the table, add the corresponding value at the correct position
                    if (columnIndex >= 0)
                        toAddValues[columnIndex] = values[rIndex];
                }

                var newEntry = new ByteDBTableEntry(tableColumns.ToArray(), toAddValues.ToArray());

                // Add Entry
                Document.Root.Add(newEntry.Element);
                Entries.Add(newEntry);

                // Save the file
                await Task.Run(() => Document.Save(TableFullPath));
            }
            catch
            {
                throw;
            }
            finally
            {
                _fileLock.Release();
            }
        }

        /// <summary>
        /// Returns all columns from all entries.
        /// </summary>
        /// <param name="columns">Columns from which to read values.</param>
        /// <param name="conditions">Conditions that table have to meet.</param>
        /// <param name="entryConditions">Conditions that entry have to meet.</param>
        /// <returns>List of entry values from columns.</returns>
        public List<ByteDBArgumentCollection> GetRows(ByteDBArgumentCollection columns, List<ByteDBQueryFunction> conditions, List<ByteDBQueryFunction> entryConditions)
        {
            _fileLock.Wait();

            try
            {
                List<ByteDBArgumentCollection> rows = new List<ByteDBArgumentCollection>();

                if (!ConditionsMet(conditions))
                    throw new ByteDBQueryConditionException(ByteDBQueryConditionException.DefaultMessage);

                foreach (var entry in Entries)
                {
                    ByteDBArgumentCollection row = new ByteDBArgumentCollection();

                    if (!ConditionsMet(entryConditions, entry))
                        continue;

                    // Assign values to columns if they exist in the entry
                    foreach (var column in columns)
                    {
                        if (entry.Columns.TryGetValue(column, out var value))
                            row.Add(value);
                    }

                    rows.Add(row);
                }

                return rows;
            }
            catch
            {
                throw;
            }
            finally { _fileLock.Release(); }
        }

        /// <summary>
        /// Returns all columns from all entries.
        /// </summary>
        /// <param name="columns">Columns from which to read values.</param>
        /// <param name="conditions">Conditions that table have to meet.</param>
        /// <param name="entryConditions">Conditions that entry have to meet.</param>
        /// <returns>List of entry values from columns.</returns>
        public async Task<List<ByteDBArgumentCollection>> GetRowsAsync(ByteDBArgumentCollection columns, List<ByteDBQueryFunction> conditions, List<ByteDBQueryFunction> entryConditions)
        {
            await _fileLock.WaitAsync();

            try
            {
                List<ByteDBArgumentCollection> rows = new List<ByteDBArgumentCollection>();

                if (!ConditionsMet(conditions))
                    throw new ByteDBQueryConditionException(ByteDBQueryConditionException.DefaultMessage);

                foreach (var entry in Entries)
                {
                    if (!ConditionsMet(entryConditions, entry))
                        continue;

                    ByteDBArgumentCollection row = new ByteDBArgumentCollection();
                    
                    // Assign values to columns if they exist in the entry
                    foreach (var column in columns)
                        if (entry.Columns.TryGetValue(column, out var value))
                            row.Add(value);

                    rows.Add(row);
                }

                return rows;
            }
            catch
            {
                throw;
            }
            finally { _fileLock.Release(); }
        }

        /// <summary>
        /// Removes all matching rows.
        /// </summary>
        /// <param name="conditions">Conditions that table have to meet.</param>
        /// <param name="entryConditions">Conditions that entry have to meet.</param>
        public void RemoveRows(List<ByteDBQueryFunction> conditions, List<ByteDBQueryFunction> entryConditions)
        {
            _fileLock.Wait();

            try
            {
                if (!ConditionsMet(conditions))
                    throw new ByteDBQueryConditionException(ByteDBQueryConditionException.DefaultMessage);


                // Create a temporary list to hold entries to remove
                List<ByteDBTableEntry> entriesToRemove = new List<ByteDBTableEntry>();

                // Start iterating and add entries to remove
                foreach (var entry in Entries)
                {
                    if (!ConditionsMet(entryConditions, entry))
                        continue;

                    entriesToRemove.Add(entry);
                }

                // Remove the entries from the original list
                foreach (var entry in entriesToRemove)
                {
                    entry.Element.Remove();
                    Entries.Remove(entry);
                }

                // Save the file
                Document.Save(TableFullPath);
            }
            catch
            {
                throw;
            }
            finally { _fileLock.Release(); }
        }

        /// <summary>
        /// Removes all matching rows.
        /// </summary>
        /// <param name="conditions">Conditions that table have to meet.</param>
        /// <param name="entryConditions">Conditions that entry have to meet.</param>
        public async Task RemoveRowsAsync(List<ByteDBQueryFunction> conditions, List<ByteDBQueryFunction> entryConditions)
        {
            await _fileLock.WaitAsync();

            try
            {
                if (!ConditionsMet(conditions))
                    throw new ByteDBQueryConditionException(ByteDBQueryConditionException.DefaultMessage);

                // Create a temporary list to hold entries to remove
                List<ByteDBTableEntry> entriesToRemove = new List<ByteDBTableEntry>();

                // Start iterating and add entries to remove
                foreach (var entry in Entries)
                {
                    if (!ConditionsMet(entryConditions, entry))
                        continue;

                    entriesToRemove.Add(entry);
                }

                // Remove the entries from the original list
                foreach (var entry in entriesToRemove)
                {
                    entry.Element.Remove();
                    Entries.Remove(entry);
                }

                // Save the file
                await Task.Run(() => Document.Save(TableFullPath));
            }
            catch
            {
                throw;
            }
            finally { _fileLock.Release(); }
        }

        /// <summary>
        /// Check if all conditions in a list are met.
        /// </summary>
        /// <param name="conditions">Conditions to check.</param>
        /// <returns>True if all conditions are met; if not False.</returns>
        public bool ConditionsMet(List<ByteDBQueryFunction> conditions)
        {
            bool met = false; 

            foreach (var condition in conditions)
            {
                switch (condition.Operator)
                {
                    case '=':
                        met = HasEntry(condition.Arg1, condition.Arg2);
                        break;
                }

                if (!met)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Check if all conditions for entry are met.
        /// </summary>
        /// <param name="conditions">Conditions to check.</param>
        /// <param name="entry">Entry to which check conditions.</param>
        /// <returns>True if all conditions are met for entry; if not False.</returns>
        public bool ConditionsMet(List<ByteDBQueryFunction> conditions, ByteDBTableEntry entry)
        {
            bool met = false;

            foreach (var condition in conditions)
            {
                switch (condition.Operator)
                {
                    case '=':
                        met = entry.Columns[condition.Arg1] == condition.Arg2;
                        break;
                }

                if (!met)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Check if column of name <paramref name="columnName"/> is present in table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <returns>True if column is present; if not False.</returns>
        public bool HasColumn(string columnName) => Columns.ContainsKey(columnName);

        /// <summary>
        /// Check if any of entries <paramref name="columnName"/> has a value of <paramref name="value"/>.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="value">Column value.</param>
        /// <returns>True if column and value is present; if not False.</returns>
        public bool HasEntry(string columnName, string value)
        {
            if (!HasColumn(columnName))
                return false;

            foreach (var entry in Entries)
                if (entry.Columns.TryGetValue(columnName, out var columnVal) && columnVal == value)
                    return true;
            
            return false;
        }

        /// <summary>
        /// Loads table from path as <see cref="ByteDBTable"/>.
        /// </summary>
        /// <param name="tableFullPath">Path to table containing name.</param>
        /// <returns><see cref="ByteDBTable"/> object with assigned properties.</returns>
        public static ByteDBTable Load(string tableFullPath)
        {
            ByteDBTable table = new ByteDBTable();

            table.Document = XDocument.Load(tableFullPath);
            table.TableFullPath = tableFullPath;

            foreach (var entry in table.Document.Root.Elements())
                table.Entries.Add(new ByteDBTableEntry(entry));

            return table;
        }
    }
}
