using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System;
using ByteDBServer.Core.Server.Tables;
using System.Threading.Tasks;
using System.Threading;
using ByteDBServer.Core.Server.Networking.Querying.Models;
using ByteDBServer.Core.Misc.Logs;
using ByteDBServer.Core.Misc;

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
        public void AddRow(ByteDBArgumentCollection columns, ByteDBArgumentCollection values)
        {
            _fileLock.Wait();

            try
            {
                List<string> tableColumns = Columns.Keys.ToList();
                List<string> toAddValues = new List<string>(Enumerable.Repeat("", tableColumns.Count));

                // Start interating and assigning values to columns that are referenced
                for (int rIndex = 0; rIndex < columns.Count; rIndex++)
                {
                    // Find the column index in the table columns
                    int columnIndex = tableColumns.IndexOf(columns[rIndex]);

                    // If the column exists in the table, add the corresponding value at the correct position
                    if (columnIndex >= 0)
                        toAddValues[columnIndex] = values[rIndex];
                }

                // Add Entry
                Document.Root.Add(new ByteDBTableEntry(tableColumns.ToArray(), toAddValues.ToArray()).GetElement());

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
        public async Task AddRowAsync(ByteDBArgumentCollection columns, ByteDBArgumentCollection values, List<ByteDBQueryFunction> conditions)
        {
            await _fileLock.WaitAsync();

            try
            {
                List<string> tableColumns = Columns.Keys.ToList();
                List<string> toAddValues = new List<string>(Enumerable.Repeat("", tableColumns.Count));

                bool meetsConditions = false;

                foreach (var condition in conditions)
                {
                    switch (condition.Operator)
                    {
                        case '=':
                            meetsConditions = HasEntry(condition.Arg1, condition.Arg2);
                            break;
                    }

                    if (!meetsConditions)
                        new ByteDBQueryConditionException(ByteDBQueryConditionException.DefaultMessage + $", Condition failed: {condition.Arg1} {condition.Operator} {condition.Arg2}.");
                }

                // Start interating and assigning values to columns that are referenced
                for (int rIndex = 0; rIndex < columns.Count; rIndex++)
                {
                    // Find the column index in the table columns
                    int columnIndex = tableColumns.IndexOf(columns[rIndex]);

                    // If the column exists in the table, add the corresponding value at the correct position
                    if (columnIndex >= 0)
                        toAddValues[columnIndex] = values[rIndex];
                }

                // Add Entry
                Document.Root.Add(new ByteDBTableEntry(tableColumns.ToArray(), toAddValues.ToArray()).GetElement());

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

        public bool HasColumn(string columnName) => Columns.ContainsKey(columnName);
        public bool HasEntry(string columnName, string value)
        {
            if (!HasColumn(columnName))
                return false;

            foreach (var entry in Document.Root.Elements())
                if (entry.Attribute(columnName).Value == value)
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
            
            return table;
        }
    }
}
