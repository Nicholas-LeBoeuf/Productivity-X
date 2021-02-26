using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Productivity_X.Models
{
	public class DBManager
	{
		public string ConnectionString { get; set; }
		public DBManager(string connectionString)
		{
			this.ConnectionString = connectionString;
		}

		private MySqlConnection GetConnection()
		{
			return new MySqlConnection(ConnectionString);
		}

		public void SaveUser(User uc)
		{
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand Query = conn.CreateCommand();
				Query.CommandText = "insert into Calendar_Schema.user_tbl (user_id, firstname, lastname, username, email, password, confirmpassword, verificationcode) VALUES (@userID,@firstname,@lastname, @username, @email, @password, @confirmpassword, @verificationcode)";
				Query.Parameters.AddWithValue("@userID", uc.userID);
				Query.Parameters.AddWithValue("@firstname", uc.firstname);
				Query.Parameters.AddWithValue("@lastname", uc.lastname);
				Query.Parameters.AddWithValue("@username", uc.username);
				Query.Parameters.AddWithValue("@email", uc.email);
				Query.Parameters.AddWithValue("@password", uc.password);
				Query.Parameters.AddWithValue("@confirmpassword", uc.confirmpassword);
				Query.Parameters.AddWithValue("@verificationcode", uc.verificationcode);

				Query.ExecuteNonQuery();
			}
		}
	}
}
