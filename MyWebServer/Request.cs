﻿using BIF.SWE1.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace MyWebServer
{
    /// <summary>
    /// Defines HTML Request
    /// </summary>
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

        /// <summary>
        /// Constructor, sets the Type and Headers of the Request
        /// </summary>
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
                        //Console.WriteLine("First line: " + line);
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

        private Dictionary<String,String> postPar = new Dictionary<String,String>();

        /// <summary>
        /// Decodes URLs to be used by other Plugins
        /// </summary>
        public static String decodeUrl (String url)
        {
            return HttpUtility.UrlDecode(url, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// Gets the Values from POST Requests
        /// </summary>
        public string GetPostVal (string key)
        {
            if (!this.Method.Equals("POST"))    
                return null;

            if (this.ContentString.Length == 0)
                return null;

            if (postPar.Count == 0)
            {
                String[] parArray = this.ContentString.Split('&');

                foreach (String par in parArray)
                {
                    String[] parPair = par.Split(new char[] { '=' }, 2);
                    postPar.Add(parPair[0], decodeUrl(parPair[1]));
                }
            }

            if (postPar.ContainsKey(key))
                return postPar[key];


            return null;
        }

        /// <summary>
        /// Stringifies the Parts included in the Request to display them
        /// </summary>
        public String toString()
        {
            String returnValue = "Request Method: " + this.method_in + "\n";
            returnValue = returnValue + "URL: " + this.url_in.RawUrl + "\n";
            returnValue = returnValue + "Valid: " + (this.isValid_in ? "Yes" : "No") + "\n";
            returnValue = returnValue + "User-Agent: " + this.user_agent_in + "\n";
            returnValue = returnValue + "Content-Type: " + this.content_type_in + "\n";
            returnValue = returnValue + "Content-Length: " + this.content_length_in + "\n";
            returnValue = returnValue + "Header-Count: " + this.header_count_in + "\n";
            returnValue = returnValue + "Content as..." + "\nStream: " + this.content_stream_in + "\nString: " + this.content_string_in + "\nBytes: " + this.content_bytes_in + "\n";
            
            return returnValue;
        }

        //-----------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the validity of the Request
        /// </summary>
        public bool IsValid => isValid_in;

        /// <summary>
        /// Gets the Method of the Request
        /// </summary>
        public string Method => method_in;

        /// <summary>
        /// Gets the URL of the Request
        /// </summary>
        public IUrl Url => url_in;

        /// <summary>
        /// Gets the Headers of the Request
        /// </summary>
        public IDictionary<string, string> Headers => headers_in;

        /// <summary>
        /// Gets the UserAgent of the Request
        /// </summary>
        public string UserAgent => user_agent_in;

        /// <summary>
        /// Gets the HeaderCount of the Request
        /// </summary>
        public int HeaderCount => header_count_in;

        /// <summary>
        /// Gets the Content Length of the Request
        /// </summary>
        public int ContentLength => content_length_in;

        /// <summary>
        /// Gets the Content Type of the Request
        /// </summary>
        public string ContentType => content_type_in;

        /// <summary>
        /// Gets the Content Stream of the Request
        /// </summary>
        public Stream ContentStream => content_stream_in;

        /// <summary>
        /// Gets the Content of the Request as a String
        /// </summary>
        public string ContentString => content_string_in;

        /// <summary>
        /// Gets the Content of the Request as Bytes
        /// </summary>
        public byte[] ContentBytes => content_bytes_in;

       
    }
}
