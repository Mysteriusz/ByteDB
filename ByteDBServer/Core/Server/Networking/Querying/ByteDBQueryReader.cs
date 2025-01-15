using ByteDBServer.Core.Server.Networking.Querying.Models;
using ByteDBServer.Core.Misc.Logs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System;
using ByteDBServer.Core.Misc;

namespace ByteDBServer.Core.Server.Networking.Querying
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
                    string queryString = Encoding.ASCII.GetString(query).Trim();

                    int qIndex = 0;
                    char qChar = '0';

                    while (qChar != ByteDBServerInstance.QueryEndingChar) // QueryEndingChar
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
                        if (qChar == ByteDBServerInstance.QueryValueChar) // QueryValueChar
                        {
                            qChar = queryString[qIndex++];

                            while (qIndex < queryString.Length && qChar != ByteDBServerInstance.QueryValueChar) // QueryValueChar
                            {
                                StringBuilder arg = new StringBuilder();

                                while (qIndex < queryString.Length && qChar != ByteDBServerInstance.QueryValueChar) // QueryValueChar
                                {
                                    arg.Append(qChar);
                                    qChar = queryString[qIndex++];
                                }

                                queryValues.Add(arg.ToString().Trim());
                            }
                        }

                        #nullable enable
                        // Check if char is a ARGUMENT STARTER CHAR
                        if (qChar == ByteDBServerInstance.QueryStartArgumentChar) // QueryStartArgumentChar
                        {
                            qChar = queryString[qIndex++];

                            ByteDBArgumentCollection args = new ByteDBArgumentCollection();

                            while (qChar != ByteDBServerInstance.QueryEndArgumentChar)
                            {
                                StringBuilder str = new StringBuilder();
                                ByteDBQueryFunction? func = null;

                                while (qChar != ByteDBServerInstance.QueryArgumentDivider && qChar != ByteDBServerInstance.QueryEndArgumentChar)
                                {
                                    if (ByteDBServerInstance.QueryOperators.Contains(qChar))
                                    {
                                        func = new ByteDBQueryFunction
                                        {
                                            Operator = qChar,
                                            Arg1 = str.ToString().Trim()
                                        };
                                        str.Clear();
                                    }
                                    else
                                    {
                                        str.Append(qChar);
                                    }

                                    qChar = queryString[qIndex++];
                                }

                                if (func != null)
                                {
                                    func.Arg2 = str.ToString().Trim();
                                    args.Functions.Add(func);
                                }
                                else
                                {
                                    args.Add(str.ToString().Trim());
                                }

                                if (qChar == ByteDBServerInstance.QueryArgumentDivider)
                                {
                                    qChar = queryString[qIndex++];
                                }
                            }

                            queryArguments.Add(args);
                        }
                    }

                    return new ByteDBQuery(queryTokens.ToArray(), queryValues.ToArray(), queryArguments.ToArray());
                }
                catch (Exception ex)
                {
                    // Log the error or handle it
                    ByteDBServerLogger.WriteExceptionToFile(ex);
                    throw new ByteDBQueryException();
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
