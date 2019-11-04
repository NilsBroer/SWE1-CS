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
        Stream content_in;

        string[] valid_methods = { "POST", "GET", "PUT", "PATCH", "DELETE" };

        public Request(System.IO.Stream stream_in)
        {
            StreamReader stream = new StreamReader(stream_in);
            String line;
            while ((line = stream.ReadLine()) != "")
            {
                if (line.Contains("HTTP"))
                {
                    method_in = line.Substring(0, line.IndexOf(" ")).ToUpper();

                    url_str = line.Substring(method_in.Length + 1, line.Length - (method_in.Length + " HTTP/1.1".Length + 1));
                    header_count_in++;

                    url_in = new Url(url_str);

                    if(valid_methods.Contains(method_in) && method_in != "" && url_str != "")
                        isValid_in = true;
                }

                if (line.Contains("User-Agent"))
                {
                    user_agent_in = line.Substring("User-Agent: ".Length);
                    header_count_in++;
                }

                if (line.Contains("Content-Type"))
                {
                    content_type_in = line.Substring("Content-Type: ".Length);
                    header_count_in++;
                }

                if (line.Contains("Content-Length"))
                {
                    content_length_in = Int32.Parse(line.Substring("Content-Length: ".Length));
                    header_count_in++;
                }
            }
            /*
            Stream content = stream_in;
            content.Position = stream.posit

            while ((line = stream.ReadLine()) != "")
            {
                content.Write(line);
            }
            */
        }

        public String toString()
        {
            String returnValue = "Request Method: " + this.method_in + "\n";
            returnValue = returnValue + "URL: " + this.url_in + "\n";
            returnValue = returnValue + "Valid: " + (this.isValid_in ? "Yes" : "No") + "\n";
            returnValue = returnValue + "User-Agent: " + this.user_agent_in + "\n";
            returnValue = returnValue + "Content-Type: " + this.content_type_in + "\n";
            returnValue = returnValue + "Content-Length: " + this.content_length_in + "\n";
            returnValue = returnValue + "Header-Count: " + this.header_count_in + "\n";
            return returnValue;
        }

        public bool IsValid => isValid_in;

        public string Method => method_in;

        public IUrl Url => url_in;

        public IDictionary<string, string> Headers => throw new NotImplementedException();

        public string UserAgent => user_agent_in;

        public int HeaderCount => header_count_in;

        public int ContentLength => content_length_in;

        public string ContentType => content_type_in;

        public Stream ContentStream => content_in;

        public string ContentString => throw new NotImplementedException();

        public byte[] ContentBytes => throw new NotImplementedException();

       
    }
}
