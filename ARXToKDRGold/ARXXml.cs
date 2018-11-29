using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ARXToKDRGold
{
    public static class ARXXml
    {
        private static List<Users> LstUsers = new List<Users>();

        public static void GetXML()
        {
            LstUsers.Clear();
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
                User.CardList = new List<string>();
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    switch (node.ChildNodes[i].Name)
                    {
                        case "id":
                            User.UserId = node.ChildNodes[i].InnerText;

                            break;

                        case "extra_fields":
                            XmlNodeList extrafieldlst = xml.SelectNodes("//extra_field");
                            foreach (XmlNode field in extrafieldlst)
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

        public static void Sendxml()
        {
            SendDataAsync(LstUsers);
        }

        private static String Base_URL = "http://localhost:8080/api/customerimport";
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