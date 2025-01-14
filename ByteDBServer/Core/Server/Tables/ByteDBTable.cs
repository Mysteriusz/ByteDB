using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System;
using ByteDBServer.Core.Server.Tables;
using System.Threading.Tasks;

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
        public string TableName { get; private set; }

        /// <summary>
        /// Table file`s path.
        /// </summary>
        public string TableFullPath { get; private set; }

        /// <summary>
        /// Table columns and it`s value type.
        /// </summary>
        public Dictionary<string, Type> Columns { get; } = new Dictionary<string, Type>();

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBTable() { }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        /// <summary>
        /// Adds an entry to the table and saves it to file.
        /// </summary>
        /// <param name="columns">Columns to which values should be assigned.</param>
        /// <param name="values">Values that are assigned to columns.</param>
        public void AddRow(List<string> columns, List<string> values)
        {
            XDocument tableDoc = XDocument.Load(TableFullPath);

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
            tableDoc.Root.Add(new ByteDBTableEntry(tableColumns.ToArray(), toAddValues.ToArray()).GetElement());
            
            // Save the file
            tableDoc.Save(TableFullPath);
        }

        /// <summary>
        /// Adds an entry to the table and saves it to file.
        /// </summary>
        /// <param name="columns">Columns to which values should be assigned.</param>
        /// <param name="values">Values that are assigned to columns.</param>
        public async Task AddRowAsync(List<string> columns, List<string> values)
        {
            XDocument tableDoc = await Task.Run(() => XDocument.Load(TableFullPath));

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
            tableDoc.Root.Add(new ByteDBTableEntry(tableColumns.ToArray(), toAddValues.ToArray()).GetElement());

            // Save the file
            tableDoc.Save(TableFullPath);
        }

        /// <summary>
        /// Loads table from path as <see cref="ByteDBTable"/>.
        /// </summary>
        /// <param name="tableFullPath">Path to table containing name.</param>
        /// <returns><see cref="ByteDBTable"/> object with assigned properties.</returns>
        public static ByteDBTable Load(string tableFullPath)
        {
            ByteDBTable table = new ByteDBTable();
            XDocument tableDoc = XDocument.Load(tableFullPath);

            table.TableFullPath = tableFullPath;
            table.TableName = tableDoc.Root.Attribute("Name").Value;

            var columnNames = tableDoc.Root.Attribute("ColumnNames").Value.Split(',').ToList();
            var columnTypes = tableDoc.Root.Attribute("ColumnTypes").Value.Split(',').Select(t => ByteDBServerInstance.TableValueTypes[t]).ToList();

            for (int i = 0; i < columnNames.Count; i++)
                table.Columns.Add(columnNames[i], columnTypes[i]);
            
            return table;
        }
    }
}
