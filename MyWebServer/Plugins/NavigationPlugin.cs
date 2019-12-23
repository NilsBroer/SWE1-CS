using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;

namespace MyWebServer.Plugins
{
    class NavigationPlugin : IPlugin
    {
        public float CanHandle(IRequest req)
        {
            if (req.IsValid == false)
                return 0.0f;

            if ((req.Method.Equals("GET") || req.Method.Equals("POST")) && (req.Url.Path.StartsWith("/navigation")))
            {
                return 0.9f;
            }

            return 0.0f;
        }

        public string GetUrl()
        {
            return "/navigation/";
        }

        public IResponse Handle(IRequest req)
        {
            throw new NotImplementedException();
        }
    }
}
