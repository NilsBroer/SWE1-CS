using BIF.SWE1.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MyWebServer
{
    /// <summary>
    /// The Response Part of the HTTP Protocol
    /// </summary>
    public class Response : IResponse
    {
        HTTPhelper helper = new HTTPhelper();

        /// <summary>
        /// StatusCode Field
        /// </summary>
        public int statuscode_in;

        /// <summary>
        /// Content as Byte Array
        /// </summary>
        public byte[] content;

        /// <summary>
        /// Constructor, Adds Server Header and Creates Header Dictionary
        /// </summary>
        public Response()
        {
            this.Headers = new Dictionary<string, string>
            {
                {"ContentType", ""},  //echter name
                {"ContentLength", ""}
            };
            this.ServerHeader = "BIF-SWE1-Server";

            //AddHeader("Server", "Default Server-Name");  //Testing
        }

        /// <summary>
        /// List of all accepted Content Types
        /// </summary>
        public readonly IDictionary<string, string> ValidContentTypes = new Dictionary<string, string>()
        {
            {"html", "text/html; charset=UTF-8"}, {"txt", "text/plain"}, {"css", "text/css"}, {"png", "image/png"},
            {"gif", "image/gif"}, {"jpg", "image/jpeg"}, {"pdf", "application/pdf"}, {"json", "application/json"},
            {"js", "application/javascript"}
        };

        /*public Response(Request request)
        {
            this.ServerHeader = "BIF-SWE1-Server";

            this.Headers = new Dictionary<string, string>
            {
                {"ContentType", ""},
                {"ContentLength", ""}
            };

            //AddHeader("Server", "Default Server-Name");   //Testing
        }*/

        /// <summary>
        /// Length of the Response'S Content
        /// </summary>
        public int ContentLength
        {
            get
            {
                try
                {
                    return Int32.Parse(this.Headers["ContentLength"]);
                }
                catch (FormatException fe)
                {
                    throw fe;
                }
            }
        }

        /// <summary>
        /// Type of the Response's Content
        /// </summary>
        public string ContentType
        {
            get => this.Headers["ContentType"]; set => this.Headers["ContentType"] = value;
        }

        /// <summary>
        /// Gets all Headers of the Response
        /// </summary>
        public IDictionary<string, string> Headers { get; }

        /// <summary>
        /// Sets the Status Code of the Response
        /// </summary>
        public int StatusCode
        {
            get
            {
                if (statuscode_in == 0)
                    throw new System.InvalidOperationException("Status Code not set properly");

                return statuscode_in;
            }

            set => statuscode_in = value;
        }

        /// <summary>
        /// Gets the Status Description of the Response
        /// </summary>
        public string Status
        {
            get
            {
                return (this.StatusCode.ToString() + " " + helper.HTTPStatusCodes[this.statuscode_in]);
            }
        }

        /// <summary>
        /// Server Header Already Added, can be disregarded
        /// </summary>
        public string ServerHeader { get; set; }

        /// <summary>
        /// Adds a Header to the Response
        /// </summary>
        public void AddHeader(string header, string value)
        {
            Headers[header] = value;
        }

        /// <summary>
        /// Sends the Response
        /// </summary>
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

        /// <summary>
        /// Converts a given string to Bytes to be Set by another Method
        /// </summary>
        public void SetContent(string content)
        {
            this.SetContent(Encoding.UTF8.GetBytes(content));
        }

        /// <summary>
        /// Sets the Content of the Response with given ByteArray
        /// </summary>
        public void SetContent(byte[] content)
        {
            this.content = content;
            this.Headers["ContentLength"] = this.content.Length.ToString();   //direkt die eigenschaft
        }

        /// <summary>
        /// Sets the Content of the Response by Converting from Stream and Passing it as a ByteArray
        /// </summary>
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