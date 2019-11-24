using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;

namespace MyWebServer.Plugins
{
    class TemperatureMeasurementPlugin : IPlugin
    {
        public float CanHandle(IRequest req)
        {
            if (req.Url.Path.StartsWith("/temperature/") || req.Url.Path.StartsWith("//gettemperature"))
            {
                return 1.0f;
            }
            else
                return 0.1f;
        }

        public IResponse Handle(IRequest req)
        {
            throw new NotImplementedException();
        }
    }
}
