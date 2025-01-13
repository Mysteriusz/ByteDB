using System.Collections.Generic;

#nullable enable
namespace ByteDBServer.Core.Server.Networking
{
    internal class ByteDBQuery
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        /// <summary>
        /// Query assigned <see cref="ByteDBServerInstance.QueryKeywords"/> as strings.
        /// </summary>
        public List<string>? Keywords { get; }

        /// <summary>
        /// Query assigned <see cref="ByteDBServerInstance.TableValueTypes"/> as strings.
        /// </summary>
        public List<string>? Values { get; }

        /// <summary>
        /// Query assigned argument collections.
        /// </summary>
        public ByteDBValueCollection[]? Arguments { get; }

        //
        // ----------------------------- CONSTRUCTORS ----------------------------- 
        //

        public ByteDBQuery(List<string>? keywords, List<string>? values, ByteDBValueCollection[]? arguments)
        {
            Values = values;
            Keywords = keywords;
            Arguments = arguments; 
        }
    }
}
