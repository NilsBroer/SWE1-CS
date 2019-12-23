using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BIF.SWE1.Interfaces;
using System.Data.SqlClient;

namespace MyWebServer.Plugins
{
    class TemperatureMeasurementPlugin : IPlugin
    {
        public float CanHandle(IRequest req)
        {
            if ((req.Url.Path).ToLower().StartsWith("/temperature") || (req.Url.Path).ToLower().StartsWith("//gettemperature")) //why // and not /
            {
                return 1.0f;
            }
            else
                return 0.0f;
        }

        public string GetUrl()
        {
            return "/temperature?type=type&from='dd/MM/yyyy HH:mm:ss'&until='dd/MM/yyyy HH:mm:ss'";
        }

        public string GetUrl (DateTime from, DateTime until)
        {
            return $"/temperature?type=regular&from='${from.ToString("dd/MM/yyyy HH:mm:ss")}'&until='{until.ToString("dd/MM/yyyy HH:mm:ss")}'";
        }

        public string GetUrl(DateTime day) //Not necessary, template for handling single-day url from indication
        {
            string daystring = day.ToString("yyyy/MM/dd");
            daystring.Replace("-", "/");
            return $"/GetTemperature/{daystring}";
        }

        public string GetRestUrl(DateTime from, DateTime until)
        {
            return $"/temperature?type=rest&from='${from.ToString("dd/MM/yyyy HH:mm:ss")}'&until='{until.ToString("dd/MM/yyyy HH:mm:ss")}'";
        }

        public IResponse Handle(IRequest req)
        {
            Response response = new Response();
            if (this.CanHandle(req) > 0.1f)
            {
                SqlConnection myConnection = new SqlConnection();
                //Change so it also works for Chris
                myConnection.ConnectionString = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = \"C:\\Users\\Nsync\\Google Drive\\UNI\\Semester 03\\SWE_CS\\SWE1-CS\\MyWebServer\\Database\\Database.mdf\"; Integrated Security = True;"; //Look-up in Server Explorer

                Console.WriteLine("Connection created, path: " + myConnection.ConnectionString);
                myConnection.Open();
                Console.WriteLine("Connected...");

                //TODO: Get actual values from request-url
                DateTime until = DateTime.Now;
                DateTime from = DateTime.Now.AddDays(-500);

                SqlCommand myCommand = new SqlCommand(null, myConnection);
                
                //add if-clauses, different commands for different urls (e.g. single day)
                //add content-restriction, restrict amount shown per site, add buttons
                myCommand.CommandText = $"SELECT [Time-Stamp], Value FROM Temperature WHERE [Time-Stamp] between @from and @until;";
                myCommand.Parameters.AddWithValue("@from", from.ToString()); //check
                myCommand.Parameters.AddWithValue("@until", until.ToString()); //check

                SqlDataReader reader = myCommand.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(String.Format("{0}, {1}",
                        reader["Time-Stamp"], reader["Value"])); //Works for testing :) but Replace with set content
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }


                //if everything is ok (no internal errors)
                response.StatusCode = 200;
            }
            else
                response.StatusCode = 500; //or 400
            return response;
        }
    }
}
