using System.Collections.Generic;
using System.Linq;

namespace ByteDBServer.Core.Misc.BDB
{
    /// <summary>
    /// Represents a column in the BDB system, including its properties and constraints.
    /// </summary>
    public class BDBColumn
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        /// <summary>
        /// Column Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Column Type.
        /// </summary>    
        public string Type { get; set; }

        /// <summary>
        /// Indicates if NULL values are allowed. Default is true.
        /// </summary>
        public bool IsNullable { get; private set; } = true;

        /// <summary>
        /// Determines if values in the column must be unique. Default is false.
        /// </summary>
        public bool IsUnique { get; private set; } = false;

        /// <summary>
        /// Specifies the default value for the column. Empty string by default.
        /// </summary>
        public string DefaultValue { get; private set; } = "";

        /// <summary>
        /// Generates a list of constraints (e.g., NOTNULL, UNIQUE) based on the column's properties.
        /// </summary>
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


        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public BDBColumn(string name, string type, string[] constraints)
        {
            Name = name;
            Type = type;

            ReadConstraints(constraints);
        }

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        /// <summary>
        /// Reads and sets constraints based on the provided array.
        /// </summary>
        /// <param name="constraints">Array of constraint strings (e.g., "NOTNULL", "UNIQUE").</param>
        private void ReadConstraints(string[] constraints)
        {
            if (constraints.Contains("NOTNULL"))
                IsNullable = false;
            if (constraints.Contains("UNIQUE"))
                IsUnique = true;
        }

        /// <summary>
        /// Updates the nullable status of the column and sets the default value if necessary.
        /// </summary>
        /// <param name="v">Specifies whether the column allows NULL values.</param>
        /// <param name="defaultValue">Default value to assign if the column does not allow NULL values.</param>
        public void ChangeIsNullable(bool v, string defaultValue)
        {
            if (!v && string.IsNullOrEmpty(defaultValue))
                return;

            IsNullable = v;
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Updates whether the column values must be unique.
        /// </summary>
        /// <param name="v">Specifies whether the column values should be unique.</param>
        public void ChangeIsUnique(bool v)
        {
            IsUnique = v;
        }
    }
}
