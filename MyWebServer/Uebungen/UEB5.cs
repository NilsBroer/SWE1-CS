using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;
using MyWebServer;
using MyWebServer.Plugins;

namespace Uebungen
{
    public class UEB5
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

        public IPlugin GetStaticFilePlugin()
        {
            return new StaticFilePlugin();
        }

        public string GetStaticFileUrl(string fileName)
        {
            StaticFilePlugin sfp = new StaticFilePlugin();
            return sfp.GetUrl(fileName);
        }

        public void SetStatiFileFolder(string folder)
        {
            StaticFilePlugin sfp = new StaticFilePlugin();
            sfp.SetStatiFileFolder(folder);
        }
    }
}
