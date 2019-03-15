using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace KDRGoldConoslenoCore.Controller
{
    public class CustomerImportController : ApiController
    {
        private List<Users> LstUsers = new List<Users>();

        // GET api/demo
        public string Get()
        {
            try
            {
                Users usr = new Users();
                usr.Company = "test";
                usr.FirstName = "Ølberget";
                usr.LastName = "lastname";
                usr.UserId = "999";

                usr.CardList.Add("12345");

                LstUsers.Add(usr);
                ImportFactory.GenerateXML(LstUsers);
                bool succesess = ImportFactory.ImporToKDRGold();
            }
            catch (Exception e)
            {
                return "Error see details in " + ImportFactory.ErrorReport(e);
            }
            return "OK";
        }

        // GET api/demo/5
        public string Get(int id)
        {
            return "Hello, World!";
        }

        // POST api/demo
        public void Post(List<Users> lstUsers)
        {
            try
            {
                LstUsers.Clear();
                LstUsers = lstUsers;
                //LstUsers = ImportFactory.CleanData(lstUsers);
                ImportFactory.GenerateXML(LstUsers);
                bool succesess = ImportFactory.ImporToKDRGold();

                ImportFactory.Archive(succesess);
            }
            catch (Exception e)
            {
                string SeError = "Error see details in " + ImportFactory.ErrorReport(e);
            }
        }

        //// POST api/demo
        //[HttpPost]
        //public void ARXImport(List<Users> lstUsers)
        //{
        //    LstUsers = lstUsers;
        //}

        // PUT api/demo/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/demo/5
        public void Delete(int id)
        {
        }
    }
}