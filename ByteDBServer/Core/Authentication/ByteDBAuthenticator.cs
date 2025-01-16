using ByteDBServer.Core.Authentication.Models;
using System.Security.Cryptography;
using System.Collections.Generic;
using ByteDBServer.Core.Server;
using ByteDBServer.Core.Misc;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System;

namespace ByteDBServer.Core.Authentication
{
    internal static class ByteDBAuthenticator
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        // USERS
        public static List<ByteDBUser> ActiveUsers { get; private set; } = new List<ByteDBUser>();
        public const string UsersFilePath = "Core\\Authentication\\Users.xml";

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        // ----------------------------- USER METHODS ----------------------------- 
        public static void InitializeUsers()
        {
            XDocument doc = XDocument.Load(Path.Combine(AppContext.BaseDirectory + UsersFilePath));

            //
            // ----------------------------- READ ALL USERS ----------------------------- 
            //

            foreach (var xmluser in doc.Root.Elements())
                ActiveUsers.Add(new ByteDBUser(xmluser.Attribute("Name").Value, xmluser.Attribute("PasswordHash").Value));
        }
        public static void WriteUser(ByteDBUser user)
        {
            string path = Path.Combine(AppContext.BaseDirectory + UsersFilePath);
            XDocument doc = XDocument.Load(path);

            //
            // ----------------------------- WRITE KEY ----------------------------- 
            //

            doc.Root.Add(NewUserElement(user));
            doc.Save(path);
        }
        public static void RemoveUser(ByteDBUser user)
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, UsersFilePath);
            XDocument doc = XDocument.Load(filePath);

            var userElement = doc.Root.Elements("User").FirstOrDefault(e =>
                e.Attribute("Name").Value == user.Username &&
                e.Attribute("PasswordHash").Value == user.PasswordHash
            );

            if (userElement != null)
            {
                userElement.Remove();
                ActiveUsers.Remove(user);
            }

            doc.Save(filePath);
        }
        public static XElement NewUserElement(ByteDBUser user)
        {
            return new XElement("User",
                new XAttribute("Name", user.Username),
                new XAttribute("PasswordHash", user.PasswordHash)
            );
        }
        
        #nullable enable
        public static ByteDBUser? GetUser(string username)
        {
            return ActiveUsers.Find(x => x.Username == username);
        }

        public static byte[] GenerateSalt(int saltSize)
        {
            byte[] buffer = new byte[saltSize];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                rng.GetBytes(buffer);

            return buffer;
        }
        public static byte[] Hash(byte[] bytes)
        {
            switch (ByteDBServerInstance.ServerAuthenticationType)
            {
                case ServerAuthenticationType.SHA_256:
                    using (SHA256 sha = SHA256.Create())
                    return sha.ComputeHash(bytes);
                
                case ServerAuthenticationType.SHA_384:
                    using (SHA384 sha = SHA384.Create())
                    return sha.ComputeHash(bytes);
                
                case ServerAuthenticationType.SHA_512:
                    using (SHA512 sha = SHA512.Create())
                    return sha.ComputeHash(bytes);
            }

            throw new ByteDBInternalException("Unknown server authentication type.");
        }

        public static HashSet<TEnum> ReadFlags<TEnum>(uint flagValue) where TEnum : Enum
        {
            HashSet<TEnum> result = new HashSet<TEnum>();

            foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
                if ((flagValue & Convert.ToUInt32(enumValue)) != 0)
                    result.Add(enumValue);

            return result;
        }
    }
}
