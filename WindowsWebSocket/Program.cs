using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket;
using SuperWebSocket;
using WindowsWebSocket;


namespace WindowsWebSocket
{
    class Program
    {
        public static WebSocketServer wsServer;
        static void Main(string[] args)
        {
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
