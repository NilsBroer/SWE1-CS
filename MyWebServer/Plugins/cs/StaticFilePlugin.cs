using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;
using System.IO;

namespace MyWebServer.Plugins
{
    /// <summary>
    /// Plugin to display local files on the Server
    /// </summary>
    [marked]
    public class StaticFilePlugin : IPlugin
    {
        string folder;

        /// <summary>
        /// Checks if the Request is to be handled by this Plugin
        /// </summary>
        public float CanHandle(IRequest req)
        {

            if (req.IsValid == false)
                return 0.0f;

            if ((req.Method.Equals("GET") || req.Method.Equals("POST")) && (req.Url.Path.StartsWith("/static")))
            {
                return 0.9f;
            }
            else
                return 0.5f; //plugin can search for weird file
            /*
            string filepath = req.Url.Path;
            if (filepath.EndsWith(".txt")) //^=Ends with mime type
                return 0.9f;
            else
            {
                return 0f;
            }
            */
        }

        /// <summary>
        /// StaticFile's accepted URL format
        /// </summary>
        public string GetUrl()
        {
            return "/static";
        }

        /// <summary>
        /// URL to be handled by this Plugin
        /// </summary>
        public string GetUrl(string fileName)
        {
            if (fileName.StartsWith("/"))
                fileName = fileName.Substring(1);
            return "/static/" + fileName;
        }

        /// <summary>
        /// Sets the Folder to be used by the plugin
        /// </summary>
        public void SetStatiFileFolder(string folder)
        {
            this.folder = folder;
        }

        /// <summary>
        /// Displays the File requested in the Browser
        /// </summary>
        public IResponse Handle(IRequest req)
        {
            Response res = new Response();

            this.folder = System.AppDomain.CurrentDomain.BaseDirectory;
            if (this.folder.Contains("deploy"))
                this.folder += "/../files/";
            else
                this.folder += "/../../unit tests/BIF-SWE1/static-files/";

            string filepath = req.Url.Path; //prettify following, but it works :)

            if (filepath.IndexOf("/static") != -1)
                filepath = filepath.Substring(filepath.IndexOf("/static") + "/static".Length);

            if ((filepath).StartsWith("/"))
                filepath = folder += filepath.Substring(1);
            else
                filepath = folder += filepath;

            Console.WriteLine(filepath);
            if (File.Exists(filepath))
            {
                res.StatusCode = 200;
                res.SetContent(File.ReadAllBytes(filepath));
                res.ContentType = filepath.Substring(filepath.LastIndexOf(".")+1);
            }
            else
            {
                res.StatusCode = 404;
            }

            return res;
        }
    }
}
