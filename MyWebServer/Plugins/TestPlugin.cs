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
            if (req.Url.RawUrl.ToLower().Contains("test") || req.Url.RawUrl == "/" /*replace this with a search for a page that also succeeds if no page is requested (rawUrl = "/")*/)
                return 1.0f;
            else
                return 0.0f;
        }

        public IResponse Handle(IRequest req)
        {
            Response re = new Response();
            re.StatusCode = 200;
            re.SetContent("GREAT SUCCESS");
            return re;
        }
    }
}
