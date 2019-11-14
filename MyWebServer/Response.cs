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
                {"cType", ""},  //echter name
                {"cLength", ""}
            };

            this.ServerHeader = "BIF-SWE1-Server";

            //AddHeader("Server", "Default Server-Name");  //Testing
        }

        /*public Response(Request request)
        {
            this.ServerHeader = "BIF-SWE1-Server";

            this.Headers = new Dictionary<string, string>
            {
                {"cType", ""},
                {"cLength", ""}
            };

            //AddHeader("Server", "Default Server-Name");   //Testing
        }*/

        public int ContentLength
        {
            get
            {
                try
                {
                    return Int32.Parse(this.Headers["cLength"]);
                }
                catch (FormatException fe)
                {
                    throw fe;
                }
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
                    throw new System.InvalidOperationException("Status Code not set properly");

                return this.statuscode_in;
            }

            set => this.statuscode_in = value;
        }

        public string Status
        {
            get
            {
                return (this.StatusCode.ToString() + " " + helper.HTTPStatusCodes[this.statuscode_in]);
            }
        }

        public string ServerHeader { get; set; }
       
        public void AddHeader(string header, string value)
        {
            // yay, value exists!
            Headers[header] = value;
        }

        public void Send(Stream network)
        {
            if ((this.ContentType != null && this.ContentType != "") && this.ContentLength <= 0)
            {
                throw new System.InvalidOperationException("You have to send content when claiming to do so, man. I mean, cmon dude...");
            }

            StreamWriter sw = new StreamWriter(network);

            sw.WriteLine("HTTP/1.1 {0}", this.Status);   //Status code hier

            foreach (KeyValuePair<string, string> entry in Headers) //"Normale" Header hier
            {
                if (entry.Value != "" && entry.Value != null)
                {
                    sw.WriteLine("{0}: {1}", entry.Key, entry.Value);
                }
            }

            sw.WriteLine("Server: {0}", this.ServerHeader); //Server Header hier
            sw.WriteLine();
            sw.Flush();

            if (this.StatusCode != 404 && this.content != null)  //Content hier
            {
                BinaryWriter bw = new BinaryWriter(network);
                bw.Write(this.content);
                bw.Flush();
            }
        }

        public void SetContent(string content)
        {
            this.SetContent(Encoding.UTF8.GetBytes(content));
        }

        public void SetContent(byte[] content)
        {
            this.content = content;
            this.Headers["cLength"] = this.content.Length.ToString();   //direkt die eigenschaft
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