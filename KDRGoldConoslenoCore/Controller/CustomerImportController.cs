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
        // GET api/demo
        public IEnumerable<string> Get()
        {
            return new string[] { "Hello", "World" };
        }

        // GET api/demo/5
        public string Get(int id)
        {
            return "Hello, World!";
        }

        // POST api/demo
        public void Post(List<Users> lstUsers)
        {
            LstUsers = lstUsers;
        }

        private List<Users> LstUsers = new List<Users>();

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