using BIF.SWE1.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MyWebServer
{
    public class Response : IResponse
    {
        int statuscode_in, ContentLength_in;
        string status_in, ContentType_in;
        IDictionary<string,string> headers_in;

        HTTPhelper helper = new HTTPhelper();

        public Response()
        {
            //return;
        }
        public Response(Request request)
        {
            headers_in = new Dictionary<string, string>();
            AddHeader("Server", "Default Server-Name");

            
            if (request.IsValid)
                statuscode_in = 200; // helper.HTTPStatusCodes.FirstOrDefault(x => x.Value == "OK").Key;
            else
                statuscode_in = 400; // helper.HTTPStatusCodes.FirstOrDefault(x => x.Value == "Bad Request").Key;
            
        }

        public IDictionary<string, string> Headers => headers_in;

        public int ContentLength => ContentLength_in;

        public string ContentType { get => ContentType_in; set => ContentType = value; }

        public int StatusCode { get => statuscode_in; set => statuscode_in = value;}

        public string Status => statuscode_in.ToString() + " " + helper.HTTPStatusCodes[statuscode_in];

        public string ServerHeader { get => throw new NotImplementedException(); set => ServerHeader = value; }

        public void AddHeader(string header, string value)
        {
            Headers.Add(header, value);
        }

        public void Send(Stream network)
        {
            throw new NotImplementedException();
        }

        public void SetContent(string content)
        {
            throw new NotImplementedException();
        }

        public void SetContent(byte[] content)
        {
            throw new NotImplementedException();
        }

        public void SetContent(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}