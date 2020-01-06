using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;

namespace MyWebServer.Plugins
{
    /// <summary>
    /// A Test Plugin
    /// </summary>
    [marked]
    public class TestPlugin : IPlugin
    {
        /// <summary>
        /// Determines if the Plugin handles this Request
        /// </summary>
        public float CanHandle(IRequest req)
        {
            if (req.Url.RawUrl.ToLower().Contains("test") || req.Url.RawUrl == "/" /*replace this with a search for a page that returns main-page if (rawUrl = "/")*/)
                return 1.0f;
            else
                return 0.0f; //test-case requires == 0f
        }

        /// <summary>
        /// The URL accepted by the Plugin
        /// </summary>
        public string GetUrl()
        {
            return "/test";
        }

        /// <summary>
        /// Displays GREAT SUCCESS in the Browser
        /// </summary>
        public IResponse Handle(IRequest req)
        {
            if (CanHandle(req) != 0.0f)
            {
                Response re = new Response();
                re.StatusCode = 200;
                re.SetContent("WELCOME TO MY SERVER :D");
                return re;
            }
            else
            {
                Response re = new Response();
                re.StatusCode = 500;
                return re;
            }
        }
    }
}
