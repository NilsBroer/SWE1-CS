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
        public Dictionary <int,string>HTTPStatusCodes = new Dictionary<int,string>();
        public HTTPhelper(bool printcodes = false)
        {
            try //Read a bunch of possible HTTP-Statuses from a file, ready for use :)
            {
                using (StreamReader FileReader = new StreamReader("../Libs/HTTPStatuses.txt"))
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

                if (printcodes)
                {
                    foreach (KeyValuePair<int, string> pair in HTTPStatusCodes)
                        Console.WriteLine("Status: {0}, which means \"{1}\" ", pair.Key, pair.Value);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("ERROR openining HTTPStatus-file: " + exc);
            }
        }
    }
}
