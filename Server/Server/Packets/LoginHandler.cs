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

namespace SharpSQL.Server.Packets
{
	/// <summary>
	/// Description of LoginPaket.
	/// </summary>
	internal class LoginHandler
	{
        private int networkChunkSize;
        private int processID;
        private int connectionID;
        private String wsid;
        private String user;
        private String password;
        private String application;
        private String server;
        private String program;
        private String language;
        private String database;

		internal LoginHandler(ReadPacket packet)
		{
			int currentPacketSize = packet.ReadInt();
            if (currentPacketSize != packet.DataAvailable + 4)
            {
                throw new IOException("Wrong Login Packet size:" + currentPacketSize);
            }
			int protocolVersion = packet.ReadInt();
            networkChunkSize = packet.ReadInt();
            if (networkChunkSize > 0xFFFF)
            {
                throw new IOException("Network Chunk Size to large:" + networkChunkSize);
            }

            packet.ReadInt(); // Unknown
            processID = packet.ReadInt();
            connectionID = packet.ReadInt();

            // 0x20: enable warning messages if USE <database> issued
            // 0x40: change to initial database must succeed
            // 0x80: enable warning messages if SET LANGUAGE issued
            packet.ReadByte();

            int loginFlags = packet.ReadByte();
            bool isNtlmLogin = (loginFlags & 0x80) > 0;
            if (isNtlmLogin)
            {
                //TODO add support for NTLM
                throw new IOException("NTLM login currently not implemented.");
            }

            packet.ReadByte(); // SQL Type
            packet.ReadByte();
            packet.ReadInt(); // Timezone
            packet.ReadInt(); // Collation


            int wsidOffset = packet.ReadUnsignedShort();
            int wsidLength = packet.ReadUnsignedShort();

            int userOffset = packet.ReadUnsignedShort();
            int userLength = packet.ReadUnsignedShort();

            int passOffset = packet.ReadUnsignedShort();
            int passLength = packet.ReadUnsignedShort();

            int applOffset = packet.ReadUnsignedShort();
            int applLength = packet.ReadUnsignedShort();

            int servOffset = packet.ReadUnsignedShort();
            int servLength = packet.ReadUnsignedShort();

            int unknOffset = packet.ReadUnsignedShort();
            int unknLength = packet.ReadUnsignedShort();

            int progOffset = packet.ReadUnsignedShort();
            int progLength = packet.ReadUnsignedShort();

            int langOffset = packet.ReadUnsignedShort();
            int langLength = packet.ReadUnsignedShort();

            int dbOffset = packet.ReadUnsignedShort();
            int dbLength = packet.ReadUnsignedShort();

            packet.Skip(6);

            int ntlmOffset = packet.ReadUnsignedShort();
            int ntlmLength = packet.ReadUnsignedShort();

            int packetSize2 = packet.ReadInt();
            if (currentPacketSize != packetSize2)
            {
                throw new IOException("Invalid login packet.");
            }

            int offset = 86;
            wsid = ReadString(ref offset, packet, wsidOffset, wsidLength);
            user = ReadString(ref offset, packet, userOffset, userLength);
            password = EncryptPassword(ReadString(ref offset, packet, passOffset, passLength));
            application = ReadString(ref offset, packet, applOffset, applLength);
            server = ReadString(ref offset, packet, servOffset, servLength);
            ReadString(ref offset, packet, unknOffset, unknLength);
            program = ReadString(ref offset, packet, progOffset, progLength);
            language = ReadString(ref offset, packet, langOffset, langLength);
            database = ReadString(ref offset, packet, dbOffset, dbLength);
        }

        private static String ReadString(ref int offset, ReadPacket packet, int strOffset, int strLength)
        {
            if (strLength == 0)
            {
                return "";
            }
            if (offset < strOffset)
            {
                packet.Skip(strOffset - offset);
                offset = strOffset;
            }
            if (offset != strOffset)
            {
                throw new IOException("Invalid login packet");
            }
            offset += 2 * strLength;
            return packet.ReadString(strLength);
        }

        private static String EncryptPassword(String pw)
        {
            int xormask = 0xA5A5;
            int len = pw.Length;
            char[] chars = new char[len];

            for (int i = 0; i < len; ++i)
            {
                int c = pw[i] ^ xormask;
                int m1 = (c >> 4) & 0x0F0F;
                int m2 = (c << 4) & 0xF0F0;

                chars[i] = (char)(m1 | m2);
            }

            return new String(chars);
        }
	}
}
