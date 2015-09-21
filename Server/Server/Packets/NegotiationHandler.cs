//
// (C) 2008 The SharpSQL Project Team (http://sharpsql.sourceforge.net)
//
// Authors:
//	Volker Berlin <smallsql@sourceforge.net>
//
// Licensed under the terms of the GNU GPL v3,
//  with Classpath Linking Exception for Libraries
//

using System.Collections.Generic;
using System.IO;
using System;

namespace SharpSQL.Server.Packets
{
    /// <summary>
    /// Handle the reading and writing of the negotiations packets.
    /// The negotiation packet will be send from client and the server answer with
    /// same packet. The packet include a list of records and the data of the records.
    /// Every record has a size of 5 bytes (type, data offset, data length). After the last
    /// record come a byte 0xFF. After this byte the data come.
    /// </summary>
    internal class NegotiationHandler
    {
        /// <summary>
        /// Hold the readed records.
        /// </summary>
        private readonly List<Record> records = new List<Record>();

        private const int VERSION_RECORD = 0;
        private const int SSL_RECORD = 1;
        private const int INSTANCENAME_RECORD = 2;
        private const int PROCESSID_RECORD = 3;

        /// <summary>
        /// SSL Mode - Login packet must be encrypted
        /// </summary>
        internal const int SSL_ENCRYPT_LOGIN = 0;
        /// <summary>
        /// SSL Mode - Client requested force encryption.
        /// </summary>
        internal const int SSL_CLIENT_FORCE_ENCRYPT = 1;
        /// <summary>
        /// SSL Mode - No server certificate installed.
        /// </summary>
        internal const int SSL_NO_ENCRYPT = 2;
        /// <summary>
        /// SSL Mode - Server requested force encryption.
        /// </summary>
        internal const int SSL_SERVER_FORCE_ENCRYPT = 3;

        /// <summary>
        /// Read a negotiation packet from the client.
        /// </summary>
        /// <param name="packet">the reading packet</param>
        internal NegotiationHandler(ReadPacket packet)
        {
            int offset = 0;
            int recordType;
            while ((recordType = packet.ReadUnsignedByte()) != 0xFF)
            {
                records.Add(new Record(recordType, packet.ReadBeShort(), packet.ReadBeShort()));
                offset += 5;
            }
            offset++; // the ending mark 0xFF

            for (int i = 0; i < records.Count; i++)
            {
                offset = records[i].ReadData(offset, packet);
            }
        }

        internal int SslMode
        {
            get
            {
                return findRecord(SSL_RECORD).ByteValue;
            }
            set
            {
                findRecord(SSL_RECORD).ByteValue = value;
            }
        }

        /// <summary>
        /// Answer to the client.
        /// </summary>
        /// <param name="packet"></param>
        internal void Answer(WritePacket packet)
        {
            packet.Init();
            packet.Type = Packet.NEGOT_PACKET;
            for (int i = 0; i < records.Count; i++)
            {
                Record record = records[i];
                packet.WriteByte(record.type);
                packet.WriteBeShort(record.offset);
                packet.WriteBeShort(record.size);
            }
            packet.WriteByte(0xFF);
            for (int i = 0; i < records.Count; i++)
            {
                Record record = records[i];
                packet.WriteBytes(record.data);
            }
            packet.Send();
        }

        private Record findRecord(int recordType)
        {
            foreach (Record record in records)
            {
                if (record.type == recordType)
                {
                    return record;
                }
            }
            return null;
        }

        /// <summary>
        /// Hold a the data of a single record.
        /// </summary>
        class Record
        {
            internal int type;
            internal int offset;
            internal int size;
            internal byte[] data;

            internal Record(int type, int offset, int size)
            {
                this.type = type;
                this.offset = offset;
                this.size = size;
            }

            /// <summary>
            /// Read the data and check if the offset is correct.
            /// </summary>
            /// <param name="offset">the desired offset</param>
            /// <param name="packet">the reading packet</param>
            /// <returns>The new offset after the data</returns>
            internal int ReadData(int offset, ReadPacket packet)
            {
                if (offset != this.offset)
                {
                    throw new IOException("Invalid Record Offset");
                }
                data = packet.ReadBytes(size);
                return offset + size;
            }

            internal int ByteValue
            {
                set
                {
                    if (data != null && data.Length > 0)
                    {
                        data[0] = (byte)value;
                    }
                }
                get
                {
                    if (data != null && data.Length > 0)
                    {
                        return data[0];
                    }
                    return 0;
                }
            }
        }
    }

}
