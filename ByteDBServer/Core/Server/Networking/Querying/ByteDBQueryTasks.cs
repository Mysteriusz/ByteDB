using ByteDBServer.Core.Server.Networking.Models;
using ByteDBServer.Core.Server.Querying.Models;
using ByteDBServer.Core.Server.Databases;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using ByteDBServer.Core.Server.Packets;
using ByteDBServer.Core.Misc.Logs;
using System;

namespace ByteDBServer.Core.Server
{
    internal static class ByteDBQueryTasks
    {
        //
        // ----------------------------- METHODS ----------------------------- 
        //

        /// <summary>
        /// Executes <see cref="ByteDBQuery"/>.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <returns>True if query was executed correctly; False if there was an error.</returns>
        public static async Task<bool> Execute(ByteDBClient client, ByteDBQuery query)
        {
            try
            {
                if (query.Keywords != null)
                {
                    switch (query.Keywords[0])
                    {
                        case "INSERT INTO":
                            await ExecuteInsertInto(client, query);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ByteDBServerLogger.WriteExceptionToFile(ex);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Executes insert with arguments from query.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <param name="client">Client to which info packets should be sent.</param>
        public static async Task ExecuteInsertInto(ByteDBClient client, ByteDBQuery query)
        {
            try
            {
                string tableFilePath = Path.Combine(ByteDBServerInstance.TablesPath, query.Values[0] + "." + ByteDBServerInstance.TablesExtension);
                ByteDBTable table = ByteDBTable.Load(tableFilePath);

                if (query.Keywords.Length == 1)
                    await InsertInto(table, [], []);
                else if (query.Keywords.Length == 2 && query.Keywords[1] == "WITH VALUES")
                    await InsertInto(table, query.ArgumentCollections[0], query.ArgumentCollections[1]);
                else 
                    throw new Exception();
            }
            catch
            {
                using (ByteDBErrorPacket err = new ByteDBErrorPacket("Query failed"))
                    await err.WriteAsync(client);

                throw;
            }

            using (ByteDBOkayPacket okay = new ByteDBOkayPacket("Query success"))
                await okay.WriteAsync(client);
        }

        /// <summary>
        /// Inserts columns and values into the <paramref name="table"/>.
        /// </summary>
        /// <param name="table">Table to which insert.</param>
        /// <param name="columns">Columns of <paramref name="table"/> to which values should be assigned.</param>
        /// <param name="values">Values of columns.</param>
        public static async Task InsertInto(ByteDBTable table, ByteDBArgumentCollection columns, ByteDBArgumentCollection values)
        {
            if (table.Columns.Count < columns.Count || table.Columns.Count < values.Count || columns.Count != values.Count)
                throw new Exception();

            await table.AddRowAsync(columns, values);
        }
    }
}
