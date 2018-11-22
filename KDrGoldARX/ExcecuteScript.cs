using System.IO;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

namespace KDrGoldARX
{
    internal static class ExcecuteScript
    {
        public static void Runscript()
        {
            //string sqlConnectionString = @"Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=hegglandtest;Data Source=HEGGLAND\HEGGLAND";
            string sqlConnectionString = @"Server=HEGGLAND\HEGGLAND;Database=HegglandTEst;Integrated Security=True;Trusted_Connection=True;MultipleActiveResultSets=true";

            string script = File.ReadAllText(@"C:\Users\oivhe\OneDrive - KDR Stavanger AS\ARX Integrasjon\KDR_Import_KunderOgKortARX.sql");

            SqlConnection conn = new SqlConnection(sqlConnectionString);

            Server server = new Server(new ServerConnection(conn));

            server.ConnectionContext.ExecuteNonQuery(script);
        }
    }
}