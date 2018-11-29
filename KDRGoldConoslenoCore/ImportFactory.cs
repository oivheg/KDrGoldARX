using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    LstUsers.Add(user);
                }
            }

            return LstUsers;
        }

        public static void GenerateXML(List<Users> lstusers)
        {
        }

        public static void ImporToKDRGold()
        {
        }
    }
}