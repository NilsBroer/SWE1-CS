using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;
using MyWebServer;

namespace MyWebServer.Plugins
{
    class ToLowerPlugin : IPlugin
    {
        public float CanHandle(IRequest req)
        {
            if (req.IsValid == false)
                return 0.0f;

            if ( (req.Method.Equals("GET") || req.Method.Equals("POST")) && (req.Url.Path.StartsWith("/tolower")) )
            {
                return 0.9f;
            }

            return 0.1f;
        }

        public IResponse Handle(IRequest req)
        {
            /*if (this.CanHandle(req) <= 0)
                throw new Exception("Can't handle this sheet");

            IResponse response = new Response();
            response.StatusCode = 200;

            StringBuilder content = new StringBuilder();

            content.Append("<!DOCTYPE html><html><head><meta charset=\"UTF-8\"></head><body><form method=\"POST\" action=\"/tolower/\"><textarea name=\"text\"></textarea><br /><input type=\"submit\" /></form><hr /><pre>");

            if (req.Method.Equals("POST") && req.GetPostVal("text") != null && req.GetPostVal("text").Length > 0 )
            {
                String str = req.GetPostVal("text").ToLower();
                str.Replace("<", "&lt;");
                str.Replace(">", "&gt;");
                content.Append(str);
            }
            else
            {
                content.Append("Enter text!");
            }

            content.Append("</pre></body></html>");
            response.SetContent(content.ToString());

            return response;*/
            throw new NotImplementedException();
        }
    }
}
