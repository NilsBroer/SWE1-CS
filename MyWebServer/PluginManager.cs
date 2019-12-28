using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using MyWebServer.Plugins;
using BIF.SWE1.Interfaces;

namespace MyWebServer
{

    class PluginManager : IPluginManager
    {
        private List<IPlugin> Plugins_in = new List<IPlugin>();

        public PluginManager()
        {
            string myPath = Path.Combine(new string[] { Path.GetDirectoryName(this.GetType().Assembly.Location), "Plugin" });   //aktuelles Directory (deploy) (Hier musst du den Ordner "Plugin" erstellen Nils, einmal im Deploy Ordner einmal bei den unit tests, weist eh
            string[] dllFileNames = null;
            if (Directory.Exists(myPath))
            {
                dllFileNames = Directory.GetFiles(myPath, "*.dll"); // Alle DLL files finden
                
                //Assemblies für dynamisches Laden von Programmteilen (reflections)
                ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
                foreach (string dllFile in dllFileNames)
                {
                    AssemblyName myAssemblyName = AssemblyName.GetAssemblyName(dllFile);
                    Assembly myAssembly = Assembly.Load(myAssemblyName);
                    assemblies.Add(myAssembly);
                }

                //den Assemblies verschiedene Plugin-Typen hinzufügen
                Type pluginType = typeof(IPlugin);
                ICollection<Type> pluginTypes = new List<Type>();
                foreach (Assembly assembly in assemblies)
                {
                    if (assembly != null)
                    {
                        Type[] types = assembly.GetTypes();
                        foreach (Type type in types)
                        {
                            if (type.IsInterface || type.IsAbstract)    //Interfaces ignorieren und Abstracts ignorieren, brauchen ja nur diese Klassen
                            {
                                continue;
                            }
                            else
                            {
                                if (type.GetInterface(pluginType.FullName) != null)     //wenn es kein Interface oder Abtract ist, muss es eine Klasse sein, Klasse is gud
                                {
                                    pluginTypes.Add(type);
                                }
                            }
                        }
                    }
                }

                foreach (Type type in pluginTypes)
                {
                    IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                    Add(plugin);    //Plugins hinzufügen
                }
            }
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
