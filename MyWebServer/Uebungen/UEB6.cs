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
            return new PluginManager();
        }

        public IRequest GetRequest(System.IO.Stream network)
        {
            return new Request(network);
        }

        public string GetNaviUrl()
        {
            NavigationPlugin plugin = new NavigationPlugin();
            return plugin.GetUrl();
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
            TemperatureMeasurementPlugin plugin = new TemperatureMeasurementPlugin();
            return plugin.GetRestUrl(from, until);
        }

        public string GetTemperatureUrl(DateTime from, DateTime until)
        {
            TemperatureMeasurementPlugin plugin = new TemperatureMeasurementPlugin();
            return plugin.GetUrl(from, until);
        }

        public IPlugin GetToLowerPlugin()
        {
            return new ToLowerPlugin();
        }

        public string GetToLowerUrl()
        {
            ToLowerPlugin plugin = new ToLowerPlugin();
            return plugin.GetUrl();
        }
    }
}
