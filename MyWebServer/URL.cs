using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;

namespace MyWebServer
{
    /// <summary>
    /// The Class for the URL
    /// </summary>
    public class Url : IUrl
    {
        Dictionary<String, String> parameterDictionary = new Dictionary<string, string>();
        string rawURL, path, trimmed_path, fragment, filename, extension;
        String[] parameters, segments;

        /// <summary>
        /// Constructor, does not do anything 
        /// </summary>
        public Url()
        {

        }

        /// <summary>
        /// Constructor with a given String, splits all the Parameters, Segments, and Substrings
        /// </summary>
        public Url(string raw)
        {
            rawURL = raw;
            try
            {
                path = raw.Substring(raw.IndexOf("/"));
            }
            catch (NullReferenceException)
            {
                path = "";
            }
            catch (ArgumentOutOfRangeException) //not good practice
            {
                path = raw;
            }

            if (path.Contains("?"))
                trimmed_path = path.Substring(0, path.IndexOf("?"));
            else
                trimmed_path = path;

            if (path.Contains("?"))
            {
                string splitString = raw.Substring(raw.IndexOf("?"));
                parameters = splitString.Split('&');

                if (parameters.Length > 0)
                {
                    parameters[0] = parameters[0].Substring(1); //remove question mark from first parameter
                    foreach (String parameter in parameters)
                    {
                        if (parameter.Contains("="))
                        {
                            String[] parameter_nameXvalue = parameter.Split('='); //name=value (in URL)
                            //Console.WriteLine("parameter found: " + parameter_nameXvalue[0]);
                            parameterDictionary.Add(parameter_nameXvalue[0], parameter_nameXvalue[1]);
                        }
                    }

                    /*
                     foreach(KeyValuePair<string,string> pair in parameterDictionary)
                     Console.WriteLine(pair.Key + "<-->" + pair.Value);
                    */
                }
            }

            if (path.Contains("/"))
            {
                segments = path.Split('/');
                segments = (segments.Skip(1)).ToArray();
            }

            if (path.Contains("#"))
            {
                fragment = path.Substring(path.IndexOf("#")+1);
                trimmed_path = path.Substring(0, path.IndexOf("#" + fragment));
            }
            
            if(segments != null && segments.Length != 0 && segments.Last().Contains("."))
            {
                filename = segments.Last();
                extension = filename.Substring(filename.IndexOf("."));
            }
            else
            {
                filename = "";
                extension = "";
            }
        }

        /// <summary>
        /// Gets the Parameters of the URL
        /// </summary>
        public IDictionary<string, string> Parameter
        {
            get { return parameterDictionary; }
        }

        /// <summary>
        /// Gets the Amount of Parameters included in the URL
        /// </summary>
        public int ParameterCount
        {
            get { return parameterDictionary.Count; }
        }

        /// <summary>
        /// Returns the path of the URL
        /// </summary>
        public string Path
        {
            get { return trimmed_path; }
        }

        /// <summary>
        /// Returns the Raw URL as given
        /// </summary>
        public string RawUrl
        {
            get { return rawURL; }
        }

        /// <summary>
        /// Returns the URL Extension
        /// </summary>
        public string Extension
        {
            get { return extension; }
        }

        /// <summary>
        /// Returns a given Filename in the URL
        /// </summary>
        public string FileName
        {
            get { return filename; }
        }

        /// <summary>
        /// returns Fragment of the URL
        /// </summary>
        public string Fragment
        {
            get { return fragment; }
        }

        /// <summary>
        /// Returns all Segments of the URL
        /// </summary>
        public string[] Segments
        {
            get { return segments; }
        }
    }
}
