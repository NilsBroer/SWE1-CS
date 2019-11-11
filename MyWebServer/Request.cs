using BIF.SWE1.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MyWebServer
{
    public class Request : IRequest
    {
        string method_in, url_str, user_agent_in, content_type_in;
        int content_length_in = 0, header_count_in = 0; //header_count for lines used or all lines?
        bool isValid_in = false;
        Url url_in = null;

        Stream content_stream_in;
        byte[] content_bytes_in;
        string content_string_in;

        Dictionary<string,string> headers_in = new Dictionary<string, string>();

        string[] valid_methods = { "POST", "GET", "PUT", "PATCH", "DELETE", "HEAD", "TRACE", "OPTIONS", "CONNECT" };

        public Request(System.IO.Stream stream_in)
        {
            StreamReader stream = new StreamReader(stream_in);
            String line, hname, hvalue;
            while ((line = stream.ReadLine()) != "")
            {
                try
                {
                    if (line.Contains("HTTP"))
                    {
                        method_in = line.Substring(0, line.IndexOf(" ")).ToUpper();

                        url_str = line.Substring(method_in.Length + 1, line.Length - (method_in.Length + " HTTP/1.1".Length + 1));
                        header_count_in++;

                        url_in = new Url(url_str);

                        if (valid_methods.Contains(method_in))
                            isValid_in = true;
                    }

                    if (line.Contains(": "))
                    {
                        hname = line.Substring(0, line.IndexOf(": "));
                        hvalue = line.Substring(line.IndexOf(": ") + 2); //+2 --> exclude ": "
                        headers_in.Add(hname.ToLower(), hvalue);
                        header_count_in++;

                        //Console.WriteLine("Name: " + hname + " - Value: " + hvalue);

                        if (hname == "User-Agent")
                            user_agent_in = hvalue;
                        else if (hname == "Content-Type")
                            content_type_in = hvalue;
                        else if (hname == "Content-Length")
                            content_length_in = Int32.Parse(hvalue);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            char[] buffer = new char[content_length_in];
            stream.ReadBlock(buffer, 0, content_length_in);
            content_bytes_in = Encoding.ASCII.GetBytes(buffer);

            content_string_in = System.Text.Encoding.UTF8.GetString(content_bytes_in);

            content_stream_in = new MemoryStream(content_bytes_in);

        }

 //testing...

        public String toString()
        {
            String returnValue = "Request Method: " + this.method_in + "\n";
            returnValue = returnValue + "URL: " + this.url_in + "\n";
            returnValue = returnValue + "Valid: " + (this.isValid_in ? "Yes" : "No") + "\n";
            returnValue = returnValue + "User-Agent: " + this.user_agent_in + "\n";
            returnValue = returnValue + "Content-Type: " + this.content_type_in + "\n";
            returnValue = returnValue + "Content-Length: " + this.content_length_in + "\n";
            returnValue = returnValue + "Header-Count: " + this.header_count_in + "\n";
            returnValue = returnValue + "Content as..." + "\nStream: " + this.content_stream_in + "\nString: " + this.content_string_in + "\nBytes: " + this.content_bytes_in + "\n";
            
            return returnValue;
        }

//-----------------------------------------------------------------------------------------------------------------

        public bool IsValid => isValid_in;

        public string Method => method_in;

        public IUrl Url => url_in;

        public IDictionary<string, string> Headers => headers_in;

        public string UserAgent => user_agent_in;

        public int HeaderCount => header_count_in;

        public int ContentLength => content_length_in;

        public string ContentType => content_type_in;

        public Stream ContentStream => content_stream_in;

        public string ContentString => content_string_in;

        public byte[] ContentBytes => content_bytes_in;

       
    }
}
