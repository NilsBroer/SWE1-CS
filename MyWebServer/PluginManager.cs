using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BIF.SWE1.Interfaces;

namespace MyWebServer
{
    class PluginManager : IPluginManager
    {
        List<IPlugin> Plugins_in = new List<IPlugin>();

        public PluginManager()
        {
            MyWebServer.Plugins.TestPlugin test = new Plugins.TestPlugin();
            MyWebServer.Plugins.NavigationPlugin navi = new Plugins.NavigationPlugin();
            MyWebServer.Plugins.StaticFilePlugin stat = new Plugins.StaticFilePlugin();
            MyWebServer.Plugins.TemperatureMeasurementPlugin temp = new Plugins.TemperatureMeasurementPlugin();
            MyWebServer.Plugins.ToLowerPlugin tlwr = new Plugins.ToLowerPlugin();

            Add(test);
            Add(navi);
            Add(stat);
            Add(temp);
            Add(tlwr);
        }

        public IEnumerable<IPlugin> Plugins => Plugins_in;

        public IEnumerable<IPlugin> GetPlugins()
        {
            return this.Plugins;
        }

        public void Add(IPlugin plugin)
        {
            Plugins_in.Add(plugin);
        }

        public void Add(string plugin)
        {
            IPlugin pluginObj = (IPlugin)Activator.CreateInstance(Type.GetType(plugin)); 

            if (pluginObj == null)
            {
                throw new Exception();
            }
            else
                Plugins_in.Add(pluginObj);
        }

        public IPlugin getRequiredPlugin (IRequest req)
        {
            IPlugin plug = null;
            float highest = 0;
            
            foreach (IPlugin p in this.GetPlugins())
            {
                float checker = p.CanHandle(req);
                if (checker > highest)
                {
                    highest = checker;
                    plug = p;
                }
            }

            return plug;
        }

        public void Clear()
        {
            Plugins_in.Clear();
        }
    }
}
