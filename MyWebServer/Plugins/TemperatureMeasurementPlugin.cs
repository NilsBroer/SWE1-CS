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
            return $"/temperature?type=regular&from={from.ToString("dd/MM/yyyy HH:mm:ss")}&until={until.ToString("dd/MM/yyyy HH:mm:ss")}";
        }

        public string GetUrl(DateTime day) //Not necessary, template for handling single-day url from indication
        {
            string daystring = day.ToString("yyyy/MM/dd");
            daystring.Replace("-", "/");
            return $"/GetTemperature/{daystring}";
        }

        public string GetRestUrl(DateTime from, DateTime until)
        {
            return $"/temperature?type=rest&from={from.ToString("dd/MM/yyyy HH:mm:ss")}&until={until.ToString("dd/MM/yyyy HH:mm:ss")}";
        }

        public IResponse Handle(IRequest req)
        {
            Response response = new Response();
            string reqtype = "";
            req.Url.Parameter.TryGetValue("type", out reqtype);
            bool is_rest = (reqtype != null && (reqtype.Contains("rest") || reqtype.Contains("restles") || req.Url.Segments.Contains("GetTemperature"))); // == instead of contains also possible, GetTemperature is apparently rest, by indication

            if (this.CanHandle(req) > 0.1f && !is_rest)
            {
                SqlConnection myConnection = new SqlConnection();
                //TODO: Change so it also works for Chris
                myConnection.ConnectionString = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = \"C:\\Users\\Nsync\\Google Drive\\UNI\\Semester 03\\SWE_CS\\SWE1-CS\\MyWebServer\\Database\\Database.mdf\"; Integrated Security = True;"; //Look-up in Server Explorer

                Console.WriteLine("Connection created, path: " + myConnection.ConnectionString);
                myConnection.Open();
                Console.WriteLine("Connected...");

                string fromString, untilString;
                DateTime from, until;
                bool from_is_set = false;

                if (from_is_set = req.Url.Parameter.TryGetValue("from", out fromString))
                    from = Convert.ToDateTime(fromString); //can fail, try-catch?
                else
                    from = DateTime.Now.AddDays(-365*10); //max from (roughly) //restrict to segment /all ?
                if (req.Url.Parameter.TryGetValue("until", out untilString))
                    until = Convert.ToDateTime(untilString); //can fail, try-catch?
                else
                    until = DateTime.Now; //max until

                SqlCommand myCommand = new SqlCommand(null, myConnection);
                
                myCommand.CommandText = $"SELECT [Time-Stamp], Value FROM Temperature WHERE [Time-Stamp] between @from and @until;";
                myCommand.Parameters.AddWithValue("@from", from.ToString()); //check
                myCommand.Parameters.AddWithValue("@until", until.ToString()); //check

                //Create base for flippable HTML-table
                string HTML_begin = @"
                        <!DOCTYPE html>   
                        <html lang=""en"">   
                        <head>   
	                        <meta charset=""utf-8"">   
	                        <title>Temperatures</title>   
	                        <meta name=""description"" content=""Bootstrap."">  
	                        <link href=""http://maxcdn.bootstrapcdn.com/bootstrap/3.2.0/css/bootstrap.min.css"" rel=""stylesheet"">   
	                        <script src=""http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js""></script>
	                        <link rel=""stylesheet"" href=""http://cdn.datatables.net/1.10.2/css/jquery.dataTables.min.css""></style>
	                        <script type=""text/javascript"" src=""http://cdn.datatables.net/1.10.2/js/jquery.dataTables.min.js""></script>
	                        <script type=""text/javascript"" src=""http://maxcdn.bootstrapcdn.com/bootstrap/3.2.0/js/bootstrap.min.js""></script>
                        </head>  
                        <body style=""margin:20px auto"">  
	                        <div class=""container"">
		                        <div class=""row header"" style=""text-align:center;color:green"">
			                        <h3>Measured Temperatures</h3>
		                        </div>
	                        <table id=""myTable"" class=""table table-striped"">
                    ";
                string HTML_end = @"
                        	</table>  
	                        </div>
                        </body>  

                        <script>
                        	$(document).ready(function(){ $('#myTable').dataTable(); });
                        </script>
                        </html>
                    ";

                StringBuilder content = new StringBuilder();
                SqlDataReader reader = myCommand.ExecuteReader();
                content.Append(HTML_begin);
                content.Append("<thead><tr><th>Time-Stampe</th><th>Temperature</th></tr></thead> <tbody>");
                try
                {
                    while (reader.Read())
                    {
                        content.Append(String.Format("<tr><td>{0}</td><td>{1}</td></tr>",
                        reader["Time-Stamp"], reader["Value"]));
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
                content.Append(HTML_end);
                content.Append("</tbody> <tfoot><tr><th>Time-Stampe</th><th>Temperature</th></tr></tfoot>");

                response.SetContent(content.ToString());
                response.ContentType = "text/html";
                //response.AddHeader();

                //if everything is ok (no internal errors)
                response.StatusCode = 200;
            }
            else if(this.CanHandle(req) > 0.1f && is_rest)
            {
                //TODO: implement rest-handle
                //TODO: implement rest-handle for single day query from indication
                response.SetContent("REST-XML goes here");
                response.ContentType = "text/xml";
                //response.AddHeader();

                response.StatusCode = 200;
            }
            else
                response.StatusCode = 500; //or 400
            return response;
        }
    }
}
