using ByteDBServer.Core.Server.Networking.Querying.Models;
using System.Collections.Generic;
using ByteDBServer.Core.Misc.BDB;
using ByteDBServer.Core.Misc;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Linq;
using System.Linq;

namespace ByteDBServer.Core.Server.Databases
{
    internal class ByteDBTable
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        /// <summary>
        /// Table file`s path.
        /// </summary>
        public string TableFullPath { get; private set; }

        /// <summary>
        /// Table column count.
        /// </summary>
        public int ColumnCount => Table.Columns.Count;

        /// <summary>
        /// Table document.
        /// </summary>        
        public BDBTable Table { get; private set; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBTable() { }
        public ByteDBTable(BDBTable table, string path) { Table = table; TableFullPath = path; }

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
                List<string> toAddValues = new List<string>(Enumerable.Repeat("", Table.Columns.Count));

                if (!ConditionsMet(conditions))
                    throw new ByteDBQueryConditionException(ByteDBQueryConditionException.DefaultMessage);

                // Start interating and assigning values to columns that are referenced
                for (int rIndex = 0; rIndex < columns.Count; rIndex++)
                {
                    // Find the column index in the table columns
                    int columnIndex = Table.Columns.IndexOf(columns[rIndex]);

                    // If the column exists in the table, add the corresponding value at the correct position
                    if (columnIndex >= 0)
                        toAddValues[columnIndex] = values[rIndex];
                }

                // Add Entry
                Table.AddEntry(toAddValues.ToArray());

                // Save the file
                Table.Save();
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
                List<string> toAddValues = new List<string>(Enumerable.Repeat("", Table.Columns.Count));

                if (!ConditionsMet(conditions))
                    throw new ByteDBQueryConditionException(ByteDBQueryConditionException.DefaultMessage);

                // Start interating and assigning values to columns that are referenced
                for (int rIndex = 0; rIndex < columns.Count; rIndex++)
                {
                    // Find the column index in the table columns
                    int columnIndex = Table.Columns.IndexOf(columns[rIndex]);

                    // If the column exists in the table, add the corresponding value at the correct position
                    if (columnIndex >= 0)
                        toAddValues[columnIndex] = values[rIndex];
                }

                // Add Entry
                Table.AddEntry(toAddValues.ToArray());

                // Save the file
                await Task.Run(() => Table.Save());
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

                foreach (var entry in Table.Entries)
                {
                    ByteDBArgumentCollection row = new ByteDBArgumentCollection();

                    if (!ConditionsMet(entryConditions, entry))
                        continue;

                    foreach (var column in columns)
                    {
                        string value = entry.GetValue(column);

                        if (value != null)
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

                foreach (var entry in Table.Entries)
                {
                    if (!ConditionsMet(entryConditions, entry))
                        continue;

                    ByteDBArgumentCollection row = new ByteDBArgumentCollection();

                    foreach (var column in columns)
                    {
                        string value = entry.GetValue(column);

                        if (value != null)
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

                List<BDBEntry> toRemove = new List<BDBEntry>();

                // Start iterating and add entries to remove
                foreach (var entry in Table.Entries)
                {
                    if (!ConditionsMet(entryConditions, entry))
                        continue;
                    
                    toRemove.Add(entry);
                }

                foreach (var entry in toRemove)
                    Table.RemoveEntry(entry.Index);

                // Save the file
                Table.Save();
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

                List<BDBEntry> toRemove = new List<BDBEntry>();

                // Start iterating and add entries to remove
                foreach (var entry in Table.Entries)
                {
                    if (!ConditionsMet(entryConditions, entry))
                        continue;

                    toRemove.Add(entry);
                }

                foreach (var entry in toRemove)
                    Table.RemoveEntry(entry.Index);

                // Save the file
                await Task.Run(() => Table.Save());
            }
            catch
            {
                throw;
            }
            finally { _fileLock.Release(); }
        }

        /// <summary>
        /// Updates all columns of matching rows.
        /// </summary>
        /// <param name="columns">Columns which should be updated.</param>
        /// <param name="values">Values that are assigned to columns.</param>
        /// <param name="conditions">Conditions that table have to meet.</param>
        /// <param name="entryConditions">Conditions that entry have to meet.</param>
        public void UpdateRows(ByteDBArgumentCollection columns, ByteDBArgumentCollection values, List<ByteDBQueryFunction> conditions, List<ByteDBQueryFunction> entryConditions)
        {
            _fileLock.Wait();

            try
            {
                if (!ConditionsMet(conditions))
                    throw new ByteDBQueryConditionException(ByteDBQueryConditionException.DefaultMessage);

                foreach (var entry in Table.Entries)
                {
                    ByteDBArgumentCollection row = new ByteDBArgumentCollection();

                    if (!ConditionsMet(entryConditions, entry))
                        continue;

                    for (int i = 0; i < columns.Count; i++)
                        entry.UpdateValue(columns[i], values[i]);
                }

                // Save the file
                Table.Save();
            }
            catch
            {
                throw;
            }
            finally { _fileLock.Release(); }
        }

        /// <summary>
        /// Updates all columns of matching rows.
        /// </summary>
        /// <param name="columns">Columns which should be updated.</param>
        /// <param name="values">Values that are assigned to columns.</param>
        /// <param name="conditions">Conditions that table have to meet.</param>
        /// <param name="entryConditions">Conditions that entry have to meet.</param>
        public async Task UpdateRowsAsync(ByteDBArgumentCollection columns, ByteDBArgumentCollection values, List<ByteDBQueryFunction> conditions, List<ByteDBQueryFunction> entryConditions)
        {
            await _fileLock.WaitAsync();

            try
            {
                if (!ConditionsMet(conditions))
                    throw new ByteDBQueryConditionException(ByteDBQueryConditionException.DefaultMessage);

                foreach (var entry in Table.Entries)
                {
                    ByteDBArgumentCollection row = new ByteDBArgumentCollection();

                    if (!ConditionsMet(entryConditions, entry))
                        continue;

                    for (int i = 0; i < columns.Count; i++)
                        entry.UpdateValue(columns[i], values[i]);
                }

                // Save the file
                await Task.Run(() => Table.Save());
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
        public bool ConditionsMet(List<ByteDBQueryFunction> conditions, BDBEntry entry)
        {
            bool met = false;

            foreach (var condition in conditions)
            {
                switch (condition.Operator)
                {
                    case '=':
                        met = entry.GetValue(condition.Arg1) == condition.Arg2;
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
        public bool HasColumn(string columnName) => Table.Columns.Contains(columnName);

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

            foreach (var entry in Table.Entries)
                if (entry.GetValue(columnName) == value)
                    return true;
            
            return false;
        }
    }
}
