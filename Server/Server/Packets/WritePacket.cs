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
using System.IO;

namespace SharpSQL.Server.Packets
{
    internal class WritePacket : Packet
    {
        private Byte[] buffer;
        private int bufferIdx;
        private int paketSize;

        internal WritePacket(NetworkStream stream)
            : base(stream)
        {
            buffer = new byte[8192];
            paketSize = 4096;
        }

        internal void Init()
        {
            bufferIdx = HEADER_SIZE; 
        }

        internal void Send()
        {
            buffer[0] = (byte)Type;
            buffer[1] = 1; //last packet
            int size = bufferIdx;
            bufferIdx = 2;
            WriteBeShort(size);
            stream.Write(buffer, 0, size);
        }

        private void CheckBuffer(int bufferNeeded)
        {
            if (bufferIdx > paketSize)
            {
            }
        }

        internal void WriteByte(int value)
        {
            CheckBuffer(1);
        	buffer[bufferIdx++] = (byte)value;
        }

        internal void WriteShort(int value)
        {
            CheckBuffer(1);
            buffer[bufferIdx++] = (byte)value;
            buffer[bufferIdx++] = (byte)(value >> 8);
        }

        internal void WriteInt(int value)
        {
            CheckBuffer(1);
            buffer[bufferIdx++] = (byte)value;
            buffer[bufferIdx++] = (byte)(value >> 8);
            buffer[bufferIdx++] = (byte)(value >> 16);
            buffer[bufferIdx++] = (byte)(value >> 24);
        }

        /// <summary>
        /// Save a 2 byte value in big endian order.
        /// </summary>
        /// <param name="value"></param>
        internal void WriteBeShort(int value)
        {
            CheckBuffer(1);
            buffer[bufferIdx++] = (byte)(value >> 8);
            buffer[bufferIdx++] = (byte)value;
        }

        internal void WriteBytes(byte[] value)
        {
            CheckBuffer(value.Length);
            Array.Copy(value, 0, buffer, bufferIdx, value.Length);
            bufferIdx += value.Length;
        }


    }
}
