using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;
using MyWebServer;

namespace Uebungen
{
    public class UEB3
    {
        public void HelloWorld()
        {
        }

        public IRequest GetRequest(System.IO.Stream network)
        {
            return new Request(network);
        }

        public IResponse GetResponse()
        {
            throw new NotImplementedException();
        }

        public IPlugin GetTestPlugin()
        {
            throw new NotImplementedException();
        }
    }
}
