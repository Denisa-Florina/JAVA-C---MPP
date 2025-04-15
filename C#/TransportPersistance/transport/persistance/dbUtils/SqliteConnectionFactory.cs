using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;
namespace ConnectionUtils
{
	public class SqliteConnectionFactory : ConnectionFactory
	{
		public override IDbConnection createConnection(IDictionary<string,string> props)
		{
			String connectionString = props["ConnectionString"];
			Console.WriteLine("SQLite ---Se deschide o conexiune la  ... {0}", connectionString);
			return new SQLiteConnection(connectionString);
		}
	}
	
}
