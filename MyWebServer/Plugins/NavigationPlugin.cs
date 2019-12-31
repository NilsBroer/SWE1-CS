using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Web;
using System.Threading;

using BIF.SWE1.Interfaces;

namespace MyWebServer.Plugins
{
    public class NavigationPlugin : IPlugin
    {
        //cities sind KEYS und Straßen VALUES
        private Dictionary<string, List<string>> _WholeMap;
        private Dictionary<string, List<string>> _NewMap;

        private Mutex _ReadingMutex = new Mutex();
        private Object _CopyLock = new Object();
         
        //private static string _OsmSubDir = "navmaps";   //DIESEN ORDNER erstellen, einmal bei den tests und einmal normal im deploy folder wie immer oder so, kennst dich eh aus ;^)
        
        //private static string _OsmPath = @"C:\Users\Nsync\Google Drive\UNI\Semester 03\SWE_CS\SWE1-CS\Libs\";
        //private static string _OsmPatch = @""; //Your Hard-Link goes here for now, Chris - Nicht vergessen meinen auszukommentieren
        private static string _OsmPath = @"C:\ProgramData\SWE01\"; //use this one for running tests, to exclude the read of osm-file(s) for runtime-reasons

        private const float _CanHandleReturn = 1.0f;
        private const float _CannotHandleReturn = 0.1f;

        public NavigationPlugin()
        {
            Directory.CreateDirectory(_OsmPath); //Machma diese Folder, wenn gibt noch nicht
        }

        public float CanHandle(IRequest req)
        {
            if (req.Url.Path.Contains(this.GetUrl()))
            {
                return _CanHandleReturn;
            }
            return _CannotHandleReturn;
        }

        public IResponse Handle(IRequest req)
        {
            var rsp = new Response();
            rsp.StatusCode = 200;

            string searchStreet = string.Empty;

            // Straße aus der request holen
            {
                if (req.Url.Parameter.ContainsKey("street")) //url-call
                {
                    req.Url.Parameter.TryGetValue("street", out searchStreet);
                }

                else if (req.ContentString.StartsWith("street=") && req.ContentString.Length != "street=".Length) //unit-test or content-call
                {
                    searchStreet = HttpUtility.UrlDecode(req.ContentString.Substring(7));
                }

                else if (!(req.Url.ParameterCount >= 1 && req.Url.Parameter.ContainsKey("Update") && req.Url.Parameter["Update"] == "true")) //No
                {
                    rsp.SetContent("Bitte geben Sie eine Anfrage ein");
                    return rsp;
                }
            }

            List<string> result = new List<string>();

            //Gibts die Map schon im speicher?
            if (_WholeMap != null && (req.Url.ParameterCount == 0 || (req.Url.ParameterCount == 1 && req.Url.Parameter.ContainsKey("Update") && req.Url.Parameter["Update"] == "false")))
            {
                lock (_CopyLock)
                {
                    if (_WholeMap.ContainsKey(searchStreet.ToLower()))
                    {
                        result = _WholeMap[searchStreet.ToLower()];
                    }
                }
            }

            // Na? Hol ma aus File //fixed: findet das file :) LG Nils
            else if (_ReadingMutex.WaitOne()) // Wait macht dass er sich die ressource krallt, wenn sie besetzt ist gibt es false und wir kommen in das andere else
            {
                // Wenn in der URL Update stand, muss man map neu machen
                if (req.Url.ParameterCount == 1 && req.Url.Parameter.ContainsKey("Update") && req.Url.Parameter["Update"] == "true")
                {
                    List<string> res = ReadWholeFile(saveAll: true);
                    _ReadingMutex.ReleaseMutex();

                    XElement xmlEles = new XElement("div", "Erfolgreiches Update");
                    if (res != null && res.Count > 0)
                    {
                        xmlEles = new XElement("div", res[0]);
                    }
                    rsp.SetContent(xmlEles.ToString());
                    return rsp;
                }

                // Straße
                else if (_WholeMap == null)
                {
                    result = ReadWholeFile(searchStreet: searchStreet);
                }

                _ReadingMutex.ReleaseMutex();
            }

            // Parsen von Map
            else
            {
                rsp.SetContent("Das NavigationPlugin kann diese Funktion zurzeit nicht ausführen, sie wird bereits benutzt. Bitte versuchen Sie es später noch einmal.");
                return rsp;
            }

            // Daten erfolgreich geparsed, antworten

            XElement xmlElements = new XElement("div", result.Count + " Orte gefunden");
            if (result.Count > 0)
            {
                xmlElements.Add(new XElement("ul", result.Select(i => new XElement("li", i))));
            }

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
                if (_NewMap == null)
                {
                    _NewMap = new Dictionary<string, List<string>>();
                }
                _NewMap.Clear();
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
                lock (_CopyLock)
                {
                    if (_WholeMap == null)
                    {
                        _WholeMap = new Dictionary<string, List<string>>();
                    }
                    _WholeMap.Clear();

                    // Copies all the data to the _WholeMap
                    foreach (var pair in _NewMap)
                    {
                        _WholeMap.Add(pair.Key, new List<string>());

                        foreach (var item in pair.Value)
                        {
                            _WholeMap[pair.Key].Add(item);
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

                        if (_NewMap.ContainsKey(street.ToLower()) && !_NewMap[street.ToLower()].Contains(city))
                        {
                            _NewMap[street.ToLower()].Add(city);
                        }
                        else if (!_NewMap.ContainsKey(street.ToLower()))
                        {
                            _NewMap.Add(street.ToLower(), new List<string>(new[] { city }));
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

        public string GetUrl()
        {
            return "/navigation";
        }
    }
}
