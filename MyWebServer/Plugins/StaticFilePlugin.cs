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
            throw new NotImplementedException();
        }

        public IResponse Handle(IRequest req)
        {
            throw new NotImplementedException();
        }
    }
}
