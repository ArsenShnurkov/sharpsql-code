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
using System.Collections.Generic;
using System.Text;

namespace SharpSQL.Server
{
    internal class Logger
    {
        private Logger()
        {
            //this is a Singeton
        }

        internal static void Info(String format, params Object[] args)
        {
            Console.WriteLine("[Info]" + format, args);
        }

        internal static void Error(String format, params Object[] args)
        {
            Console.WriteLine("[Error]" + format, args);
        }

        internal static void Error(Exception ex)
        {
            Console.WriteLine("[Error]" + ex);
        }

        internal static void Dump(byte[] bytes)
        {
            Dump(bytes, 0, bytes.Length);
        }

        internal static void Dump(byte[] bytes, int offset, int length)
        {
            char[] line = new char[16];
            for (int i = 0; i < length; i++)
            {
                if (i % 16 == 0 && (i>0))
                {
                    Console.WriteLine(new String(line));
                }
                byte digit = bytes[offset + i];
                Console.Write("{0:x2} ", digit);
                line[i % 16] = digit < ' ' ? ' ' : (char)digit;
            }
            Console.WriteLine(new String(line, 0, length % 16));
        }
    }
}
