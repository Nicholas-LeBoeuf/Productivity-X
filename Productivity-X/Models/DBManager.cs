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

/*		public User CreateUser()
		{
			User myUser = new User();
			return myUser;
		}
*/
		public bool SaveUser(User uc)
		{
			bool bRet = false;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand CheckUser = conn.CreateCommand();
				CheckUser.Parameters.AddWithValue("@username", uc.username);
				CheckUser.CommandText = "select count(*) from Calendar_Schema.user_tbl where userName = @userName";
				// if 1 then already exist
				int UserExist = Convert.ToInt32(CheckUser.ExecuteScalar());

				if (UserExist >= 1)
				{
					bRet = true;
				}
				else
				{
					MySqlCommand Query = conn.CreateCommand();
					Query.CommandText = "insert into Calendar_Schema.user_tbl (user_id, firstname, lastname, username, email, password, confirmpassword, verificationcode) VALUES (@userID,@firstname,@lastname, @username, @email, @password, @confirmpassword, @verificationcode)";
					Query.Parameters.AddWithValue("@userID", uc.userID);
					Query.Parameters.AddWithValue("@firstname", uc.fname);
					Query.Parameters.AddWithValue("@lastname", uc.lname);
					Query.Parameters.AddWithValue("@username", uc.username);
					Query.Parameters.AddWithValue("@email", uc.email);
					Query.Parameters.AddWithValue("@password", uc.password);
					Query.Parameters.AddWithValue("@confirmpassword", uc.confirmPassword);
					Query.Parameters.AddWithValue("@verificationcode", uc.verificationcode);

					Query.ExecuteNonQuery();
				}
			}
			return bRet;
		}



//		public void GetUser()
	}
}
