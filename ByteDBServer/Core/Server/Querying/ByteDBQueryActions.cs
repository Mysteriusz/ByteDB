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
    }

    internal static class ByteDBQueryActions
    {
        public static void InsertInto(string tableName, ByteDBValueCollection columns, ByteDBValueCollection values)
        {
            try
            {
                string tableFilePath = Path.Combine(ByteDBServerInstance.TablesPath, tableName + "." + ByteDBServerInstance.TablesExtension);
                ByteDBTable table = ByteDBTable.Load(tableFilePath);
                
                table.AddRow(columns.Values, values.Values);
                
            }
            catch
            {
                throw;
            }
        }
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
    }
}
