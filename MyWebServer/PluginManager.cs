using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using BIF.SWE1.Interfaces;

namespace MyWebServer
{
    class PluginManager : IPluginManager
    {
        private List<IPlugin> Plugins_in = new List<IPlugin>();
        private string pluginPath_in;

        public PluginManager(string pluginPath = "./Plugins")
        {
            /*pluginPath_in = pluginPath;

            if (File.Exists(pluginPath)) throw new Exception("Expected directory, not a file");
            if (!Directory.Exists(pluginPath))
                throw new Exception("Directory \"" + Path.GetFullPath(pluginPath) + "\" does not exist");

            // load each dll file in Plugin Directory, create plugin, add it to Plugins List
            string[] fileEntries = Directory.GetFiles(pluginPath, "*.dll");

            IEnumerable<IPlugin> plugins = fileEntries.SelectMany(pluginP =>
            {
                Assembly pluginAssembly = LoadPlugin(pluginP);
                return CreatePlugins(pluginAssembly);
            });

            foreach (var plugin in plugins)
            {
                Add(plugin);
            }*/
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
            if (highest <= 0.1f)
            {
                Console.WriteLine("No appropriate plugin found."); //Change later
                return null;
            }
            Console.WriteLine("Returned: " + plug.ToString()); //Rather print plugin-name
            return plug;
        }

        public void Clear()
        {
            Plugins_in.Clear();
        }

        /*public IPlugin GetSpecificPlugin(string pluginP)
        {
            Assembly pluginAssembly = LoadPlugin(pluginP);
            return CreatePlugins(pluginAssembly).First();
        }*/

        private static IEnumerable<IPlugin> CreatePlugins(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IPlugin).IsAssignableFrom(type))
                {
                    if (Activator.CreateInstance(type) is IPlugin result)
                    {
                        yield return result;
                    }
                }
            }
        }

        /*private static Assembly LoadPlugin(string relativePath)
        {
            PluginLoadContext loadContext = new PluginLoadContext(relativePath);
            if (File.Exists(relativePath))
            {
                return loadContext.LoadFromAssemblyName(
                    new AssemblyName(Path.GetFileNameWithoutExtension(relativePath)));
            }

            return null;
        }*/
    }
}
