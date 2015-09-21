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
using System.Net.Sockets;

namespace SharpSQL.Server.Packets
{
    internal abstract class Packet
    {
        protected readonly NetworkStream stream;

        private int type;

        /// <summary>
        /// TDS header size
        /// </summary>
        protected const int HEADER_SIZE = 8;
        /// <summary>
        /// TDS Query packet
        /// </summary>
        internal const byte QUERY_PACKET = 1;
        
        /// <summary>
        /// TDS Remote Procedure Call
        /// </summary>
        internal const byte RPC_PACKET = 3;

        /// <summary>
        /// TDS Reply packet
        /// </summary>
        internal const byte REPLY_PACKET = 4;

        /// <summary>
        /// TDS Cancel packet
        /// </summary>
        internal const byte CANCEL_PACKET = 6;

        /// <summary>
        /// TDS MSDTC packet
        /// </summary>
        internal const byte MSDTC_PACKET = 14;

        /// <summary>
        /// TDS Login packet
        /// </summary>
        internal const byte LOGIN_PACKET = 16;

        /// <summary>
        /// TDS NTLM Authentication packet
        /// </summary>
        internal const byte NTLMAUTH_PACKET = 17;

        /// <summary>
        /// TDS negotiation packet
        /// </summary>
        internal const byte NEGOT_PACKET = 18;

        protected Packet(NetworkStream stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// Descript the TDS packet type.
        /// </summary>
        internal int Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }
    }
}
