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

        List<IPlugin> Plugins_in = new List<IPlugin>();

        public IEnumerable<IPlugin> Plugins { get => Plugins_in; }

        public void Add(IPlugin plugin)
        {
            Plugins_in.Add(plugin);
        }

        public void Add(string plugin)
        {
            IPlugin pluginObj = (IPlugin)Activator.CreateInstance(Type.GetType(plugin)); //Examine what this does in more detail

            if (pluginObj == null)
            {
                throw new Exception();
            }
            else
                Plugins_in.Add(pluginObj);
        }

        public void Clear()
        {
            Plugins_in.Clear();
        }
    }
}
