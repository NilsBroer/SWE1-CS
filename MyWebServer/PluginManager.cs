using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using BIF.SWE1.Interfaces;
using System.Reflection;
using MyWebServer.Plugins;

namespace MyWebServer
{
    /// <summary>
    /// The PluginManager to choose which Plugin handles a certain Request
    /// </summary>
    class PluginManager : IPluginManager
    {
        private List<IPlugin> Plugins_in = new List<IPlugin>();
        private static string _PluginFolder = "";
        private static string _ExecutionLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /*
        public PluginManager(bool a)
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
            
            
            string path = @"C:\Users\Nsync\Google Drive\UNI\Semester 03\SWE_CS\SWE1-CS\MyWebServer\Plugins";
            string[] dll_plugin_strings = Directory.GetFiles(path + "/dll", "*.dll");
            IPlugin [] dll_plugins = new IPlugin[dll_plugin_strings.Length];
            dll_plugins = (
                from file in dll_plugin_strings                 //For each .dll-file found
                let assembly = Assembly.LoadFile(file)          //load the assembly
                from type in assembly.GetExportedTypes()        //for every type in the assembly
                where typeof(IPlugin).IsAssignableFrom(type)    //where said type implements the interface
                select (IPlugin)Activator.CreateInstance(type)  //create an instance
                ).ToArray();                                    //and materialize all to an array
            foreach(var plugin in dll_plugins)
            {
                Console.WriteLine("Plugin: " + plugin.ToString());
                Add(plugin);
            }
        }
*/

        /// <summary>
        /// Constructor, loads Plugins at runtime from dll or exe
        /// </summary>
        public PluginManager()
        {

            Console.WriteLine(_ExecutionLocation + _PluginFolder);
            // create Plugins from exe
            var lst = Directory.GetFiles(_ExecutionLocation)
                .Where(i => new[] { ".dll", ".exe" }.Contains(Path.GetExtension(i)))
                .SelectMany(i => Assembly.LoadFrom(i).GetTypes())
                .Where(myType => myType.IsClass
                              && !myType.IsAbstract
                              && myType.GetCustomAttributes(true).Any(x => x.GetType() == typeof(marked))
                              && myType.GetInterfaces().Any(i => i == typeof(IPlugin)));
            
            // Add plugins from plugin folder
            lst = lst.Concat((Directory.GetFiles(Path.Combine(_ExecutionLocation, _PluginFolder)))
                .Where(i => new[] { ".dll", ".exe" }.Contains(Path.GetExtension(i)))
                .SelectMany(i => Assembly.LoadFrom(i).GetTypes())
                .Where(myType => myType.IsClass
                              && !myType.IsAbstract
                              && myType.GetInterfaces().Any(i => i == typeof(IPlugin))));
            
            foreach (Type type in lst)
            {
                Add((IPlugin)Activator.CreateInstance(type));
            }
        }

        /// <summary>
        /// The Plugins which are available
        /// </summary>
        public IEnumerable<IPlugin> Plugins => Plugins_in;

        /// <summary>
        /// returns all available Plugins
        /// </summary>
        public IEnumerable<IPlugin> GetPlugins()
        {
            return this.Plugins;
        }

        /// <summary>
        /// Adds a Plugin to the Manager
        /// </summary>
        public void Add(IPlugin plugin)
        {
            Plugins_in.Add(plugin);
        }

        /// <summary>
        /// Adds a Plugin via it's name
        /// </summary>
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

        /// <summary>
        /// Chooses the best Plugin for the Request
        /// </summary>
        public IPlugin GetBestPlugin (IRequest req)
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

        /// <summary>
        /// Clears all Plugins from the Manager
        /// </summary>
        public void Clear()
        {
            Plugins_in.Clear();
        }
    }
}
