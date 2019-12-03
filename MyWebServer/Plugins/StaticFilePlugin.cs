using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;

namespace MyWebServer.Plugins
{
    class StaticFilePlugin : IPlugin
    {
        public float CanHandle(IRequest req)
        {
            if (req.IsValid == false)
                return 0.0f;

            if ((req.Method.Equals("GET") || req.Method.Equals("POST")) && (req.Url.Path.StartsWith("/static")))
            {
                return 0.9f;
            }

            return 0.1f;
        }

        public IResponse Handle(IRequest req)
        {
            throw new NotImplementedException();
        }
    }
}
