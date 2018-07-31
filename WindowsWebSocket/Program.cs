using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket;
using SuperWebSocket;
using WindowsWebSocket;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Common;
using System.Windows.Forms.DataVisualization.Charting;
namespace WindowsWebSocket
{
    class Program
    {
        //public string conn="Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\imihalev\Documents\Visual Studio 2013\WindowsWebSocketServer\WindowsWebSocket\loggerDatabase.mdf";
        public static WebSocketServer wsServer;








        static void Main(string[] args)
        {

            //DateTime time = DateTime.Now;
            //double unix=Math.Round((time - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
            
            //Console.WriteLine("Human read time:{0}",time);
            //Console.WriteLine("Unix:{0}", unix);

            //// Unix timestamp is seconds past epoch
            ////System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            ////dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
           

            //Console.WriteLine("Coonnection to Database error.");
            //Console.ReadLine();
            //return;



            string provider = ConfigurationManager.AppSettings["provider"];
            string connectionString = ConfigurationManager.AppSettings["connectionString"];
            SqlConnection dbCon = new SqlConnection(connectionString);
            dbCon.Open();
            if (dbCon == null)
            {
                Console.WriteLine("Coonnection to Database error.");
                Console.ReadLine();
                return;
            }
            using (dbCon)
            {
                SqlCommand command = new SqlCommand(
                    "SELECT COUNT(*) FROM Logger",dbCon);
                int count = (int)command.ExecuteScalar();
                Console.WriteLine("I have {0} counts.", count);
                if (count > 1)
                {
                    command = new SqlCommand("SELECT * FROM Logger", dbCon);
                    SqlDataReader reader = command.ExecuteReader();
                   
                    Console.WriteLine();
                    using (reader)
                    {
                        while (reader.Read())
                        {
                            int index = (int)reader["Logger_id"];
                            decimal rtcData = (decimal)reader["rtc"];
                            decimal tempData = (decimal)reader["temperature"];
                           
                            Console.WriteLine("Logger Data:{0} -->{1} {2}", index, rtcData, tempData);
                        }
                    }

                    Console.WriteLine();
                    command = new SqlCommand("SELECT TOP(5) * FROM  Logger ORDER BY Logger_id  DESC", dbCon);
                    reader = command.ExecuteReader();
                    
                    using (reader)
                    {
                        while (reader.Read())
                        {
                            int index = (int)reader["Logger_id"];
                            decimal rtcData = (decimal)reader["rtc"];
                            decimal tempData = (decimal)reader["temperature"];
                            Console.WriteLine("Logger Last Data:{0} -->{1} {2}", index, rtcData, tempData);
                        }
                    }

                    Console.WriteLine();



                }
            
                command = dbCon.CreateCommand();
                command.CommandText="INSERT INTO Logger(rtc,temperature) VALUES(@param1,@param2)";

                decimal rtc =(decimal) Math.Round((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
                command.Parameters.Add("@param1", System.Data.SqlDbType.Decimal).Value = rtc;
                command.Parameters.Add("@param2", System.Data.SqlDbType.Float).Value = 30.6;
                command.ExecuteNonQuery();
            }

            //DbProviderFactory factory = DbProviderFactories.GetFactory(provider);
            //using (DbConnection connection = factory.CreateConnection())
            //{
            //    if (connection == null)
            //    {
            //        Console.WriteLine("Coonnection to Database error.");
            //        Console.ReadLine();
            //        return;
            //    }
            //    connection.ConnectionString = connectionString;
            //    connection.Open();
            //}

            wsServer = new WebSocketServer();
            int port = 8080;
            wsServer.Setup(port);
            wsServer.NewSessionConnected += wsServer_NewSessionConnected;
            wsServer.NewMessageReceived += wsServer_NewMessageReceived;
            wsServer.NewDataReceived += wsServer_NewDataReceived;
            wsServer.SessionClosed += wsServer_SessionClosed;
            wsServer.Start();
            Console.WriteLine("Websocket Server was started port:"+port+". Press any key to Exit.");
            Console.ReadKey();
            wsServer.Stop();

        }

        static void wsServer_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            Console.WriteLine("WebSocket Session Closed:" + session.SessionID + ".");
          
        }

        static void wsServer_NewDataReceived(WebSocketSession session, byte[] value)
        {
            Console.WriteLine("New data received...");
        }

        static void wsServer_NewMessageReceived(WebSocketSession session, string value)
        {
            Console.WriteLine("New data Message received:"+"<"+value+">");
            if (value == "Hello")
            {
                session.Send("I am here !.");
            }
        }

        static void wsServer_NewSessionConnected(WebSocketSession session)
        {
            Console.WriteLine("New Session Connected:"+session.SessionID+".");
            Console.WriteLine("Curent token:" + session.CurrentToken);
        }
    }
}
