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
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using NUnit.Framework;

namespace TestSQLServer
{
	[TestFixture]
	public class Test1
	{
		[Test]
		public void TestConnect()
		{
                string connectionString = @"Initial Catalog=master;" +
                            @"Data Source=localhost;" +
                            @"User ID=sa;" +
                            @"Password=javacash";

                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
		}
		
		[TestFixtureSetUp]
		public void Init()
		{
			SharpSQL.Server.SqlServer.Start();
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			SharpSQL.Server.SqlServer.Stop();
		}
	}
}
