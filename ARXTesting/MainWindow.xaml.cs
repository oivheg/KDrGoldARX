using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;

namespace ARXTesting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GetXML();
            //SendDataAsync(LstUsers);
            CleanData(LstUsers);
            CreateImportXML(LstUsers, "test.xml");
        }

        private void CleanData(List<Users> lstUsers)
        {
            //            var replacements = new[]{
            //                 new{Find="PID:",Replace=""},
            //                 new{Find="ID:",Replace=""},
            //                 //new{Find="999",Replace="Word two"},
            //};
            foreach (Users user in lstUsers)
            {
                user.UserId = RemoveExtraText(user.UserId);
                //user.UserId = user.UserId.Replace("ID:", "");
            }
        }

        private string RemoveExtraText(string value)
        {
            var allowedChars = "01234567890";
            return new string(value.Where(c => allowedChars.Contains(c)).ToArray());
        }

        private static List<Users> LstUsers = new List<Users>();

        private static void CreateImportXML(List<Users> lstUsers, string filename)
        {
            XmlWriter xmlWriter = XmlWriter.Create(filename);

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

            //xmlWriter.WriteAttributeString("age", "42");

            //xmlWriter.WriteStartElement("user");
            //xmlWriter.WriteAttributeString("age", "39");
            //xmlWriter.WriteString("Jane Doe");

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        public static void GetXML()
        {
            // run http Request against ARX server with basic auth.
            string path = "C:\\Users\\oivhe\\OneDrive - KDR Stavanger AS\\ARX Integrasjon\\KDRIMPORT\\allusers.xml";
            XmlDocument xml = new XmlDocument();

            XmlDeclaration xmldecl;
            xmldecl = xml.CreateXmlDeclaration("1.0", null, null);
            xmldecl.Encoding = "UTF-16";
            xmldecl.Standalone = "yes";

            // Add the new node to the document.
            XmlElement root = xml.DocumentElement;
            xml.InsertBefore(xmldecl, root);

            //xml.Load("C:\\Users\\oivhe\\OneDrive - KDR Stavanger AS\\ARX Integrasjon\\KDRIMPORT\\enkel bruker.xml");
            xml.Load(path);

            XmlNodeList nodePersons = xml.SelectNodes("//person");
            XmlNodeList nodecards = xml.SelectNodes("//card");
            foreach (XmlNode node in nodePersons)
            {
                Users User = new Users();
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    switch (node.ChildNodes[i].Name)
                    {
                        case "id":
                            User.UserId = node.ChildNodes[i].InnerText;

                            break;

                        case "first_name":
                            User.FirstName = node.ChildNodes[i].InnerText;

                            break;

                        case "last_name":
                            User.LastName = node.ChildNodes[i].InnerText;

                            break;

                        case "extra_fields":
                            /*XmlNodeList extrafieldlst = node.ChildNodes.Count()*/
                            ; ;
                            foreach (XmlNode field in node.ChildNodes[i].ChildNodes)
                            {
                                if (field.FirstChild.InnerText.Equals("Gruppe"))
                                {
                                    User.Company = field.SelectSingleNode("value").InnerText;
                                }
                            }

                            break;
                    }
                }
                LstUsers.Add(User);
            }

            foreach (XmlNode cardnode in nodecards)
            {
                for (int i = 0; i < cardnode.ChildNodes.Count; i++)
                {
                    switch (cardnode.ChildNodes[i].Name)
                    {
                        case "person_id":
                            var pers_id = cardnode.ChildNodes[i].InnerText;

                            foreach (Users user in LstUsers)
                            {
                                if (user.UserId.Equals(pers_id))
                                {
                                    var tmpcard = cardnode.SelectSingleNode("number").InnerText;
                                    user.CardList.Add(tmpcard);
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        private static String Base_URL = "http://91.189.171.231/restbusserv/api/UserAPI/";
        private static HttpResponseMessage Response { get; set; }

        public static async void SendDataAsync(Object Users)
        {
            var client = new HttpClient();
            string json = JsonConvert.SerializeObject(Users);
            var content = new StringContent(json, Encoding.Unicode, "application/json");
            var request = new HttpRequestMessage();
            Response = await client.PostAsync(Base_URL, content).ConfigureAwait(false);
        }
    }
}