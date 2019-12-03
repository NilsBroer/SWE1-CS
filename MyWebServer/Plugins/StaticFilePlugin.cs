using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;
using System.IO;

namespace MyWebServer.Plugins
{
    public class StaticFilePlugin : IPlugin
    {
        string folder;

        public float CanHandle(IRequest req)
        {

            if (req.IsValid == false)
                return 0.0f;

            if ((req.Method.Equals("GET") || req.Method.Equals("POST")) && (req.Url.Path.StartsWith("/static")))
            {
                return 0.9f;
            }
            else
                return 0.1f;
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

        public string GetStaticFileUrl(string fileName)
        {
            return this.folder += fileName;
        }

        public void SetStatiFileFolder(string folder)
        {
            this.folder = folder;
        }

        public IResponse Handle(IRequest req)
        {
            Response res = new Response();

            this.folder = System.AppDomain.CurrentDomain.BaseDirectory;
            if (this.folder.Contains("deploy"))
                this.folder += "/../files/";
            else
                this.folder += "/../../unit tests/BIF-SWE1/static-files/";

            string filepath = folder += req.Url.Path;

            Console.WriteLine(filepath);
            if (File.Exists(filepath))
            {
                res.StatusCode = 200;
                res.SetContent(File.ReadAllBytes(filepath));
            }
            else
            {
                res.StatusCode = 404;
            }

            return res;
        }
    }
}
