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
		public int m_UserID;

		public DBManager(string connectionString)
		{
			this.ConnectionString = connectionString;
		}

		private MySqlConnection GetConnection()
		{
			return new MySqlConnection(ConnectionString);
		}

		// Saves user info to database
		public bool SaveUser(UserCreateAccnt uc)
		{
			bool bRet = false;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand CheckUser = conn.CreateCommand();

				// Checks to see if there are duplicate usernames
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
					// Hash password
					uc.password = BCrypt.Net.BCrypt.HashPassword(uc.password);
					uc.confirmPassword = BCrypt.Net.BCrypt.HashPassword(uc.confirmPassword);

					// Inserting data into fields of database
					MySqlCommand Query = conn.CreateCommand();
					Query.CommandText = "insert into Calendar_Schema.user_tbl (firstname, lastname, username, email, password, confirmpassword, verificationcode) VALUES (@firstname,@lastname, @username, @email, @password, @confirmpassword, @verificationcode)";
					Query.Parameters.AddWithValue("@firstname", uc.fname);
					Query.Parameters.AddWithValue("@lastname", uc.lname);
					Query.Parameters.AddWithValue("@username", uc.username);
					Query.Parameters.AddWithValue("@email", uc.email);
					Query.Parameters.AddWithValue("@password", uc.password);
					Query.Parameters.AddWithValue("@confirmpassword", uc.confirmPassword);
					Query.Parameters.AddWithValue("@verificationcode", " ");

					Query.ExecuteNonQuery();
				}
			}
			return bRet;
		}

		// Get userid from database without any arguments to meet
		public int GetUserID()
		{
			int nUserID = -1;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand FindUser = conn.CreateCommand();

				// Checks to see if there are duplicate usernames
				FindUser.Parameters.AddWithValue("@username", DBObject.sUsername);
				FindUser.CommandText = "SELECT user_id FROM Calendar_Schema.user_tbl where username = @username";

				// Execute the SQL command against the DB:
				MySqlDataReader reader = FindUser.ExecuteReader();
				if (reader.Read()) // Read returns false if the user does not exist!
				{
					// Read the DB values:
					Object[] values = new object[1];
					int fieldCount = reader.GetValues(values);
					if (1 == fieldCount)
					{
						// Successfully retrieved the user from the DB:
						nUserID = nUserID = Convert.ToInt32(values[0]);
						DBObject.sID = nUserID;
					}
				}
				reader.Close();
			}
			return nUserID;
		}

		// Gets the userid from Database
		public int GetUserID(ForgotPw1 forgotpassword)
		{
			int nUserID = -1;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand FindUser = conn.CreateCommand();

				// Checks to see if there are duplicate usernames
				FindUser.Parameters.AddWithValue("@username", DBObject.sUsername);
				FindUser.Parameters.AddWithValue("@email", forgotpassword.email);
				FindUser.CommandText = "SELECT user_id FROM Calendar_Schema.user_tbl where username = @username and email = @email";

				// Execute the SQL command against the DB:
				MySqlDataReader reader = FindUser.ExecuteReader();
				if (reader.Read()) // Read returns false if the user does not exist!
				{
					// Read the DB values:
					Object[] values = new object[1];
					int fieldCount = reader.GetValues(values);
					if (1 == fieldCount)
					{
						// Successfully retrieved the user from the DB:
						nUserID = Convert.ToInt32(values[0]);
						DBObject.sID = nUserID;
					}
				}
				reader.Close();
			}
			return nUserID;
		}

		public bool LoadUser(UserLogin dbUser)
		{
			bool bRet = false;

			// Checks the username and password for Login Screen
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand CheckData = conn.CreateCommand();

				// Checks to see if there are duplicate usernames
				CheckData.Parameters.AddWithValue("@username", DBObject.sUsername);
				CheckData.CommandText = "SELECT user_id, password FROM Calendar_schema.user_tbl where userName = @userName";

				// Execute the SQL command against the DB:
				MySqlDataReader reader = CheckData.ExecuteReader();
				if (reader.Read()) // Read returns false if the user does not exist!
				{
					// Read the DB values:
					Object[] values = new object[2];
					int fieldCount = reader.GetValues(values);
					if (2 == fieldCount) // Asked for 2 values, so expecting 2 values!
					{
						// Successfully retrieved the user from the DB:
						DBObject.sID = Convert.ToInt32(values[0]);
						dbUser.password = values[1].ToString();

						bRet = true;
					}
				}
				reader.Close();
			}

			return bRet;
		}

		// Gets the username based upon id
		public string GetUserName()
		{
			string sRet = "";
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				// Inserting data into fields of database
				MySqlCommand FindUsername = conn.CreateCommand();
				FindUsername.CommandText = "select username from calendar_schema.user_tbl where user_id = @userID;";
				FindUsername.Parameters.AddWithValue("@userID", DBObject.sID);
				FindUsername.ExecuteNonQuery();

				// Execute the SQL command against the DB:
				MySqlDataReader reader = FindUsername.ExecuteReader();
				if (reader.Read()) // Read returns false if the verificationcode does not exist!
				{
					// Read the DB values:
					Object[] values = new object[1];
					int fieldCount = reader.GetValues(values);
					if (1 == fieldCount)
					{
						sRet = values[0].ToString();
					}
				}
				reader.Close();
			}
			return sRet;
		}

		// Gets the password based upon id
		public string GetPassword()
		{
			string sRet = "";
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				// Inserting data into fields of database
				MySqlCommand FindPassword = conn.CreateCommand();
				FindPassword.CommandText = "select password from calendar_schema.user_tbl where user_id = @userID;";
				FindPassword.Parameters.AddWithValue("@userID", DBObject.sID);
				FindPassword.ExecuteNonQuery();

				// Execute the SQL command against the DB:
				MySqlDataReader reader = FindPassword.ExecuteReader();
				if (reader.Read()) // Read returns false if the verificationcode does not exist!
				{
					// Read the DB values:
					Object[] values = new object[1];
					int fieldCount = reader.GetValues(values);
					if (1 == fieldCount)
					{
						sRet = values[0].ToString();
					}
				}
				reader.Close();
			}
			return sRet;
		}

		// Checks if password matches with username
		public bool CheckPassword(UserLogin loginUser)
		{
			bool bRet = false;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				// Checks the username and password for Login Screen
				MySqlCommand CheckData = conn.CreateCommand();
				// Provide the username as a parameter:
				CheckData.Parameters.AddWithValue("@username", DBObject.sUsername);
				CheckData.CommandText = "SELECT user_id, password FROM Calendar_Schema.user_tbl where username = @username";

				// Execute the SQL command against the DB:
				MySqlDataReader reader = CheckData.ExecuteReader();
				if (reader.Read()) // Read returns false if the user does not exist!
				{
					// Read the DB values:
					Object[] values = new object[2];
					int fieldCount = reader.GetValues(values);
					if (2 == fieldCount)
					{
						// Successfully retrieved the user from the DB:
						DBObject.sID = Convert.ToInt32(values[0]);
						string password = Convert.ToString(values[1]);
						//loginUser.password = Convert.ToString(values[1]);

						bool isValidPassword = BCrypt.Net.BCrypt.Verify(loginUser.password, password);
						if (isValidPassword)
						{
							bRet = true;
						}
					}
				}
				reader.Close();
			}
			return bRet;
		}

		public void UpdatePassword(ForgotPw3 forgotPassword)
		{
			bool bRet = false;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				// Inserting data into fields of database
				MySqlCommand Query = conn.CreateCommand();
				Query.CommandText = "update Calendar_Schema.user_tbl set password = @newpassword, confirmpassword = @confirmpassword where (user_id = @userid)";

				// Hash passwords
				forgotPassword.newPassword = BCrypt.Net.BCrypt.HashPassword(forgotPassword.newPassword);
				forgotPassword.confirmPassword = BCrypt.Net.BCrypt.HashPassword(forgotPassword.confirmPassword);
				Query.Parameters.AddWithValue("@newpassword", forgotPassword.newPassword);
				Query.Parameters.AddWithValue("@confirmpassword", forgotPassword.confirmPassword);
				Query.Parameters.AddWithValue("@userid", DBObject.sID);
				Query.ExecuteNonQuery();
			}
		}

		// Update verificationcode field in database
		public void SaveSecurityCode(string sCode)
		{
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				// Inserting data into fields of database
				MySqlCommand Query = conn.CreateCommand();
				Query.CommandText = "update Calendar_Schema.user_tbl set verificationcode = @verificationcode where (user_id = @userid)";
				Query.Parameters.AddWithValue("@userID", DBObject.sID);
				Query.Parameters.AddWithValue("@verificationcode", sCode);

				Query.ExecuteNonQuery();
			}
		}

		// Get the security code from user table in database
		public bool CheckSecurityCode(ForgotPw2 forgotpw2)
		{
			bool bRet = false;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				// Inserting data into fields of database
				MySqlCommand FindSecurityCode = conn.CreateCommand();
				FindSecurityCode.CommandText = "SELECT verificationcode FROM Calendar_Schema.user_tbl where user_id = @userid";
				FindSecurityCode.Parameters.AddWithValue("@userID", DBObject.sID);
				FindSecurityCode.ExecuteNonQuery();

				// Execute the SQL command against the DB:
				MySqlDataReader reader = FindSecurityCode.ExecuteReader();
				if (reader.Read()) // Read returns false if the verificationcode does not exist!
				{
					// Read the DB values:
					Object[] values = new object[1];
					int fieldCount = reader.GetValues(values);
					if (1 == fieldCount && values[0].ToString() == forgotpw2.verificationCode)
					{
						bRet = true;
					}
				}
				reader.Close();
			}
			return bRet;
		}


	}
}