using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using BIF.SWE1.Interfaces;
using BIF.SWE1;
using System.Text.Json;

namespace MyWebServer.Plugins
{
    public class NavigationPlugin : IPlugin
    {
        private const string Url = "/navi";
        private readonly Dictionary<string, List<string>> _streetCities = new Dictionary<string, List<string>>();
        private bool _loadingMap;
        private readonly Mutex _mtx = new Mutex();

        public float CanHandle(IRequest req)
        {
            if (req.IsValid == false)
                return 0.0f;

            if ((req.Method.Equals("GET") || req.Method.Equals("POST")) && (req.Url.Path.StartsWith("/navigation")))
            {
                return 0.9f;
            }

            return 0.0f;
        }

        public IResponse Handle(IRequest req)
        {
            var resp = new Response();

            if (_loadingMap)
            {
                // Wenn die map gerade geladen wird
                resp.StatusCode = 503;
                resp.SetContent(resp.Status);
                resp.ContentType = resp.ValidContentTypes["txt"];
            }

            else
            {
                if (req.Method == "GET" && req.Url.Parameter.ContainsKey("refresh") && req.Url.Parameter["refresh"] == "1")
                {
                    // Map neuladen weil User wollte
                    Thread t = new Thread(LoadMap);
                    t.Start();
                    resp.StatusCode = 200;
                    resp.SetContent(resp.Status);
                    resp.ContentType = resp.ValidContentTypes["txt"];
                }
                else if (req.Method == "POST" && req.ContentLength > 0)
                {
                    // Alle Städte mit der Straße
                    string key = req.ContentString.Split('=').First();
                    string value = req.ContentString.Split('=').Last();

                    if (key == "street")
                    {
                        if (String.IsNullOrEmpty(value))
                        {
                            resp.SetContent("{\"msg\": \"Bitte geben Sie eine Anfrage ein\"}");
                        }
                        else
                        {
                            int amountOfCities = _streetCities.ContainsKey(value) ? _streetCities[value].Count : 0;
                            resp.SetContent("{\"msg\": \"" + amountOfCities + " Orte gefunden\"}");

                            if (amountOfCities > 0)
                            {
                                var citiesJson = JsonSerializer.Serialize(_streetCities[value]);
                                resp.SetContent("{\"msg\": \"" + amountOfCities + " Orte gefunden\", \"cities\": " + citiesJson + "}");
                            }
                        }

                        resp.StatusCode = 200;
                        resp.ContentType = resp.ValidContentTypes["json"];
                    }
                }
                else
                {
                    resp.StatusCode = 400;
                    resp.SetContent(resp.Status);
                    resp.ContentType = resp.ValidContentTypes["txt"];
                }
            }

            return resp;
        }

        public NavigationPlugin() //Ladet Map wenn gestartet wird (data.osm ausm Discord, die einzige die akzeptable Größe hat)
        {
            Thread t = new Thread(LoadMap);
            t.Start();
        }

        private void LoadMap()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            lock (_mtx) //Mutex für den Thread
            {
                _loadingMap = true;
                Console.WriteLine("Load map...");   //Das führt der Test auch noch aus, aber anscheinend nicht bis zum Ende
                var file = "C:\\Users\\genos\\Documents\\FH-Folder\\3rdSem\\DeppatesSWEProjekt\\SWE_CS\\navigation\\data.osm"; //Hier deinen Pfad(?)
                using (var fs = File.OpenRead(file))
                {
                    var xmlReader = XmlReader.Create(fs, settings);
                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "osm")   //Hier ließt er das Dokument durch und speicher alles
                        {
                            var osm = xmlReader.ReadSubtree();
                            while (osm.Read())
                            {
                                if (osm.NodeType == XmlNodeType.Element && (osm.Name == "node" || osm.Name == "way"))
                                {
                                    string city = "";
                                    string street = "";

                                    using (var element = osm.ReadSubtree())
                                    {
                                        while (element.Read())
                                        {
                                            if (element.NodeType == XmlNodeType.Element && element.Name == "tag")
                                            {
                                                string tagType = element.GetAttribute("k");
                                                string value = element.GetAttribute("v");

                                                if (tagType == "addr:city") 
                                                    city = value;

                                                if (tagType == "addr:street") 
                                                    street = value;
                                            }
                                        }
                                    }

                                    if (city != "")
                                    {
                                        if (_streetCities.ContainsKey(street))
                                        {
                                            if (!_streetCities[street].Contains(city))
                                            {
                                                _streetCities[street].Add(city);
                                            }
                                        }
                                        else
                                        {
                                            _streetCities.Add(street, new List<string> { city });
                                        }
                                    }
                                }
                            }
                            
                        }
                    }
                   
                }

                Console.WriteLine("Finished loading!");
                _loadingMap = false;
            }
        }
    }
}
