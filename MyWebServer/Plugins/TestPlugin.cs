using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;

namespace MyWebServer.Plugins
{
    class TestPlugin : IPlugin
    {
        public float CanHandle(IRequest req)
        {
            if (req.Url.RawUrl.ToLower().Contains("test") || req.Url.RawUrl == "/" /*replace this with a search for a page that returns main-page if (rawUrl = "/")*/)
                return 1.0f;
            else
                return 0.0f; //test-case requires == 0f
        }

        public IResponse Handle(IRequest req)
        {
            if (CanHandle(req) != 0.0f)
            {
                Response re = new Response();
                re.StatusCode = 200;
                re.SetContent("GREAT SUCCESS");
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
