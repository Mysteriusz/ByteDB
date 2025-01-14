using System.Collections.Generic;
using System.Text;
using System;
using System.Threading.Tasks;
using ByteDBServer.Core.Server.Querying.Models;

namespace ByteDBServer.Core.Server.Networking
{
    internal class ByteDBQueryReader : IDisposable
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        private List<ByteDBArgumentCollection> queryArguments = new List<ByteDBArgumentCollection>();
        private List<string> queryTokens = new List<string>();
        private List<string> queryValues = new List<string>();

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        /// <summary>
        /// Tries to tokenize the query bytes.
        /// </summary>
        /// <param name="query">Bytes of a query.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ByteDBQuery> Read(byte[] query)
        {
            return await Task.Run(() =>
            {
                try
                {
                    string queryString = ByteDBServerInstance.ServerEncoding.GetString(query).Trim();

                    int qIndex = 0;
                    char qChar = '0';

                    while (qChar != ByteDBServerInstance.QueryEndingChar)
                    {
                        // Check for keyword
                        foreach (string word in ByteDBServerInstance.QueryKeywords)
                        {
                            if (queryString.Substring(qIndex).StartsWith(word))
                            {
                                queryTokens.Add(word);
                                qIndex += word.Length;
                                break;
                            }
                        }

                        qChar = queryString[qIndex++];

                        // Check if char is a VALUECHAR
                        if (qChar == ByteDBServerInstance.QueryValueChar)
                        {
                            qChar = queryString[qIndex++];

                            while (qIndex < queryString.Length && qChar != ByteDBServerInstance.QueryValueChar)
                            {
                                StringBuilder arg = new StringBuilder();

                                while (qIndex < queryString.Length && qChar != ByteDBServerInstance.QueryValueChar)
                                {
                                    arg.Append(qChar);
                                    qChar = queryString[qIndex++];
                                }

                                queryValues.Add(arg.ToString().Trim());
                            }
                        }

                        // Check if char is a ARGUMENT STARTER CHAR
                        if (qChar == ByteDBServerInstance.QueryStartArgumentChar)
                        {
                            qChar = queryString[qIndex++];

                            ByteDBArgumentCollection argCollection = new ByteDBArgumentCollection();

                            while (qChar != ByteDBServerInstance.QueryEndArgumentChar)
                            {
                                StringBuilder arg = new StringBuilder();

                                while (qChar != ByteDBServerInstance.QueryArgumentDivider && qChar != ByteDBServerInstance.QueryEndArgumentChar)
                                {
                                    arg.Append(qChar);
                                    qChar = queryString[qIndex++];
                                }

                                argCollection.Add(arg.ToString().Trim());

                                if (qChar == ByteDBServerInstance.QueryArgumentDivider)
                                    qChar = queryString[qIndex++];
                            }

                            queryArguments.Add(argCollection);

                            // Skip the EndArgumentChar
                            if (qIndex < queryString.Length)
                                qChar = queryString[qIndex++];
                        }
                    }

                    return new ByteDBQuery(queryTokens.ToArray(), queryValues.ToArray(), queryArguments.ToArray());
                }
                catch (Exception ex)
                {
                    // Log the error or handle it
                    throw new Exception("Error while reading the query", ex);
                }
            });
        }

        //
        // ----------------------------- DISPOSAL ----------------------------- 
        //

        public void Dispose()
        {
            queryArguments = null;
            queryTokens = null;
            queryValues = null;
        }
    }
}
