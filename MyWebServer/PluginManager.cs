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
            
        }
        public IEnumerable<IPlugin> Plugins => Plugins_in;

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
