using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace KDRGoldConoslenoCore
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<WebServer>(s =>
                {
                    s.ConstructUsing(name => new WebServer());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                //x.RunAsNetworkService();
                x.RunAsLocalSystem();
                x.SetDisplayName("ImportToGold");
                x.SetDescription("Web Service for Recieving customer data and importing to KDRGold");
                x.SetServiceName("KDRGoldImporter");
            });
        }
    }
}