using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;

namespace MyWebServer.Plugins
{
    class ToLowerPlugin : IPlugin
    {
        public float CanHandle(IRequest req)
        {
            if (req.Url.Path.StartsWith("/tolower"))
                return 1.0f;

            if (req.Url.RawUrl == "/")
                return 0.1f;

            return 0.0f;
        }

        public IResponse Handle(IRequest req)
        {
            if (req.IsValid)
            {
                if (req.Url.Path.StartsWith("/tolower"))
                {
                    Response res = new Response();
                    res.StatusCode = 200;
                    res.SetContent("GREAT SUCCESS");
                    return res;
                }

                if (req.Url.RawUrl == "/")
                {
                    Response res = new Response();
                    res.StatusCode = 200;
                    res.SetContent("MILD SUCCESS");
                    return res;
                }
            }

            Response re = new Response();
            re.StatusCode = 500;
            re.SetContent("Cannot Handle!");
            return re;
        }
    }
}
