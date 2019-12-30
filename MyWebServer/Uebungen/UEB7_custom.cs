using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;
using MyWebServer;
using MyWebServer.Plugins;

namespace Uebungen
{
    public class UEB7_custom
    {
        public void HelloWorld()
        {
            // I'm fine
        }

        public IUrl GetUrl(string path)
        {
            return new Url(path);
        }

        public IPluginManager GetPluginManager()
        {
            return new PluginManager();
        }

        public IRequest GetRequest(System.IO.Stream network)
        {
            return new Request(network);
        }

        public IPlugin GetTemperaturePlugin()
        {
            return new TemperatureMeasurementPlugin();
        }

        public string GetNaviUrl()
        {
            return "/navigation";
        }

        public IPlugin GetNavigationPlugin()
        {
            return new NavigationPlugin();
        }
    }
}
