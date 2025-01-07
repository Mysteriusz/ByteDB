using ByteDBServer.Core.Authentication.Models;
using ByteDBServer.Core.Misc;
using ByteDBServer.Core.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ByteDBServer.Core.Authentication
{
    internal static class ByteBDAuthenticator
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        // KEYS
        public static List<ByteDBKey> ActiveKeys { get; private set; }
        public const string KeysFilePath = "Core\\Authentication\\Keys.xml";

        // USERS
        public static List<ByteDBUser> ActiveUsers { get; private set; }
        public const string UsersFilePath = "Core\\Authentication\\Users.xml";

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        // ----------------------------- KEY METHODS ----------------------------- 
        public static void InitializeKeys()
        {
            XDocument doc = XDocument.Load(Path.Combine(AppContext.BaseDirectory + KeysFilePath));

            //
            // ----------------------------- READ ALL KEYS ----------------------------- 
            //

            foreach (var xmlkey in doc.Root.Elements())
            {
                TimeSpan timeSpan = DateTime.Now - DateTime.Parse(xmlkey.Attribute("ExpireDate").Value);
                int saltSize = int.Parse(xmlkey.Attribute("SaltSize").Value);
                byte[] salt = xmlkey.Attribute("Salt").Value.Split('-').Select(byte.Parse).ToArray();

                if (DateTime.Parse(xmlkey.Attribute("ExpireDate").Value) >= DateTime.Now)
                    continue;

                ActiveKeys.Add(new ByteDBKey(timeSpan, saltSize, salt));
            }
        }
        public static void WriteKey(ByteDBKey key)
        {
            string path = Path.Combine(AppContext.BaseDirectory + KeysFilePath);
            XDocument doc = XDocument.Load(path);

            //
            // ----------------------------- WRITE KEY ----------------------------- 
            //

            doc.Root.Add(NewKeyElement(key));
            doc.Save(path);
        }
        public static void RemoveKey(ByteDBKey key)
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, KeysFilePath);
            XDocument doc = XDocument.Load(filePath);

            var keyElement = doc.Root.Elements("Key").FirstOrDefault(e =>
                DateTime.Parse(e.Attribute("ExpireDate").Value) == key.ExpireDate &&
                int.Parse(e.Attribute("SaltSize").Value) == key.Salt.Length &&
                e.Attribute("Salt").Value == string.Join("-", key.Salt)
            );

            if (keyElement != null)
            {
                keyElement.Remove();
                ActiveKeys.Remove(key);
            }

            doc.Save(filePath);
        }

        // ----------------------------- USER METHODS ----------------------------- 
        public static void InitializeUsers()
        {
            XDocument doc = XDocument.Load(Path.Combine(AppContext.BaseDirectory + UsersFilePath));

            //
            // ----------------------------- READ ALL USERS ----------------------------- 
            //

            foreach (var xmluser in doc.Root.Elements())
                ActiveUsers.Add(new ByteDBUser(xmluser.Attribute("Name").ToString(), xmluser.Attribute("PasswordHash").ToString()));
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

        private static XElement NewKeyElement(ByteDBKey key)
        {
            return new XElement("Key",
                new XAttribute("ExpireDate", DateTime.Now.Subtract(key.ExpireDate)),
                new XAttribute("SaltSize", key.Salt.Length),
                new XAttribute("Salt", string.Join("-", key.Salt))
            );
        }
        private static XElement NewUserElement(ByteDBUser user)
        {
            return new XElement("User",
                new XAttribute("Name", user.Username),
                new XAttribute("PasswordHash", user.PasswordHash)
            );
        }

        public static ByteDBUser GetUser(string username)
        {
            return ActiveUsers.Find(x => x.Username == username);
        }
        public static byte[] Hash(byte[] bytes)
        {
            switch (ByteDBServerInstance.ServerAuthenticationType)
            {
                case ServerAuthenticationType.SHA224:
                    using (SecureHash.SHA2.SHA224 sha = SecureHash.SHA2.SHA224.Create())
                    return sha.ComputeHashBytes(bytes);

                case ServerAuthenticationType.SHA256:
                    using (SecureHash.SHA2.SHA256 sha = SecureHash.SHA2.SHA256.Create())
                    return sha.ComputeHashBytes(bytes);
                
                case ServerAuthenticationType.SHA384:
                    using (SecureHash.SHA2.SHA384 sha = SecureHash.SHA2.SHA384.Create())
                    return sha.ComputeHashBytes(bytes);
                
                case ServerAuthenticationType.SHA512:
                    using (SecureHash.SHA2.SHA512 sha = SecureHash.SHA2.SHA512.Create())
                    return sha.ComputeHashBytes(bytes);
                
                case ServerAuthenticationType.SHA512_224:
                    using (SecureHash.SHA2.SHA512_224 sha = SecureHash.SHA2.SHA512_224.Create())
                    return sha.ComputeHashBytes(bytes);
                
                case ServerAuthenticationType.SHA512_256:
                    using (SecureHash.SHA2.SHA512_256 sha = SecureHash.SHA2.SHA512_256.Create())
                    return sha.ComputeHashBytes(bytes);
            }

            throw new ByteDBInternalException("Unknown server authentication type.");
        }
    }
}
