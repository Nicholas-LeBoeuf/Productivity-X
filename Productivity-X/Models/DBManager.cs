﻿using System;
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

/*		public UserCreateAccnt CreateUser()
		{
			UserCreateAccnt myUser = new UserCreateAccnt();
			return myUser;
		}
*/

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
					Query.CommandText = "insert into Calendar_Schema.user_tbl (user_id, firstname, lastname, username, email, password, confirmpassword, verificationcode) VALUES (@userID,@firstname,@lastname, @username, @email, @password, @confirmpassword, @verificationcode)";
					Query.Parameters.AddWithValue("@userID", uc.userID);
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

		public bool CheckPassword(UserLogin loginUser)
		{
			bool bRet = false;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				// Checks the username and password for Login Screen
				MySqlCommand CheckData = conn.CreateCommand();
				// Provide the username as a parameter:
				CheckData.Parameters.AddWithValue("@username", loginUser.username);
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
						loginUser.userID = Convert.ToInt32(values[0]);
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

		public int GetUserID(ForgotPw1 forgotpassword)
		{
			int nUserID = -1;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand FindUser = conn.CreateCommand();

				// Checks to see if there are duplicate usernames
				FindUser.Parameters.AddWithValue("@username", forgotpassword.username);
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

						//loginUser.password = Convert.ToString(values[1]);

						//						if (isValidPassword)
						//						{
						//							bRet = true;
						//						}
					}
				}
				reader.Close();
			}
			return nUserID;
		}

		public void SaveSecurityCode(int nUserID, string sCode)
		{
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				// Inserting data into fields of database
				MySqlCommand Query = conn.CreateCommand();
				Query.CommandText = "update Calendar_Schema.user_tbl set verificationcode = @verificationcode where (user_id = @userid)";
				Query.Parameters.AddWithValue("@userID", nUserID);
				Query.Parameters.AddWithValue("@verificationcode", sCode);

				Query.ExecuteNonQuery();
			}
		}
	}
}
