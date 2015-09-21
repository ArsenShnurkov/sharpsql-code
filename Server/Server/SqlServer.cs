using System.IO;
//
// (C) 2008 The SharpSQL Project Team (http://sharpsql.sourceforge.net)
//
// Authors:
//	Volker Berlin <smallsql@sourceforge.net>
//
// Licensed under the terms of the GNU GPL v3,
//  with Classpath Linking Exception for Libraries
//

using System.Net;
using System.Net.Sockets;
using System;
using System.Resources;
using System.Reflection;
using SharpSQL.Server.i18n;
using System.Threading;

namespace SharpSQL.Server
{
    public class SqlServer
    {
    	private static TcpListener listener;
    	
        //static ResourceManager resources = new ResourceManager("SharpSQL.Server.i18n.Msg", Assembly.GetExecutingAssembly());
        private static void Main(string[] args)
        {
            Run();
        }

        private static void Run(){
            listener = new TcpListener(IPAddress.Any, 1433);
            listener.Start();

            // Enter the listening loop.
            while (true)
            {
                Logger.Info(Msg.WaitingForConnection);

                // Perform a blocking call to accept requests.
                // You could also user server.AcceptSocket() here.
                TcpClient client = listener.AcceptTcpClient();
                Logger.Info(Msg.ConnectedWith, client.Client.RemoteEndPoint);

                Session session = new Session(client);
                session.Start();
            }
        }

        public static void Start()
        {
            new Thread(new ThreadStart(Run)).Start();
        }

        public static void Stop(){
        	listener.Stop();
        }
    }
}
