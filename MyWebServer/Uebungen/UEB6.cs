using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;
using MyWebServer;
using MyWebServer.Plugins;

namespace Uebungen
{
    public class UEB6
    {
        public void HelloWorld()
        {
        }

        public IPluginManager GetPluginManager()
        {
            throw new NotImplementedException();
        }

        public IRequest GetRequest(System.IO.Stream network)
        {
            return new Request(network);
        }

        public string GetNaviUrl()
        {
            throw new NotImplementedException();
        }

        public IPlugin GetNavigationPlugin()
        {
            return new NavigationPlugin();
        }

        public IPlugin GetTemperaturePlugin()
        {
            return new TemperatureMeasurementPlugin();
        }

        public string GetTemperatureRestUrl(DateTime from, DateTime until)
        {
            throw new NotImplementedException();
        }

        public string GetTemperatureUrl(DateTime from, DateTime until)
        {
            throw new NotImplementedException();
        }

        public IPlugin GetToLowerPlugin()
        {
            return new ToLowerPlugin();
        }

        public string GetToLowerUrl()
        {
            throw new NotImplementedException();
        }
    }
}
