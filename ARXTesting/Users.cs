using System.Collections.Generic;

namespace ARXTesting
{
    public class Users
    {
        public string UserId { get; set; }
        public string Company { get; set; }
        public List<string> CardList = new List<string>();
    }
}