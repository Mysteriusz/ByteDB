using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System;
using ByteDBServer.Core.Server.Tables;

namespace ByteDBServer.Core.Server.Databases
{
    internal class ByteDBTable
    {
        public string TableName { get; private set; }
        public string TableFullPath { get; private set; }

        public List<string> ColumnNames { get; private set; }
        public List<Type> ColumnTypes { get; private set; }

        public ByteDBTable() { }

        public void AddRow(List<string> columns, List<string> values)
        {
            XDocument tableDoc = XDocument.Load(TableFullPath);
            tableDoc.Root.Add(new ByteDBTableEntry(columns, values).GetElement());
            tableDoc.Save(TableFullPath);
        }

        public static ByteDBTable Load(string tableFullPath)
        {
            ByteDBTable table = new ByteDBTable();
            XDocument tableDoc = XDocument.Load(tableFullPath);

            table.TableFullPath = tableFullPath;
            table.TableName = tableDoc.Root.Attribute("Name").Value;
            table.ColumnNames = tableDoc.Root.Attribute("ColumnNames").Value.Split(',').ToList();
            table.ColumnTypes = tableDoc.Root.Attribute("ColumnTypes").Value.Split(',').Select(t => ByteDBServerInstance.TableValueTypes[t]).ToList();
            
            return table;
        }
    }
}
