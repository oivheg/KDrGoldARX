using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace KDRGoldConoslenoCore
{
    internal static class ImportFactory
    {
        private static List<Users> LstUsers = new List<Users>();

        public static List<Users> CleanData(List<Users> lstusers)
        {
            LstUsers.Clear();

            foreach (Users user in lstusers)
            {
                if (user.CardList.Count > 0)
                {
                    user.UserId = RemoveExtraText(user.UserId);
                    LstUsers.Add(user);
                }
            }

            return LstUsers;
        }

        public static void GenerateXML(List<Users> lstusers)
        {
            //SerializeObject(lstusers, "CustomerImport.xml");
            string filename = "CustImport" + DateTime.Now.ToFileTime();
            CreateImportXML(lstusers, filename);
        }

        internal static void Archive()
        {
            File.Move(XmlPath + "\\" + XmlFileName, XmlArchivePath + "\\" + XmlFileName); // Try to move
        }

        private static string RemoveExtraText(string value)
        {
            var allowedChars = "01234567890";
            return new string(value.Where(c => allowedChars.Contains(c)).ToArray());
        }

        private static string XmlPath = @"C:\\ARXGOld\XMLFIles";
        private static string XmlFileName;
        private static string XmlArchivePath = @"C:\\ARXGOld\XMLFIles\ARCHIVE";

        private static void CreateImportXML(List<Users> lstUsers, string filename)
        {
            Directory.CreateDirectory(XmlPath);
            Directory.CreateDirectory(XmlArchivePath);
            XmlWriter xmlWriter = XmlWriter.Create(XmlPath + "\\" + filename);
            XmlFileName = filename;
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("Persons");
            foreach (Users user in lstUsers)
            {
                foreach (string card in user.CardList)
                {
                    xmlWriter.WriteStartElement("Person");
                    //EMPID
                    xmlWriter.WriteStartElement("EmpID");
                    xmlWriter.WriteString(user.UserId);
                    xmlWriter.WriteEndElement();
                    //FNAME
                    xmlWriter.WriteStartElement("FName");
                    xmlWriter.WriteString(user.FirstName);
                    xmlWriter.WriteEndElement();
                    //LNAME
                    xmlWriter.WriteStartElement("LName");
                    xmlWriter.WriteString(user.LastName);
                    xmlWriter.WriteEndElement();
                    //Card
                    xmlWriter.WriteStartElement("Card");
                    xmlWriter.WriteString(card);
                    xmlWriter.WriteEndElement();
                    //Company
                    xmlWriter.WriteStartElement("Company");
                    xmlWriter.WriteString(user.Company);
                    xmlWriter.WriteEndElement();

                    xmlWriter.WriteEndElement();
                }
            }

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        //public static void SerializeObject(this List<Users> list, string fileName)
        //{
        //    var serializer = new XmlSerializer(typeof(List<Users>));
        //    using (var stream = File.OpenWrite(fileName))
        //    {
        //        serializer.Serialize(stream, list);
        //    }
        //}

        public static void ImporToKDRGold()
        {
            //string sqlConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=hegglandtest;Data Source=HEGGLAND\HEGGLAND";
            string sqlConnectionString = @"Server=HEGGLAND\HEGGLAND;Database=HegglandTEst;Integrated Security=True;Trusted_Connection=True;MultipleActiveResultSets=true";

            string script = File.ReadAllText(@"C:\Users\oivhe\OneDrive - KDR Stavanger AS\ARX Integrasjon\KDR_Import_KunderOgKortARX_ BETTER.sql");

            SqlConnection conn = new SqlConnection(sqlConnectionString);

            Server server = new Server(new ServerConnection(conn));

            server.ConnectionContext.ExecuteNonQuery(script);
        }
    }
}