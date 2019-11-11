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
        string rawURL, path, path_in, fragment;
        String[] segment;
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

            if (path.Contains("?"))
                path_in = path.Substring(0, path.IndexOf("?"));
            else
                path_in = path;

            if (path.Contains("?"))
            {
                string parameterString = raw.Substring(raw.IndexOf("?"));
                String[] parameters = parameterString.Split('&');

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
                }
            }

            if (path.Contains("/"))
            {
                String[] parameters = path.Substring(1).Split('/');
                segment = parameters;
            }

            if (path.Contains("#"))
            {
                String[] parameters = path.Split('#');
                path_in = parameters[0];
                fragment = parameters[1];
            }
            /*
            foreach(KeyValuePair<string,string> pair in parameterDictionary)
                Console.WriteLine(pair.Key + "<-->" + pair.Value);
            */
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
            get { return path_in; }
        }

        public string RawUrl
        {
            get { return rawURL; }
        }

        public string Extension
        {
            get { throw new NotImplementedException(); }
        }

        public string FileName
        {
            get { throw new NotImplementedException(); }
        }

        public string Fragment
        {
            get { return fragment; }
        }

        public string[] Segments
        {
            get { return segment; }
        }
    }
}
