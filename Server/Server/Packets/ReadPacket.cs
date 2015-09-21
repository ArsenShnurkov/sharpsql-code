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

namespace SharpSQL.Server.Packets
{
    internal class ReadPacket : Packet
    {
        private Byte[] buffer;
        private int bufferIdx;
        private int bufferSize;
        private bool lastPacket;

        internal delegate void CallbackDelegate();

        internal ReadPacket(NetworkStream stream)
            : base(stream)
        {
            buffer = new byte[4096];
        }

        /// <summary>
        /// Read the next packet from the client asynchronous.
        /// </summary>
        internal void BeginReceive(CallbackDelegate callback)
        {
            bufferIdx = 0;
            bufferSize = HEADER_SIZE;
            stream.BeginRead(buffer, 0, HEADER_SIZE, new AsyncCallback(ProcessRead), callback);
        }

        private void ProcessRead(IAsyncResult result)
        {
            CallbackDelegate callback = (CallbackDelegate)result.AsyncState;
            if (HEADER_SIZE != stream.EndRead(result))
            {
                // the Header was not read that the client has cancel the connection
                stream.Close();
                return;
            }
            Type = ReadByte();
            lastPacket = ReadByte() != 0; // is ever 1 or 0
            int packetSize = ReadBeShort() - HEADER_SIZE;
            Skip(4);
            if (buffer.Length < packetSize)
            {
                buffer = new byte[packetSize];
            }
            ReadFully(packetSize);

            try
            {
                callback.Invoke();
            }
            catch (Exception ex)
            {
            	Logger.Error(ex);
                stream.Close();
                return;
            }
        }

        /// <summary>
        /// Read the requested count of bytes in the buffer or throws an exception.
        /// If ok then bufferIdx will be 0 and bufferSize has the value of count.
        /// </summary>
        /// <param name="count">the count of bytes that should be read.</param>
        private void ReadFully(int count)
        {
            int received = 0;
            while (received < count)
            {
                int read = stream.Read(buffer, received, count - received);
                if (read == 0)
                {
                    throw new IOException("Client shut down");
                }
                received += read;
            }
            bufferIdx = 0;
            bufferSize = count;
        }

        /// <summary>
        /// Check if enough bytes in the buffer for the next read operation.
        /// </summary>
        /// <param name="bytesNeeded">the needed byte count</param>
        private void CheckBuffer(int bytesNeeded)
        {
            if (bufferIdx + bytesNeeded > bufferSize)
            {
                throw new IOException("Read Buffer size(" + bufferSize + ") exceeded. Needed Bytes:" + bytesNeeded + " at " + bufferIdx);
            }
        }

        /// <summary>
        /// Skip some bytes form the buffer / stream.
        /// </summary>
        /// <param name="count">the byte count</param>
        internal void Skip(int count)
        {
            CheckBuffer(count);
            bufferIdx += count;
        }

        /// <summary>
        /// Read a single byte from Buffer with sign.
        /// </summary>
        /// <returns>the readed value</returns>
        internal int ReadByte()
        {
            CheckBuffer(1);
            return buffer[bufferIdx++];
        }

        /// <summary>
        /// Read a single byte from Buffer without sign.
        /// </summary>
        /// <returns>the readed value</returns>
        internal int ReadUnsignedByte()
        {
            return ReadByte() & 0xFF;
        }

        /// <summary>
        /// Read a 2 byte number with sign in Little Endian.
        /// </summary>
        /// <returns>the readed value</returns>
        internal int ReadShort()
        {
            CheckBuffer(2);
            return (buffer[bufferIdx++] & 0xFF) + (buffer[bufferIdx++] << 8);
        }

        /// <summary>
        /// Read a 2 byte number without sign in Little Endian.
        /// </summary>
        /// <returns></returns>
        internal int ReadUnsignedShort()
        {
            return ReadShort() & 0xFFFF;
        }

        /// <summary>
        /// Read a 4 byte number with sign in Little Endian.
        /// </summary>
        /// <returns>the readed value</returns>
        internal int ReadInt()
        {
            CheckBuffer(4);
            return (buffer[bufferIdx++] & 0xFF) +
                  ((buffer[bufferIdx++] & 0xFF) << 8) +
                  ((buffer[bufferIdx++] & 0xFF) << 16) +
                  ((buffer[bufferIdx++]) << 24);
        }

        /// <summary>
        /// Read a 2 byte number without sign in big endian order.
        /// </summary>
        /// <returns>the readed value</returns>
        internal int ReadBeShort()
        {
            CheckBuffer(2);
            return ((buffer[bufferIdx++] & 0xFF) << 8) + (buffer[bufferIdx++] & 0xFF);
        }

        /// <summary>
        /// Read multiple bytes from buffer.
        /// </summary>
        /// <param name="size">the byte count</param>
        /// <returns>the readed bytes</returns>
        internal byte[] ReadBytes(int size)
        {
            CheckBuffer(size);
            byte[] bytes = new byte[size];
            Array.Copy(buffer, bufferIdx, bytes, 0, size);
            bufferIdx += size;
            return bytes;
        }

        /// <summary>
        /// Get the currently avaialble data in the buffer.
        /// </summary>
        internal int DataAvailable
        {
            get
            {
                return bufferSize - bufferIdx;
            }
        }

        /// <summary>
        /// Read a unicode String
        /// </summary>
        /// <param name="length">the charcter count</param>
        /// <returns>the resulting String</returns>
        internal string ReadString(int length)
        {
            CheckBuffer(length*2);
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = (char)((buffer[bufferIdx++] & 0xFF) + (buffer[bufferIdx++] << 8));
            }
            return new String(chars);
        }
    }
}
