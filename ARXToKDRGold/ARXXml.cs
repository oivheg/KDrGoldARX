﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ARXToKDRGold
{
    public static class ARXXml
    {
        private static List<Users> LstUsers = new List<Users>();

        public static void GetDatafromARX()
        {
            //Certificates.Instance.GetCertificatesAutomatically();
            //ServicePointManager.Expect100Continue = true;

            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            var request = WebRequest.Create("http://localhost:5004/arx/export");
            //authInfo = "master" + ":" + "4bdk0jf2";
            //authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            request.Credentials = new NetworkCredential("master", "4bdk0jf2");
            request.PreAuthenticate = true;

            //like this:
            //request.Headers["Authorization"] = "Basic " + authInfo;

            var response = request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var responseText = streamReader.ReadToEnd();
            }
        }

        public static void GetXML()
        {
            // run http Request against ARX server with basic auth.
            GetDatafromARX();
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
            xml.LoadXml(GetXMLData());

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

        public static List<Users> GetList()
        {
            return LstUsers;
        }

        public static List<Users> CleanXML(List<Users> _lstUSers)
        {
            List<Users> TMPLSTUsers = new List<Users>();

            foreach (Users user in _lstUSers)
            {
                if (user.CardList.Count > 0)
                {
                    user.UserId = RemoveExtraText(user.UserId);
                    TMPLSTUsers.Add(user);
                }
            }
            LstUsers.Clear();
            return TMPLSTUsers;
        }

        private static string RemoveExtraText(string value)
        {
            var allowedChars = "01234567890";
            return new string(value.Where(c => allowedChars.Contains(c)).ToArray());
        }

        public static void Sendxml(List<Users> _lstUSers)
        {
            SendDataAsync(_lstUSers);
        }

        private static String Base_URL = "http://localhost:8080/api/customerimport";
        private static HttpResponseMessage Response { get; set; }

        public static async void SendDataAsync(Object Users)
        {
            try
            {
                var client = new HttpClient();
                string json = JsonConvert.SerializeObject(Users);
                var content = new StringContent(json, Encoding.Unicode, "application/json");
                var request = new HttpRequestMessage();
                Response = await client.PostAsync(Base_URL, content).ConfigureAwait(false);
            }
            catch (Exception)
            {
                //Host not reacable.
                Library.WriteErrorLog("Host not reacable");
            }
        }

        public static string GetXMLData()
        {
            //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            //ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);

            var request = WebRequest.Create("http://localhost:5004/arx/export") as HttpWebRequest;
            request.Credentials = new NetworkCredential("master", "4bdk0jf2");

            var response = request.GetResponse();

            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

            var result = readStream.ReadToEnd();

            return result;
        }

        //private static bool AcceptAllCertifications(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        //{
        //    return true;
        //}
    }
}