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
        HTTPhelper helper = new HTTPhelper();
        public int statuscode_in;
        public byte[] content;

        public Response()
        {
            this.Headers = new Dictionary<string, string>
            {
                {"cType", ""},
                {"cLength", ""}
            };
        }

        public Response(Request request)
        {
            this.ServerHeader = "WebServer vong Chris und Nils";

            this.Headers = new Dictionary<string, string>
            {
                {"cType", ""},
                {"cLength", ""}
            };

            AddHeader("Server", "Default Server-Name");
        }

        public int ContentLength
        {
            get
            {
                return int.Parse(this.Headers["cLength"]);
            }
        }

        public string ContentType
        {
            get => this.Headers["cType"]; set => this.Headers["cType"] = value;
        }

        public IDictionary<string, string> Headers { get; }

        public int StatusCode
        {
            get
            {
                if (this.statuscode_in == 0)
                    throw new System.InvalidOperationException("Status Code is Null");

                return this.statuscode_in;
            }

            set => this.statuscode_in = value;
        }

        public string Status
        {
            get
            {
                return (statuscode_in.ToString() + " " + helper.HTTPStatusCodes[this.statuscode_in]);
            }
        }

        public string ServerHeader { get; set; }

        public void AddHeader(string header, string value)
        {
            if (Headers.ContainsKey(header))
            {
                // yay, value exists!
                Headers[header] = value;
            }
            else
            {
                // darn, lets add the value
                Headers.Add(header, value);
            };
        }

        public void Send(Stream network)
        {
            throw new NotImplementedException();
        }

        public void SetContent(string content)
        {
            this.SetContent(Encoding.UTF8.GetBytes(content));
        }

        public void SetContent(byte[] content)
        {
            this.content = content;
            this.Headers["cLength"] = this.content.Length.ToString();
        }

        public void SetContent(Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                SetContent(ms.ToArray());
            }
        }
    }
}