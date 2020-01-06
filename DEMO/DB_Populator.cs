using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace DEMO
{
    public class DB_Populator
    {
        public DB_Populator()
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = \"C:\\Users\\Nsync\\Google Drive\\UNI\\Semester 03\\SWE_CS\\SWE1-CS\\MyWebServer\\Database\\Database.mdf\"; Integrated Security = True;"; //Look-up in Server Explorer

            //Console.WriteLine("Connection created, path: "+myConnection.ConnectionString);
            myConnection.Open();
            Console.WriteLine("Succesfully connected to database.");

            Random rndm = new Random();
            var timestamp = (DateTime.Now).AddYears(-10);
            var currenttime = DateTime.Now;

            SqlCommand myCommand = new SqlCommand(null, myConnection);
            myCommand.CommandText =
                "INSERT INTO Temperature ([Time-Stamp], Value, isAutoGenerate) " +
                "VALUES (@dt, @val, @iAG)";
            SqlParameter dtParam, valParam, iAGParam;
            dtParam = new SqlParameter("@dt", System.Data.SqlDbType.SmallDateTime);
            valParam = new SqlParameter("@val", System.Data.SqlDbType.Float);
            iAGParam = new SqlParameter("@iAG", System.Data.SqlDbType.Bit);

            //dtParam.Value = null; //template
            //valParam.Value = null; //template
            iAGParam.Value = 1; //template & actual use

            myCommand.Parameters.Add(dtParam);
            myCommand.Parameters.Add(valParam);
            myCommand.Parameters.Add(iAGParam);
            myCommand.Prepare(); //initialize template, do NOT execute or null-values will be safed into DB

            for (; DateTime.Compare(currenttime, timestamp) > 0; timestamp = timestamp.AddHours(8)) //compare: current time ">" than timestamp --> returns 1
            {
                dtParam.Value = timestamp; //use template
                double temp = rndm.NextDouble() * 40;
                valParam.Value = Math.Round(temp, 2); //use template

                myCommand.ExecuteNonQuery(); //EXECUTE
                Console.WriteLine("Entry: [" + dtParam.Value + "][" + valParam.Value + "]"); //time-intensive
            }
            Console.WriteLine($"Populating data took {sw.ElapsedMilliseconds / 1000m:n2} s");
        }

        public DB_Populator(int entries)
        {
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = \"C:\\Users\\Nsync\\Google Drive\\UNI\\Semester 03\\SWE_CS\\SWE1-CS\\MyWebServer\\Database\\Database.mdf\"; Integrated Security = True;"; //Look-up in Server Explorer

            //Console.WriteLine("Connection created, path: "+myConnection.ConnectionString);
            myConnection.Open();
            //Console.WriteLine("Succesfully connected to database.");

            Random rndm = new Random();

            SqlCommand myCommand = new SqlCommand(null, myConnection);
            myCommand.CommandText =
                "INSERT INTO Temperature ([Time-Stamp], Value, isAutoGenerate) " +
                "VALUES (@dt, @val, @iAG)";
            SqlParameter dtParam, valParam, iAGParam;
            dtParam = new SqlParameter("@dt", System.Data.SqlDbType.SmallDateTime);
            valParam = new SqlParameter("@val", System.Data.SqlDbType.Float);
            iAGParam = new SqlParameter("@iAG", System.Data.SqlDbType.Bit);

            //dtParam.Value = null; //template
            //valParam.Value = null; //template
            iAGParam.Value = 0; //template & actual use

            myCommand.Parameters.Add(dtParam);
            myCommand.Parameters.Add(valParam);
            myCommand.Parameters.Add(iAGParam);
            myCommand.Prepare(); //initialize template, do NOT execute or null-values will be safed into DB

            for (int i = 0; i < entries; i++) //compare: current time ">" than timestamp --> returns 1
            {
                dtParam.Value = DateTime.Now; //use template
                double temp = rndm.NextDouble() * 40;
                valParam.Value = Math.Round(temp, 2); //use template

                myCommand.ExecuteNonQuery(); //EXECUTE
                Console.WriteLine("Entry: [" + dtParam.Value + "][" + valParam.Value + "]");
            }
        }
    }
}
