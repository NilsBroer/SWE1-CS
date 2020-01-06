using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Web;
using System.Threading;
using System.Text;

using BIF.SWE1.Interfaces;

namespace MyWebServer.Plugins
{
    /// <summary>
    /// Navigation Plugin takes a Street and reads an OSM Map to search for matching cities
    /// </summary>
    [marked]
    public class NavigationPlugin : IPlugin
    {
        //cities sind KEYS und Straßen VALUES
        private static Dictionary<string, List<string>> _ParsedDict;
        private static Dictionary<string, List<string>> _NewDict;

        private static SoftLock _MapParserMutex = new SoftLock();
        private static Object _LockObject = new Object();        
        
        private static string _OsmPath = @"C:\Users\genos\Documents\FH-Folder\3rdSem\DeppatesSWEProjekt\SWE_CS\SWE1-CS\Libs\";
        //private static string _OsmPath = @"C:\Users\genos\Documents\FH-Folder\3rdSem\DeppatesSWEProjekt\SWE_CS\SWE1-CS\Libs\"; //Your Hard-Link goes here for now, Chris - Nicht vergessen meinen auszukommentieren
        //private static string _OsmPath = @"C:\ProgramData\SWE01\"; //use this one for running tests, to exclude the read of osm-file(s) for runtime-reasons

        /// <summary>
        /// Constructor, also creates path for the OSM files
        /// </summary>
        public NavigationPlugin()
        {
            Directory.CreateDirectory(_OsmPath); //Machma diese Folder, wenn gibt noch nicht
        }

        /// <summary>
        /// Checks whether the Plugin handles a specific Request
        /// </summary>
        public float CanHandle(IRequest req)
        {
            if (req.Url.Path.Contains(this.GetUrl()))
            {
                return 1.0f;
            }
            return 0.1f;
        }

        /// <summary>
        /// Handles a healthy request, can save the map in the program's memory or parse through the OSM file to find cities
        /// </summary>
        public IResponse Handle(IRequest req)
        {
            var rsp = new Response();
            rsp.StatusCode = 200;

            string searchStreet = string.Empty;

            StringBuilder content = new StringBuilder();

            content.Append("<!DOCTYPE html><html><head><meta charset=\"UTF-8\"></head><body><form method=\"POST\" action=\"/navigation/\"><textarea name=\"street\"></textarea><br /><input type=\"submit\" /></form><hr />");

            if (req.Url.Parameter.ContainsKey("street")) //url-call
            {
                req.Url.Parameter.TryGetValue("street", out searchStreet);
            }

            else if (req.ContentString.StartsWith("street=") && req.ContentString.Length != "street=".Length) //unit-test or content-call
            {
                searchStreet = HttpUtility.UrlDecode(req.ContentString.Substring(7));
            }

            else if (req.Method.Equals("POST") && ((Request)req).GetPostVal("street") != null && ((Request)req).GetPostVal("street").Length > 0)
            {
                searchStreet = ((Request)req).GetPostVal("street");
                searchStreet.Replace("<", "&lt;");
                searchStreet.Replace(">", "&gt;");
            }

            else if (!(req.Url.ParameterCount >= 1 && req.Url.Parameter.ContainsKey("Update") && req.Url.Parameter["Update"] == "true")) //No
            {
                content.Append("Bitte geben Sie eine Anfrage ein");

                rsp.SetContent(content.ToString());
                return rsp;
            }

            List<string> result = new List<string>();

            //Gibts die Map schon im speicher?
            if (_ParsedDict != null && (req.Url.ParameterCount == 0 || (req.Url.ParameterCount == 1 && req.Url.Parameter.ContainsKey("Update") && req.Url.Parameter["Update"] == "false")))
            {
                lock (_LockObject)
                {
                    if (_ParsedDict.ContainsKey(searchStreet.ToLower()))
                    {
                        result = _ParsedDict[searchStreet.ToLower()];
                    }
                }
            }

            // Na? Hol ma aus File //fixed: findet das file :) LG Nils
            else if (_MapParserMutex.TryWait()) // Wait macht dass er sich die ressource krallt, wenn sie besetzt ist gibt es false und wir kommen in das andere else
            {
                // Wenn in der URL Update stand, muss man map neu machen
                if (req.Url.ParameterCount == 1 && req.Url.Parameter.ContainsKey("Update") && req.Url.Parameter["Update"] == "true")
                {
                    List<string> res = ReadWholeFile(saveAll: true);
                    _MapParserMutex.Release();

                    XElement xmlEles = new XElement("div", "Erfolgreiches Update");
                    if (res != null && res.Count > 0)
                    {
                        xmlEles = new XElement("div", res[0]);
                    }
                    rsp.SetContent(xmlEles.ToString());
                    return rsp;
                }

                // Straße
                else if (_ParsedDict == null)
                {
                    result = ReadWholeFile(searchStreet: searchStreet);
                }

                _MapParserMutex.Release();
            }
            
            // Parsen von Map
            else
            {
                Response resp = new Response();
                resp.StatusCode = 503;
                resp.SetContent("Currently Parsing Map, please try again shortly!");
                return resp;
            }

            // Daten erfolgreich geparsed, antworten
            XElement xmlElements = new XElement("div", result.Count + " Orte gefunden");
            if (result.Count > 0)
            {
                xmlElements.Add(new XElement("ul", result.Select(i => new XElement("li", i))));
            }

            content.Append("</body></html>");

            //if (!(result.Contains("No OSM Files found.")))
            rsp.SetContent(xmlElements.ToString());
            //else
                //rsp.SetContent("No OSM File(s) found.");

            return rsp;
        }

