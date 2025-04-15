using System;
using System.Data;
using System.Collections.Generic;

namespace tasks.repository
{
	public static class DBUtils
	{
		private static IDbConnection instance = null;
		public static IDbConnection getConnection(IDictionary<string, string> props)
		{
			var connection = getNewConnection(props);
			connection.Open();
			return connection;
		}

		private static IDbConnection getNewConnection(IDictionary<string,string> props)
		{
			return ConnectionUtils.ConnectionFactory.getInstance().createConnection(props);
		}
	}
}
