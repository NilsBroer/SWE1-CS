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
            if (req.Url.Path.StartsWith("/navigation"))
                return 1.0f;

            return 0.0f;
        }

        public IResponse Handle(IRequest req)
        {
            if (req.IsValid)
            {
                if (req.Url.Path.StartsWith("/navigation"))
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
