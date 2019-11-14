using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net; // For IPAddress
using System.Net.Sockets; // For TcpListener, TcpClient
using System.Threading;

namespace MyWebServer
{
    class HTTPhelper
    {
        public Dictionary <int,string> HTTPStatusCodes = new Dictionary<int,string>();
        public HTTPhelper()
        {
            try //Read a bunch of possible HTTP-Statuses from a file, ready for use :)
            {
                string httpath = System.AppDomain.CurrentDomain.BaseDirectory;
                if (httpath.Contains("deploy"))
                    httpath += "/../Libs/HTTPStatuses.txt";
                else
                    httpath += "/SWE1-CS_Nils/Libs/HTTPStatuses.txt";

                using (StreamReader FileReader = new StreamReader(httpath))
                {
                    String line;
                    while (!(line = FileReader.ReadLine()).Contains("[END]"))
                    {
                        if (!(line.Contains("[")) && !(line.Contains("]")) && line != "")
                        {
                            HTTPStatusCodes.Add(Int32.Parse(line.Substring(0, 3)), line.Substring(4));
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("ERROR openining HTTPStatus-file: " + exc);
            }
        }
    }
}
