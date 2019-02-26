using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
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
            string filename = "CustImport";
            CreateImportXML(lstusers, filename);
        }

        internal static void Archive(bool sucsess)
        {
            if (sucsess)
            {
                File.Move(XmlPath + "\\" + XmlFileName + ".xml", XmlArchivePath + "\\" + XmlFileName + DateTime.Now.ToFileTime() + ".xml"); // Try to move
            }
            else
            {
            }
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
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.Unicode;
            XmlWriter xmlWriter = XmlWriter.Create(XmlPath + "\\" + filename + ".xml", settings);

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

        public static bool ImporToKDRGold()
        {
            try
            {
                //string sqlConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=hegglandtest;Data Source=HEGGLAND\HEGGLAND";
                string sqlConnectionString = Data.ServerString;
                //string sqlConnectionString = @"Server=HEGGLAND\HEGGLAND;Database=KDRGoldDemoKDRSNy;Integrated Security=True;Trusted_Connection=True;MultipleActiveResultSets=true";

                //string script = File.ReadAllText(@"C:\ArxImporter\KDR_Import_KunderOgKortARX_ BETTER.sql");
                XmlDocument xmlToSave = new XmlDocument();
                xmlToSave.Load("C:\\ARXGOld\\XMLFIles\\CustImport.xml");
                string xml = File.ReadAllText("C:\\ARXGOld\\XMLFIles\\CustImport.xml");
                //xml.Replace("<? xml version =\"1.0\" encoding=\"utf-8\"?>", "");
                SqlConnection conn = new SqlConnection(sqlConnectionString);

                //  using (var command = new SqlCommand("Import_ArxCustomers", conn)
                //  {
                //      CommandType = CommandType.StoredProcedure
                //  })

                //  {
                //      command.Parameters.Add(
                //new SqlParameter("@xml", SqlDbType.Xml)
                //{
                //    Value = new SqlXml(new XmlTextReader(xmlToSave.InnerXml
                //               , XmlNodeType.Document, null))
                //});
                //      conn.Open();
                //      command.ExecuteNonQuery();
                //  }

                using (SqlCommand cmd = new SqlCommand("Import_ArxCustomers"))
                {
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@XmlVal", xml);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                //Server server = new Server(new ServerConnection(conn));

                //server.ConnectionContext.ExecuteNonQuery(script);
                return true;
            }
            catch (Exception e)
            {
                string filePath = @"C:\Error.txt";

                Exception ex = e;

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("-----------------------------------------------------------------------------");
                    writer.WriteLine("Date : " + DateTime.Now.ToString());
                    writer.WriteLine();

                    while (ex != null)
                    {
                        writer.WriteLine(ex.GetType().FullName);
                        writer.WriteLine("Message : " + ex.Message);
                        writer.WriteLine("StackTrace : " + ex.StackTrace);

                        ex = ex.InnerException;
                    }
                }
            }
            return false;
        }
    }
}