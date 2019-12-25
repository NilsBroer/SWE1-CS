using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;

namespace MyWebServer
{
    public class Url : IUrl
    {
        Dictionary<String, String> parameterDictionary = new Dictionary<string, string>();
        string rawURL, path, trimmed_path, fragment, filename, extension;
        String[] parameters, segments;
        public Url()
        {

        }

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

        public IDictionary<string, string> Parameter
        {
            get { return parameterDictionary; }
        }

        public int ParameterCount
        {
            get { return parameterDictionary.Count; }
        }

        public string Path
        {
            get { return trimmed_path; }
        }

        public string RawUrl
        {
            get { return rawURL; }
        }

        public string Extension
        {
            get { return extension; }
        }

        public string FileName
        {
            get { return filename; }
        }

        public string Fragment
        {
            get { return fragment; }
        }

        public string[] Segments
        {
            get { return segments; }
        }
    }
}
