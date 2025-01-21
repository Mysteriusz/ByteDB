using System.Collections.Generic;
using System.Linq;

namespace ByteDBServer.Core.Misc.BDB
{
    public class BDBColumn
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public bool IsNullable { get; private set; } = true;
        public bool IsUnique { get; private set; } = false;

        public string DefaultValue { get; private set; } = "";

        public List<string> Constraints 
        {
            get
            {
                List<string> list = new List<string>();
                if (!IsNullable)
                    list.Add("NOTNULL");
                if (IsUnique)
                    list.Add("UNIQUE");

                return list;
            }
        }

        public BDBColumn(string name, string type, string[] constraints)
        {
            Name = name;
            Type = type;

            ReadConstraints(constraints);
        }

        private void ReadConstraints(string[] constraints)
        {
            if (constraints.Contains("NOTNULL"))
                IsNullable = false;
            if (constraints.Contains("UNIQUE"))
                IsUnique = true;
        }

        public void ChangeIsNullable(bool v, string defaultValue)
        {
            if (!v && string.IsNullOrEmpty(defaultValue))
                return;

            IsNullable = v;
            DefaultValue = defaultValue; 
        }
        public void ChangeIsUnique(bool v)
        {
            IsUnique = v;
        }
    }
}
