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
using ByteDBServer.Core.Misc.BDB;

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
                            if (client.InTransaction)
                                client.TransactionQueries.Add(query);
                            else
                                await ExecuteInsertInto(query);
                            break;
                        case "FETCH FROM":
                            if (client.InTransaction)
                                client.TransactionQueries.Add(query);
                            else
                                await ExecuteFetchFrom(client, query);
                            break;
                        case "DELETE FROM":
                            if (client.InTransaction)
                                client.TransactionQueries.Add(query);
                            else
                                await ExecuteDeleteFrom(query);
                            break;
                        case "UPDATE IN":
                            if (client.InTransaction)
                                client.TransactionQueries.Add(query);
                            else
                                await ExecuteUpdateIn(query);
                            break;

                        case "CREATE TABLE":
                            if (client.InTransaction)
                                client.TransactionQueries.Add(query);
                            else
                                await ExecuteCreateTable(query);
                            break;

                        case "BEGIN TRANSACTION":
                            if (ByteDBServerInstance.CheckCapability(ServerCapabilities.SUPPORTS_TRANSACTIONS, client.RequestedCapabilitiesInt) && !client.InTransaction)
                                client.InTransaction = true;
                            else
                                return false;
                            break;
                        case "COMMIT":
                            if (client.InTransaction)
                            {
                                foreach (ByteDBQuery transactionQuery in client.TransactionQueries)
                                {
                                    ByteDBServerListener.QueryPool.EnqueueTask(ByteDBTasks.ExecuteQuery(client, transactionQuery, false));
                                }

                                client.InTransaction = false;
                                client.TransactionQueries.Clear();
                            }
                            else
                                return false;
                            break;
                        case "ROLLBACK":
                            if (client.InTransaction)
                            {
                                client.TransactionQueries.Clear();
                                client.InTransaction = false;
                            }
                            else
                                return false;
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
        public static async Task ExecuteInsertInto(ByteDBQuery query)
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
                throw;
            }
        }

        /// <summary>
        /// Inserts columns and values into the <paramref name="table"/>.
        /// </summary>
        /// <param name="table">Table to which insert.</param>
        /// <param name="columns">Columns of <paramref name="table"/> to which values should be assigned.</param>
        /// <param name="values">Values of columns.</param>
        /// <param name="conditions">Conditions that table have to meet.</param>
        public static async Task InsertInto(ByteDBTable table, ByteDBArgumentCollection columns, ByteDBArgumentCollection values, List<ByteDBQueryFunction> conditions)
        {
            if (table.ColumnCount < columns.Count || table.ColumnCount < values.Count || columns.Count != values.Count)
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
                throw;
            }
        }

        /// <summary>
        /// Reads values from the <paramref name="table"/>.
        /// </summary>
        /// <param name="table">Table from which read.</param>
        /// <param name="columns">Columns of <paramref name="table"/> from which values should be read.</param>
        /// <param name="conditions">Conditions that table have to meet.</param>
        /// <param name="entryConditions">Conditions that entry have to meet.</param>
        public static async Task<List<ByteDBArgumentCollection>> FetchFrom(ByteDBTable table, ByteDBArgumentCollection columns, List<ByteDBQueryFunction> conditions, List<ByteDBQueryFunction> entryConditions)
        {
            if (table.ColumnCount < columns.Count)
                throw new Exception();

            return await table.GetRowsAsync(columns, conditions, entryConditions);
        }

        
        /// <summary>
        /// Executes fetch with arguments from query.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        public static async Task ExecuteDeleteFrom(ByteDBQuery query)
        {
            try
            {
                string tableName = query.Values[0];
                string tablePath = Path.Combine(ByteDBServerInstance.TablesPath, tableName + ByteDBServerInstance.TablesExtension);
                ByteDBTable table = ByteDBServerInstance.Tables[tablePath];

                List<ByteDBQueryFunction> ifConds = GetConditions(query, "IF CONTAINS");
                List<ByteDBQueryFunction> whereConds = GetConditions(query, "WHERE VALUES");

                await DeleteFrom(table, ifConds, whereConds);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes entry/entries from query based on conditions.
        /// </summary>
        /// <param name="table">Table on which execute query.</param>
        /// <param name="conditions">Conditions that table have to meet.</param>
        /// <param name="entryConditions">Conditions that entry have to meet.</param>
        public static async Task DeleteFrom(ByteDBTable table, List<ByteDBQueryFunction> conditions, List<ByteDBQueryFunction> entryConditions)
        {
            await table.RemoveRowsAsync(conditions, entryConditions);
        }


        /// <summary>
        /// Updated values in columns with arguments from query.
        /// </summary>
        /// <param name="query">>Query to execute.</param>
        public static async Task ExecuteUpdateIn(ByteDBQuery query)
        {
            try
            {
                string tableName = query.Values[0];
                string tablePath = Path.Combine(ByteDBServerInstance.TablesPath, tableName + ByteDBServerInstance.TablesExtension);
                ByteDBTable table = ByteDBServerInstance.Tables[tablePath];

                List<ByteDBQueryFunction> ifConds = GetConditions(query, "IF CONTAINS");
                List<ByteDBQueryFunction> whereConds = GetConditions(query, "WHERE VALUES");

                await UpdateIn(table, query.ArgumentCollections[0], query.ArgumentCollections[1], ifConds, whereConds);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Updates columns in <paramref name="table"/>.
        /// </summary>
        /// <param name="table">Table from which read.</param>
        /// <param name="columns">Columns of <paramref name="table"/> to which values should be assigned.</param>
        /// <param name="values">Values of columns.</param>
        /// <param name="conditions">Conditions that table have to meet.</param>
        /// <param name="entryConditions">Conditions that entry have to meet.</param>
        public static async Task UpdateIn(ByteDBTable table, ByteDBArgumentCollection columns, ByteDBArgumentCollection values, List<ByteDBQueryFunction> conditions, List<ByteDBQueryFunction> entryConditions)
        {
            if (table.ColumnCount < columns.Count || table.ColumnCount < values.Count || columns.Count != values.Count)
                throw new Exception();

            await table.UpdateRowsAsync(columns, values, conditions, entryConditions);
        }


        /// <summary>
        /// Creates new table from query.
        /// </summary>
        /// <param name="query">>Query to execute.</param>
        public static async Task ExecuteCreateTable(ByteDBQuery query)
        {
            try
            {
                string tableName = query.Values[0];
                string tablePath = Path.Combine(ByteDBServerInstance.TablesPath, tableName + ByteDBServerInstance.TablesExtension);

                await CreateTable(tablePath, query.ArgumentCollections[0], query.ArgumentCollections[1]);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Creates a new table.
        /// </summary>
        /// <param name="tablePath">Full path to the table.</param>
        /// <param name="columns">Column names of the table.</param>
        /// <param name="columnTypes">Column types of the table.</param>
        public static async Task CreateTable(string tablePath, ByteDBArgumentCollection columns, ByteDBArgumentCollection columnTypes)
        {
            await Task.Run(() => ByteDBServerInstance.Tables.Add(tablePath, new ByteDBTable(BDBTable.Create(tablePath, columns.ToArray(), columnTypes.ToArray()), tablePath)));
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