        private List<string> ReadWholeFile(bool saveAll = false, string searchStreet = null)
        {
            if (saveAll)
            {
                if (_NewDict == null)
                {
                    _NewDict = new Dictionary<string, List<string>>();
                }
                _NewDict.Clear();
            }

            List<string> result = new List<string>();

            try // Black magic or something, I mean WTF
            {
                foreach (string file in Directory.GetFiles(_OsmPath).Where(x => x.EndsWith(".osm")))
                {
                    using (var fs = File.OpenRead(file))
                    using (var xml = new System.Xml.XmlTextReader(fs))
                    {
                        while (xml.Read())
                        {
                            if (xml.NodeType == System.Xml.XmlNodeType.Element && xml.Name == "osm")
                            {
                                ReadOsm(xml, saveAll, searchStreet).Where(x => !result.Any(y => x == y)).ToList().ForEach(x => result.Add(x));
                            }
                        }
                    }
                }
            }
            catch (FileNotFoundException)   //Das bekommst du nicht wenn du Ordner richtig erstellt hast
            {
                Console.WriteLine("The OpenStreetMap has not been found (" + _OsmPath + ")");
            }

            // In den Programm Speicher ziehen
            if (saveAll)
            {
                lock (_LockObject)
                {
                    if (_ParsedDict == null)
                    {
                        _ParsedDict = new Dictionary<string, List<string>>();
                    }
                    _ParsedDict.Clear();

                    // Copies all the data to the _ParsedDict
                    foreach (var pair in _NewDict)
                    {
                        _ParsedDict.Add(pair.Key, new List<string>());

                        foreach (var item in pair.Value)
                        {
                            _ParsedDict[pair.Key].Add(item);
                        }
                    }
                }
            }

            if (Directory.GetFiles(_OsmPath).Count() == 0)
            {
                return new List<string>(new[] { "No OSM Files found." });
            }

            return result;
        }

        // Die Funktionen sind aus den Folien, nicht anfassen!

        private List<string> ReadOsm(System.Xml.XmlTextReader xml, bool saveAll, string searchStreet)
        {
            List<string> cities = new List<string>();

            using (var osm = xml.ReadSubtree())
            {
                while (osm.Read())
                {
                    if (osm.NodeType == System.Xml.XmlNodeType.Element && (osm.Name == "node" || osm.Name == "way"))
                    {
                        ReadAnyOsmElement(osm, saveAll, ref cities, searchStreet);
                    }
                }
            }

            return cities;
        }

        private List<string> ReadAnyOsmElement(System.Xml.XmlReader osm, bool saveAll, ref List<string> cities, string searchStreet)
        {
            using (var element = osm.ReadSubtree())
            {
                string street = null, city = null, postcode = null;
                while (element.Read())
                {
                    if (element.NodeType == System.Xml.XmlNodeType.Element
                    && element.Name == "tag")
                    {
                        switch (element.GetAttribute("k"))
                        {
                            case "addr:city":
                                city = element.GetAttribute("v");
                                break;
                            case "addr:postcode":
                                postcode = element.GetAttribute("v");
                                break;
                            case "addr:street":
                                street = element.GetAttribute("v");
                                break;
                        }
                    }
                }

                //if (postcode != null && city != null)
                //    city = city + ", " + postcode;
                if (!string.IsNullOrEmpty(street) && !string.IsNullOrEmpty(city))
                {
                    if (saveAll)
                    {

                        if (_NewDict.ContainsKey(street.ToLower()) && !_NewDict[street.ToLower()].Contains(city))
                        {
                            _NewDict[street.ToLower()].Add(city);
                        }
                        else if (!_NewDict.ContainsKey(street.ToLower()))
                        {
                            _NewDict.Add(street.ToLower(), new List<string>(new[] { city }));
                        }
                    }
                    else
                    {
                        if (!cities.Contains(city) && street.ToLower() == searchStreet.ToLower())
                        {
                            cities.Add(city);
                        }
                    }
                }
            }

            return cities;
        }

        /// <summary>
        /// Format of the Plugin's accepted URL
        /// </summary>
        public string GetUrl()
        {
            return "/navigation";
        }
    }
}
