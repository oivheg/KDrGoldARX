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
            ARXExporter service = new ARXExporter();
            service.onDebug();
#else

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ARXExporter()
            };
            ServiceBase.Run(ServicesToRun);

#endif
        }
    }
}