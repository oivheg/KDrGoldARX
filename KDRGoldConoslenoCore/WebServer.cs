using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDRGoldConoslenoCore
{
    public class WebServer
    {
        private IDisposable _webap;

        public void Start()
        {
            _webap = WebApp.Start<Startup>("http://*:8080");
        }

        public void Stop()
        {
            _webap?.Dispose();
        }
    }
}