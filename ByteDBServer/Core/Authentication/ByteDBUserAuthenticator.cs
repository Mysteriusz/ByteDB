using ByteDBServer.Core.Authentication.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace ByteDBServer.Core.Authentication
{
    internal static class ByteDBUserAuthenticator
    {
        //
        // ----------------------------- PROPERTIES ----------------------------- 
        //

        public static List<ByteDBUser> Users { get; private set; }

        public const string UserFilePath = "Core\\Authentication\\Users.xml";

        //
        // ----------------------------- METHODS ----------------------------- 
        //

        public static void InitializeUsers()
        {
            XDocument doc = XDocument.Load(Path.Combine(AppContext.BaseDirectory + UserFilePath));

            //
            // ----------------------------- READ ALL USERS ----------------------------- 
            //

            foreach (var xmluser in doc.Root.Elements())
                Users.Add(new ByteDBUser(xmluser.Attribute("Name").ToString(), xmluser.Attribute("PasswordHash").ToString()));
        }
    }
}
