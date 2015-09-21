//
// (C) 2008 The SharpSQL Project Team (http://sharpsql.sourceforge.net)
//
// Authors:
//	Volker Berlin <smallsql@sourceforge.net>
//
// Licensed under the terms of the GNU GPL v3,
//  with Classpath Linking Exception for Libraries
//

using System;
using System.IO;
using System.Net.Sockets;
using SharpSQL.Server.Packets;

namespace SharpSQL.Server
{
    internal class Session
    {
        private readonly NetworkStream stream;
        private readonly ReadPacket readPacket;
        private readonly WritePacket writePacket;
        private readonly ReadPacket.CallbackDelegate callback;
        private LoginHandler login;

        internal Session(TcpClient client)
        {
            // Get a stream object for reading and writing
            stream = client.GetStream();

            readPacket = new ReadPacket(stream);
            writePacket = new WritePacket(stream);
            callback = new ReadPacket.CallbackDelegate(HandlePacket);

        }

        internal void Start(){
            readPacket.BeginReceive(callback);
        }

        private void HandlePacket(){
            switch (readPacket.Type)
            {
                case Packet.NEGOT_PACKET:
                    NegotiationHandler negotiation = new NegotiationHandler(readPacket);
                    negotiation.SslMode = NegotiationHandler.SSL_NO_ENCRYPT;
                    negotiation.Answer(writePacket);
                    break;
                case Packet.LOGIN_PACKET:
                    login = new LoginHandler(readPacket);
                    goto default;
                default:
                    Logger.Dump(readPacket.ReadBytes(readPacket.DataAvailable));
                    throw new IOException("Unknown Packet Type:"+readPacket.Type);
            }
            readPacket.BeginReceive(callback);
        }
    }
}
