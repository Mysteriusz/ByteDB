using ByteDBServer.Core.Server.Networking.Querying.Models;
using ByteDBServer.Core.Server.Networking.Models;
using ByteDBServer.Core.Server.Databases;
using ByteDBServer.Core.Server.Packets;
using ByteDBServer.Core.Misc.Logs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System;

namespace ByteDBServer.Core.Server.Networking.Querying
{
    /// <summary>
    /// Tasks that can be executed using <see cref="ByteDBQuery"/>.
    /// </summary>
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
                        case "FETCH FROM":
                            await ExecuteFetchFrom(client, query);
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
        /// <param name="client">Client to which info packets should be sent.</param>
        /// <param name="query">Query to execute.</param>
        public static async Task ExecuteInsertInto(ByteDBClient client, ByteDBQuery query)
        {
            try
            {
                string tableName = query.Values[0];
                string tablePath = Path.Combine(ByteDBServerInstance.TablesPath, tableName + ByteDBServerInstance.TablesExtension);
                ByteDBTable table = ByteDBServerInstance.Tables[tablePath];

                List<ByteDBQueryFunction> ifConds = GetConditions(query, "IF CONTAINS");

                await InsertInto(table, query.ArgumentCollections[0], query.ArgumentCollections[1], ifConds);
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
        public static async Task InsertInto(ByteDBTable table, ByteDBArgumentCollection columns, ByteDBArgumentCollection values, List<ByteDBQueryFunction> conditions)
        {
            if (table.Columns.Count < columns.Count || table.Columns.Count < values.Count || columns.Count != values.Count)
                throw new Exception();

            await table.AddRowAsync(columns, values, conditions);
        }

        /// <summary>
        /// Executes fetch with arguments from query.
        /// </summary>
        /// <param name="client">Client to which info packets should be sent.</param>
        /// <param name="query">Query to execute.</param>
        public static async Task ExecuteFetchFrom(ByteDBClient client, ByteDBQuery query)
        {
            try
            {
                string tableName = query.Values[0];
                string tablePath = Path.Combine(ByteDBServerInstance.TablesPath, tableName + ByteDBServerInstance.TablesExtension);
                ByteDBTable table = ByteDBServerInstance.Tables[tablePath];

                List<ByteDBQueryFunction> ifConds = GetConditions(query, "IF CONTAINS");
                List<ByteDBQueryFunction> whereConds = GetConditions(query, "WHERE VALUES");
                
                List<ByteDBArgumentCollection> rows = await FetchFrom(table, query.ArgumentCollections[0], ifConds, whereConds);

                string resultQuery = string.Join(";", rows);

                using (ByteDBQueryPacket queryResult = new ByteDBQueryPacket(resultQuery))
                    await queryResult.WriteAsync(client);
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
        /// Reads values from the <paramref name="table"/>.
        /// </summary>
        /// <param name="table">Table from which read.</param>
        /// <param name="columns">Columns of <paramref name="table"/> from which values should be read.</param>
        public static async Task<List<ByteDBArgumentCollection>> FetchFrom(ByteDBTable table, ByteDBArgumentCollection columns, List<ByteDBQueryFunction> conditions, List<ByteDBQueryFunction> entryConditions)
        {
            if (table.Columns.Count < columns.Count)
                throw new Exception();

            return await table.GetRowsAsync(columns, conditions, entryConditions);
        }

        private static List<ByteDBQueryFunction> GetConditions(ByteDBQuery query, string keyword)
        {
            if (query.Keywords.Contains(keyword))
            {
                return query.ArgumentCollections.FirstOrDefault(ac => ac.PreviousWord == keyword)?.Functions ?? new List<ByteDBQueryFunction>();
            }
            return new List<ByteDBQueryFunction>();
        }
    }
}
