using ByteDBServer.Core.Server.Networking;
using ByteDBServer.Core.Server.Databases;
using ByteDBServer.Core.Misc.Logs;
using System.Collections.Generic;
using System.IO;

namespace ByteDBServer.Core.Server
{
    internal class ByteDBValueCollection
    {
        public List<string> Values = new List<string>();
        public int Count => Values.Count;
    }

    internal static class ByteDBQueryActions
    {
        //
        // ----------------------------- METHODS ----------------------------- 
        //

        /// <summary>
        /// Executes <see cref="ByteDBQuery"/>.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <returns>True if query was executed correctly; False if there was an error.</returns>
        public static bool Execute(ByteDBQuery query)
        {
            try
            {
                if (query.Keywords != null)
                {
                    switch (query.Keywords[0])
                    {
                        case "INSERT INTO":
                            ByteDBServerLogger.WriteToFile($"INSERTING INTO {query.Values![0]}");
                            InsertInto(query.Values![0], query.Arguments[0], query.Arguments[1]);
                            break;
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Inserts columns and values into the <paramref name="tableName"/> table.
        /// </summary>
        /// <param name="tableName">Name of a table.</param>
        /// <param name="columns">Columns of <paramref name="tableName"/> to which values should be assigned.</param>
        /// <param name="values">Values of columns.</param>
        public static void InsertInto(string tableName, ByteDBValueCollection columns, ByteDBValueCollection values)
        {
            try
            {
                string tableFilePath = Path.Combine(ByteDBServerInstance.TablesPath, tableName + "." + ByteDBServerInstance.TablesExtension);
             
                ByteDBTable table = ByteDBTable.Load(tableFilePath);

                if (table.Columns.Count < columns.Count || table.Columns.Count < values.Count || columns.Count != values.Count)
                    throw new System.Exception();

                table.AddRow(columns.Values, values.Values);
            }
            catch
            {
                throw;
            }
        }
    }
}
