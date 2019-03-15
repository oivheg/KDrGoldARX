using ARXToKDRGold.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ARXToKDRGold
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
#if DEBUG

            MainAsync().Wait();
            //HttpRequest.Get("customerimport").ConfigureAwait(true).GetAwaiter();
        }

        private static async Task MainAsync()
        {
            try
            {
                ARXExporter service = new ARXExporter();
                await service.OnDebugAsync();

                Console.WriteLine("Program finished");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

#else

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ARXExporter()
            };
            ServiceBase.Run(ServicesToRun);
        }

#endif
    }
}