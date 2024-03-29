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
		public object TempData { get; private set; }

		public int m_UserID;

		public DBManager(string connectionString)
		{
			this.ConnectionString = connectionString;
		}

		private MySqlConnection GetConnection()
		{
			return new MySqlConnection(ConnectionString);
		}

		// User, login, forgot password queries....
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
		public int GetUserID(string sUsername)
		{

			int userID = -1;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand FindUser = conn.CreateCommand();

				// Checks to see if there are duplicate usernames
				FindUser.Parameters.AddWithValue("@username", sUsername);
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
						userID = userID = Convert.ToInt32(values[0]);
					}
				}
				reader.Close();
			}
			return userID;
		}

		// Gets the userid from Database
		public int GetUserID(ForgotPw1 forgotpassword, string sUsername)
		{
			int userID = -1;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand FindUser = conn.CreateCommand();

				// Checks to see if there are duplicate usernames
				FindUser.Parameters.AddWithValue("@username", sUsername);
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
						userID = Convert.ToInt32(values[0]);
					}
				}
				reader.Close();
			}
			return userID;
		}

		public bool LoadUser(UserLogin dbUser, ref int userid, string sUsername)
		{
			bool bRet = false;

			// Checks the username and password for Login Screen
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand CheckData = conn.CreateCommand();

				// Checks to see if there are duplicate usernames
				CheckData.Parameters.AddWithValue("@username", sUsername);
				CheckData.CommandText = "SELECT user_id, password FROM Calendar_Schema.user_tbl where userName = @userName";

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
						userid = Convert.ToInt32(values[0]);
						dbUser.password = values[1].ToString();

						bRet = true;
					}
				}
				reader.Close();
			}

			return bRet;
		}

		// Gets the username based upon id
		public string GetUserName(int nUserID)
		{
			string sRet = "";
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				// Inserting data into fields of database
				MySqlCommand FindUsername = conn.CreateCommand();
				FindUsername.CommandText = "select username from Calendar_Schema.user_tbl where user_id = @userID;";
				FindUsername.Parameters.AddWithValue("@userID", nUserID);
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
		public string GetPassword(int nUserID)
		{
			string sRet = "";
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				// Inserting data into fields of database
				MySqlCommand FindPassword = conn.CreateCommand();
				FindPassword.CommandText = "select password from Calendar_Schema.user_tbl where user_id = @userID;";
				FindPassword.Parameters.AddWithValue("@userID", nUserID);
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
		public bool CheckPassword(UserLogin loginUser, string sUsername, int nUserID)
		{
			bool bRet = false;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				// Checks the username and password for Login Screen
				MySqlCommand CheckData = conn.CreateCommand();
				// Provide the username as a parameter:
				CheckData.Parameters.AddWithValue("@username", sUsername);
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
						nUserID = Convert.ToInt32(values[0]);
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

		public void UpdatePassword(ForgotPw3 forgotPassword, int nUserID)
		{
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
				Query.Parameters.AddWithValue("@userid", nUserID);
				Query.ExecuteNonQuery();
			}
		}

		// Update verificationcode field in database
		public void SaveSecurityCode(string sCode, int nUserID)
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

		// Get the security code from user table in database
		public bool CheckSecurityCode(ForgotPw2 forgotpw2, int nUserID)
		{
			bool bRet = false;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				// Inserting data into fields of database
				MySqlCommand FindSecurityCode = conn.CreateCommand();
				FindSecurityCode.CommandText = "SELECT verificationcode FROM Calendar_Schema.user_tbl where user_id = @userid";
				FindSecurityCode.Parameters.AddWithValue("@userID", nUserID);
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

		// Event Queries:
		// Saves event info to database
		public bool SaveEvent(UserCreateEvent ce, int nUserID)
		{
			bool bRet = true;

			using (MySqlConnection conn = GetConnection())
			{
				int nEventExists = 0;
				conn.Open();
				MySqlCommand CheckEvent = conn.CreateCommand();

				// Checks to see if there are duplicate values and if event already created
				CheckEvent.Parameters.AddWithValue("@event_name", ce.eventName);
				CheckEvent.Parameters.AddWithValue("@event_date", Convert.ToDateTime(ce.event_date));
				CheckEvent.Parameters.AddWithValue("@start_at", ce.start_at);
				CheckEvent.Parameters.AddWithValue("@end_at", ce.end_at);
				CheckEvent.Parameters.AddWithValue("@userid", nUserID);
				CheckEvent.CommandText = "select count(*) from Calendar_Schema.events_tbl where eventname = @event_name and event_date = @event_date and start_at = @start_at and end_at = @end_at and user_id = @userid";

				nEventExists = Convert.ToInt32(CheckEvent.ExecuteScalar());

				if (nEventExists >= 1)
				{
					bRet = false;
				}
				else
				{
					// Inserting data into fields of database, can have duplicate events:
					MySqlCommand Query = conn.CreateCommand();
					Query.CommandText = "insert into Calendar_Schema.events_tbl (user_id, eventname, event_date, start_at, end_at, " +
						"location, description, categoryname, friendname, bAcceptEvent) VALUES (@user_id, @eventname, @event_date, @start_at, @end_at, @location, @description, " +
						"@categoryname, @friendname, @accept)";

					Query.Parameters.AddWithValue("@user_id", nUserID);
					Query.Parameters.AddWithValue("@eventname", ce.eventName);
					// Saved as date
					Query.Parameters.AddWithValue("@event_date", ce.event_date);//Convert.ToDateTime(ce.event_date));
																				// Saved as time
					Query.Parameters.AddWithValue("@start_at", ce.start_at);
					// Saved as time
					Query.Parameters.AddWithValue("@end_at", ce.end_at);
					Query.Parameters.AddWithValue("@location", ce.location);
					Query.Parameters.AddWithValue("@description", ce.description);
					Query.Parameters.AddWithValue("@categoryname", ce.category);
					Query.Parameters.AddWithValue("@friendname", ce.friendUsername);
					if (ce.friendUsername != "Not Selected")
					{
						Query.Parameters.AddWithValue("@accept", false);
					}
					else
					{
						Query.Parameters.AddWithValue("@accept", true);
					}

					Query.ExecuteNonQuery();
				}
			}
			return bRet;
		}



		// Save event that has been recommended on friends page..
		public void SaveRecommendedEvent(int eventid, int nUserID)
		{
			int friendid = 1;
			string username = "";
			object[] eventDataList = new object[9];

			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				using (MySqlCommand cmd = new MySqlCommand("select * from Calendar_Schema.events_tbl where event_id = @eventid", conn))
				{
					cmd.Parameters.AddWithValue("@eventid", eventid);
					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							friendid = (Convert.ToInt32(reader[0]));
							eventDataList[0] = (Convert.ToString(reader[2]));
							eventDataList[1] = (Convert.ToString(reader[3]));
							eventDataList[2] = (Convert.ToString(reader[4]));
							eventDataList[3] = (Convert.ToString(reader[5]));
							eventDataList[4] = (Convert.ToString(reader[6]));
							eventDataList[5] = (Convert.ToString(reader[7]));
							eventDataList[6] = (Convert.ToString(reader[8]));
							eventDataList[7] = (Convert.ToString(reader[9]));
							eventDataList[8] = (Convert.ToBoolean(reader[10]));
						}
					}
				}

				// Get username of friend
				using (MySqlCommand getusername = new MySqlCommand("select username from Calendar_Schema.user_tbl where user_id = @friendid", conn))
				{
					getusername.Parameters.AddWithValue("@friendid", nUserID);
					using (var reader = getusername.ExecuteReader())
					{
						while (reader.Read())
						{
							username = (Convert.ToString(reader[0]));
						}
					}
				}


				// Check for events that have not been completed and keep for next day
				using (MySqlCommand cmd2 = new MySqlCommand("insert into Calendar_Schema.events_tbl (user_id, eventname, event_date, start_at, end_at, " +
					"location, description, categoryname, friendname, bAcceptEvent) VALUES (@user_id, @eventname, @event_date, @start_at, @end_at, @location, @description, " +
					"@categoryname, @friendname, @accept)", conn))
				{
					cmd2.Parameters.AddWithValue("@user_id", nUserID);
					cmd2.Parameters.AddWithValue("@eventname", eventDataList[0]);
					// Saved as date
					cmd2.Parameters.AddWithValue("@event_date", Convert.ToDateTime(eventDataList[1]));
					cmd2.Parameters.AddWithValue("@start_at", eventDataList[2]);
					// Saved as time
					cmd2.Parameters.AddWithValue("@end_at", eventDataList[3]);
					cmd2.Parameters.AddWithValue("@location", eventDataList[4]);
					cmd2.Parameters.AddWithValue("@description", eventDataList[5]);
					cmd2.Parameters.AddWithValue("@categoryname", "Friends");
					cmd2.Parameters.AddWithValue("@friendname", username);
					cmd2.Parameters.AddWithValue("@accept", true);
					cmd2.ExecuteNonQuery();
				}

				// update friends events to true
				if (Convert.ToString(eventDataList[6]) != "Not Selected")
				{
					using (MySqlCommand cmd3 = new MySqlCommand("update Calendar_Schema.events_tbl set bAcceptEvent=true, friendname=@username where user_id = @friendid and event_id=@eventid", conn))
					{
						cmd3.Parameters.AddWithValue("@friendid", friendid);
						cmd3.Parameters.AddWithValue("@username", username);
						cmd3.Parameters.AddWithValue("@eventid", eventid);
						cmd3.ExecuteNonQuery();
					}
				}
			}
		}

		// Save event that has been recommended on friends page..
		public void DeleteRecommendedEvent(int eventid, int nUserID)
		{
			int friendid = 1;
			string username = "";
			object[] eventDataList = new object[9];

			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				using (MySqlCommand cmd = new MySqlCommand("select * from Calendar_Schema.events_tbl where event_id = @eventid", conn))
				{
					cmd.Parameters.AddWithValue("@eventid", eventid);
					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							friendid = (Convert.ToInt32(reader[0]));
							eventDataList[0] = (Convert.ToString(reader[2]));
							eventDataList[1] = (Convert.ToString(reader[3]));
							eventDataList[2] = (Convert.ToString(reader[4]));
							eventDataList[3] = (Convert.ToString(reader[5]));
							eventDataList[4] = (Convert.ToString(reader[6]));
							eventDataList[5] = (Convert.ToString(reader[7]));
							eventDataList[6] = (Convert.ToString(reader[8]));
							eventDataList[7] = (Convert.ToString(reader[9]));
							eventDataList[8] = (Convert.ToBoolean(reader[10]));
						}
					}
				}

				// update friends events to true
				using (MySqlCommand cmd3 = new MySqlCommand("update Calendar_Schema.events_tbl set bAcceptEvent=true, friendname=@friendname where user_id=@friendid and event_id=@eventid", conn))
				{
					cmd3.Parameters.AddWithValue("@friendid", friendid);
					cmd3.Parameters.AddWithValue("@username", username);
					cmd3.Parameters.AddWithValue("@friendname", "Friend cannot make event!");
					cmd3.Parameters.AddWithValue("@eventid", eventid);
					cmd3.ExecuteNonQuery();
				}	
			}
		}

		public int GetEventID(string eventname, string eventdate, string startat, string endat, int nUserID)
		{
			int nRet = 0;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				// Inserting data into fields of database
				MySqlCommand FindEventID = conn.CreateCommand();
				FindEventID.CommandText = "select event_ID from Calendar_Schema.events_tbl where user_id = @userID and eventname = @eventname and event_date = @event_date " +
					"and start_at = @start_at and end_at = @end_at";
				FindEventID.Parameters.AddWithValue("@userID", nUserID);
				FindEventID.Parameters.AddWithValue("@eventname", eventname);
				FindEventID.Parameters.AddWithValue("@event_date", Convert.ToDateTime(eventdate));
				FindEventID.Parameters.AddWithValue("@start_at", startat);
				FindEventID.Parameters.AddWithValue("@end_at", endat);
				FindEventID.ExecuteNonQuery();

				// Execute the SQL command against the DB:
				MySqlDataReader reader = FindEventID.ExecuteReader();
				if (reader.Read()) // Read returns false if the verificationcode does not exist!
				{
					// Read the DB values:
					Object[] values = new object[1];
					int fieldCount = reader.GetValues(values);
					if (1 == fieldCount)
					{
						nRet = Convert.ToInt32(values[0]);
					}
				}
				reader.Close();
			}
			return nRet;
		}

		public void DeleteEvent(int eventid, int userid)
		{
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand deleteEventRow = conn.CreateCommand();
				deleteEventRow.Parameters.AddWithValue("@eventid", eventid);
				deleteEventRow.Parameters.AddWithValue("@userid", userid);
				deleteEventRow.CommandText = "delete FROM Calendar_Schema.events_tbl where event_id = @eventid and user_id=@userid";
				deleteEventRow.ExecuteNonQuery();
			}
		}


		public bool DeleteEventsGreaterThan10Days(int userid)
		{
			bool bRet = false;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				MySqlCommand countOldEvents = conn.CreateCommand();
				var date = DateTime.Now.ToString("yyyy-MM-dd");

				//Checks to see if there are duplicate category values for category name
				countOldEvents.Parameters.AddWithValue("@userid", userid);
				countOldEvents.Parameters.AddWithValue("@todaysdate", date);
				countOldEvents.CommandText = "select count(*) FROM Calendar_Schema.events_tbl where user_id=@userid and event_date < now() - interval 10 DAY";
				int nOldEvents = Convert.ToInt32(countOldEvents.ExecuteScalar());

				if (nOldEvents == 0)
				{
					bRet = false;
				}
				else
				{
					MySqlCommand deleteOldEvents = conn.CreateCommand();
					deleteOldEvents.Parameters.AddWithValue("@userid", userid);
					deleteOldEvents.CommandText = "delete FROM Calendar_Schema.events_tbl where user_id=@userid and event_date < now() - interval 10 DAY";
					deleteOldEvents.ExecuteNonQuery();
					bRet = true;
				}
			}
			return bRet;
		}

		public List<UserEvents> GetEventsFromDB(int nUserID)
		{
			int eventid;
			object[] eventDataList = new object[9];
			List<UserEvents> eventData = new List<UserEvents>();
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				MySqlCommand FindEventData = conn.CreateCommand();
				FindEventData.CommandText = "select * from Calendar_Schema.events_tbl where user_id = @userid ORDER BY DATE(event_date) DESC, start_at asc";
				FindEventData.Parameters.AddWithValue("@userid", nUserID);
				FindEventData.ExecuteNonQuery();

				// Execute the SQL command against the DB:
				MySqlDataReader reader = FindEventData.ExecuteReader();
				while (reader.Read())
				{
					eventid = Convert.ToInt32(reader[1]);
					eventDataList[0] = (Convert.ToString(reader[2]));
					eventDataList[1] = (Convert.ToString(reader[3]));
					eventDataList[2] = (Convert.ToString(reader[4]));
					eventDataList[3] = (Convert.ToString(reader[5]));
					eventDataList[4] = (Convert.ToString(reader[6]));
					eventDataList[5] = (Convert.ToString(reader[7]));
					eventDataList[6] = (Convert.ToString(reader[8]));
					eventDataList[7] = (Convert.ToString(reader[9]));
					eventDataList[8] = (Convert.ToBoolean(reader[10]));

					eventData.Add(new UserEvents(eventDataList, eventid));
				}
				reader.Close();
				for (int counter = 0; counter < eventData.Count(); counter++)
				{
					if (eventData[counter].GetCategory() != "Default" && eventData[counter].GetCategory() != "Friends")
					{
						MySqlCommand FindCategoryColor = conn.CreateCommand();
						FindCategoryColor.CommandText = "select color from Calendar_Schema.category_tbl where user_id = @user_id and categoryname = @categoryname";
						FindCategoryColor.Parameters.AddWithValue("@user_id", nUserID);
						FindCategoryColor.Parameters.AddWithValue("@categoryname", eventData[counter].GetCategory());
						FindCategoryColor.ExecuteNonQuery();

						// Execute the SQL command against the DB:
						MySqlDataReader Reader = FindCategoryColor.ExecuteReader();
						while (Reader.Read())
						{
							eventData[counter].SetEventColor(Convert.ToString(Reader[0]));
						}
						Reader.Close();
					}
					else if (eventData[counter].GetCategory() == "Default")
					{
						eventData[counter].SetEventColor("grey");
					}
					else
					{
						eventData[counter].SetEventColor("#FF69B4");
					}
				}
			}

			return eventData;
		}

		// Get weekly events for weekly calendar....
		public List<WeeklyEventsView> GetWeeklyEvents(int userid)
		{
			var result = new List<WeeklyEventsView>();
			string color = "", category = "";
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand FindEvents = conn.CreateCommand();
				FindEvents.Parameters.AddWithValue("@user_id", userid);
				FindEvents.CommandText = "SELECT e.eventName,e.event_date,e.start_at,e.end_at,e.categoryname, friendname, bAcceptEvent FROM Calendar_Schema.events_tbl e where e.user_id = @user_id";

				// Execute the SQL command against the DB:
				MySqlDataReader reader = FindEvents.ExecuteReader();
				while (reader.Read()) // Read returns false if the event does not exist!
				{
					category = reader[4].ToString();
					//var color = reader[5].ToString();
					if (category == "Default")
					{
						color = "grey";

					}
					else if (category == "Friends")
					{
						color = "pink";
					}
					else
					{
						color = "";
					}
					// Read the DB values:
					result.Add(new WeeklyEventsView()
					{
						categoryname = category,
						name = reader[0].ToString(),
						start = Convert.ToDateTime(reader[1].ToString()).ToString("yyyy-MM-dd") + "  " + reader[2].ToString(),
						end = Convert.ToDateTime(reader[1].ToString()).ToString("yyyy-MM-dd") + "  " + reader[3].ToString(),
						color = color
					});
					
				}
				reader.Close();

				// Did not set color yet, category must exist in database
				for (int counter = 0; counter < result.Count(); counter++)
				{
					if (result[counter].color == "" && (result[counter].categoryname != "Default" && result[counter].categoryname != "Friends"))
					{
						MySqlCommand FindCategoryColor = conn.CreateCommand();
						FindCategoryColor.CommandText = "select color from Calendar_Schema.category_tbl where user_id = @user_id and categoryname = @categoryname";
						FindCategoryColor.Parameters.AddWithValue("@user_id", userid);
						FindCategoryColor.Parameters.AddWithValue("@categoryname", result[counter].categoryname);
						FindCategoryColor.ExecuteNonQuery();

						// Execute the SQL command against the DB:
						MySqlDataReader Reader = FindCategoryColor.ExecuteReader();
						while (Reader.Read())
						{
							result[counter].color = Convert.ToString(Reader[0]);
						}
						Reader.Close();
					}
					else
						continue;
				}
			}
			return result;
		}

		// Get user's events and friends events....
		public List<CombinedEvents> GetCombinedEvents(int userid, int friendid)
		{
			var result = new List<CombinedEvents>();
			string color = "";
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand FindEvents = conn.CreateCommand();
				FindEvents.Parameters.AddWithValue("@user_id", userid);
				FindEvents.Parameters.AddWithValue("@friendid", friendid);
				FindEvents.CommandText = "SELECT e.user_id, e.eventName,e.event_date,e.start_at,e.end_at,e.categoryname, bAcceptEvent FROM Calendar_Schema.events_tbl e where e.user_id in (@user_id, @friendid)";

				// Execute the SQL command against the DB:
				MySqlDataReader reader = FindEvents.ExecuteReader();
				while (reader.Read()) // Read returns false if the event does not exist!
				{
					// if friend has not accepted event, do not display event
					if (Convert.ToBoolean(reader[6]) || Convert.ToInt32(reader[0]) == userid)
					{
						// Friend event
						if (Convert.ToInt32(reader[0]) == friendid)
						{
							color = "pink";
						}
						else
						{
							color = "green";
						}

						// Read the DB values:
						result.Add(new CombinedEvents()
						{
							name = reader[1].ToString(),
							start = Convert.ToDateTime(reader[2].ToString()).ToString("yyyy-MM-dd") + "  " + reader[3].ToString(),
							end = Convert.ToDateTime(reader[2].ToString()).ToString("yyyy-MM-dd") + "  " + reader[4].ToString(),
							color = color
						});
					}
				}
				reader.Close();
			}
			return result;
		}

		// Find today's events for today page...
		public List<TodayEventView> GetTodayEvents(int userid)
		{
			var result = new List<TodayEventView>();
			string color = "", categoryname = "";

			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				var date = DateTime.Now.ToString("yyyy-MM-dd");
				// Find events that match todays date
				using (MySqlCommand cmd = new MySqlCommand("SELECT e.eventName,e.event_date,e.start_at,e.end_at,e.categoryname, bAcceptEvent FROM Calendar_Schema.events_tbl e where e.user_id = @user_id and e.event_date= " + "'" + date + "'", conn))
				{
					cmd.Parameters.AddWithValue("@user_id", userid);

					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read()) // Read returns false if the event does not exist!
						{
							if (Convert.ToBoolean(reader[5]) || Convert.ToString(reader[5]) == "Not Selected")
							{
								categoryname = reader[4].ToString();
								if (categoryname == "Default")
								{
									color = "grey";

								}
								else if (categoryname == "Friends")
								{
									color = "pink";
								}
								// Category exists in database
								else
								{
									using (MySqlConnection conn2 = GetConnection())
									{
										conn2.Open();
										// Find color in the database and then set variable color
										using (MySqlCommand categoryreader = new MySqlCommand("select color from Calendar_Schema.category_tbl where user_id = @user_id and categoryname = @categoryname", conn2))
										{
											categoryreader.Parameters.AddWithValue("@user_id", userid);
											categoryreader.Parameters.AddWithValue("@categoryname", categoryname);

											using (var reader2 = categoryreader.ExecuteReader())
											{
												while (reader2.Read())
												{
													color = Convert.ToString(reader2[0]);
												}
												reader2.Close();
											}
										}
									}
								}
								// Read the DB values:
								result.Add(new TodayEventView()
								{
									name = reader[0].ToString(),
									start = Convert.ToDateTime(reader[1].ToString()).ToString("yyyy-MM-dd") + "  " + reader[2].ToString(),
									end = Convert.ToDateTime(reader[1].ToString()).ToString("yyyy-MM-dd") + "  " + reader[3].ToString(),
									color = color,
									category = "J"
								});
							}
						}
						reader.Close();
					}
				}
			}
			return result;
		}




		/*		
				//-----------TodayButton, find events with todays date, pass back event id----------------
				public List<string> FindTodaysEvents(string todaysDate)

				{
					List<string> eventData = new List<string>();
					using (MySqlConnection conn = GetConnection())
					{
						conn.Open();
						MySqlCommand UpdateEvent = conn.CreateCommand();
						UpdateEvent.CommandText = "select event_id, eventname from Calendar_Schema.events_tbl where user_id = @userid and event_date = @eventdate";
						UpdateEvent.Parameters.AddWithValue("@userid", DBObject.id);
						UpdateEvent.Parameters.AddWithValue("@eventdate", Convert.ToDateTime(todaysDate));
						UpdateEvent.ExecuteNonQuery();
						// Execute the SQL command against the DB:
						MySqlDataReader reader = UpdateEvent.ExecuteReader();
						while (reader.Read())
						{
							eventData.Add(Convert.ToString(reader[0]));
						}
						reader.Close();
					}
					return eventData;
				}
		*/

		// Category queries.... 
		public string GetCategoryNameFromDB(int categoryid, int nUserID)
		{
			string categoryname = "";
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand FindCategoryName = conn.CreateCommand();

				//Checks to see if there are duplicate category values for category name
				FindCategoryName.Parameters.AddWithValue("@categoryid", categoryid);
				FindCategoryName.Parameters.AddWithValue("@userid", nUserID);
				FindCategoryName.CommandText = "select categoryname from Calendar_Schema.category_tbl where category_id = @categoryid and user_id = @userid";
				MySqlDataReader reader = FindCategoryName.ExecuteReader();

				while (reader.Read())
				{
					categoryname = reader.GetString(0);
				}
				reader.Close();
			}
			return categoryname;
		}

		public bool SaveCategory(UserCreateCategory cat, int nUserID)
		{
			bool bRet = true;

			using (MySqlConnection conn = GetConnection())
			{
				int nCatExists = 0;
				conn.Open();
				MySqlCommand CheckCategories = conn.CreateCommand();

				//Checks to see if there are duplicate category values for category name
				CheckCategories.Parameters.AddWithValue("@category_name", cat.categoryname);
				CheckCategories.Parameters.AddWithValue("@userid", nUserID);
				CheckCategories.CommandText = "select count(*) from Calendar_Schema.category_tbl where categoryname = @category_name and user_id = @userid";
				nCatExists = Convert.ToInt32(CheckCategories.ExecuteScalar());

				MySqlCommand CheckCategoryColors = conn.CreateCommand();
				// Checks to see if there are duplicate category values for category color
				CheckCategoryColors.Parameters.AddWithValue("@color", cat.color);
				CheckCategoryColors.Parameters.AddWithValue("@userid", nUserID);
				CheckCategoryColors.CommandText = "select count(*) from Calendar_Schema.category_tbl where color = @color and user_id = @userid";
				nCatExists += Convert.ToInt32(CheckCategoryColors.ExecuteScalar());

				if (nCatExists >= 1)
				{
					bRet = false;
				}
				else
				{
					MySqlCommand Query = conn.CreateCommand();
					Query.CommandText = "insert into Calendar_Schema.category_tbl (user_id, categoryname, color, description) VALUES (@user_id, @category_name, @color, @description)";

					Query.Parameters.AddWithValue("@user_id", nUserID);
					Query.Parameters.AddWithValue("@category_name", cat.categoryname);
					Query.Parameters.AddWithValue("@color", cat.color);
					Query.Parameters.AddWithValue("@description", cat.description);

					Query.ExecuteNonQuery();
				}
			}

			return bRet;
		}

		// Get data from category table including categoryname, color, description based upon the categoryid

		public List<Categories> GetCategoriesFromDB(int userid)
		{
			object[] categoryDataList = new object[3];
			List<Categories> categoryObj = new List<Categories>();
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand FindCategoryData = conn.CreateCommand();
				FindCategoryData.CommandText = "select category_id, categoryname, color, description from Calendar_Schema.category_tbl where user_id = @user_id";
				FindCategoryData.Parameters.AddWithValue("@user_id", userid);
				FindCategoryData.ExecuteNonQuery();
				// Execute the SQL command against the DB:
				MySqlDataReader reader = FindCategoryData.ExecuteReader();

				int categoryid = 0;
				while (reader.Read())
				{
					//categoryDataList[0]=(Convert.ToInt32(reader[0]));
					categoryid = Convert.ToInt32(reader[0]);
					categoryDataList[0] = reader.GetString(1);//Convert.ToString(reader[1]);
					categoryDataList[1] = Convert.ToString(reader[2]);
					categoryDataList[2] = Convert.ToString(reader[3]);
					categoryObj.Add(new Categories(categoryDataList, categoryid));
				}
				reader.Close();
			}
			return categoryObj;
		}
		public void DeleteCategory(int categoryid, string categoryname, int userid)
		{
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				MySqlCommand updateEventsTable = conn.CreateCommand();
				updateEventsTable.Parameters.AddWithValue("@categoryname", categoryname);
				updateEventsTable.Parameters.AddWithValue("@userid", userid);
				updateEventsTable.CommandText = "update Calendar_Schema.events_tbl set categoryname = \"Default\" where categoryname = @categoryname and user_id = @userid";
				updateEventsTable.ExecuteNonQuery();

				MySqlCommand deleteCategoryRow = conn.CreateCommand();
				deleteCategoryRow.Parameters.AddWithValue("@categoryid", categoryid);
				deleteCategoryRow.CommandText = "delete FROM Calendar_Schema.category_tbl where category_id = @categoryid";
				deleteCategoryRow.ExecuteNonQuery();
			}
		}


		// To-Do queries...
		public bool SaveTask(UserCreateTask ct, int nUserID)
		{
			bool bRet = true;
			int nTaskCounter = 0;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand AmountTasksCreated = conn.CreateCommand();

				// Checks to see if there are more than 10 tasks created
				AmountTasksCreated.CommandText = "select count(*) from Calendar_Schema.todo_tbl where user_id = @userid and task_date = curdate()";
				AmountTasksCreated.Parameters.AddWithValue("@userid", nUserID);
				nTaskCounter = Convert.ToInt32(AmountTasksCreated.ExecuteScalar());

				if (nTaskCounter >= 10)
				{
					bRet = false;
				}
				else
				{
					var date = DateTime.Now.ToString("yyyy-MM-dd");
					MySqlCommand Query = conn.CreateCommand();
					Query.CommandText = "insert into Calendar_Schema.todo_tbl (user_id, taskname, complete, keepfornextday, task_date) VALUES (@user_id, @taskname, @bFinished, @bkeepfornextday, @date)";
					Query.Parameters.AddWithValue("@user_id", nUserID);
					Query.Parameters.AddWithValue("@taskname", ct.taskName);
					Query.Parameters.AddWithValue("@bFinished", false);
					Query.Parameters.AddWithValue("@bkeepfornextday", false);
					Query.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
					Query.ExecuteNonQuery();
				}
			}
			return bRet;
		}
		public List<ToDoTasks> GetTasksFromDB(int userid)
		{
			//			object[] tasksDataList = new object[3];
			List<ToDoTasks> taskObj = new List<ToDoTasks>();
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand FindTaskData = conn.CreateCommand();
				FindTaskData.CommandText = "select task_id, taskname, complete, keepfornextday from Calendar_Schema.todo_tbl where user_id = @user_id";
				FindTaskData.Parameters.AddWithValue("@user_id", userid);
				FindTaskData.ExecuteNonQuery();
				// Execute the SQL command against the DB:
				MySqlDataReader reader = FindTaskData.ExecuteReader();

				int taskid = 0;
				string taskname;
				bool finished, keeptask;
				while (reader.Read())
				{
					taskid = Convert.ToInt32(reader[0]);
					taskname = reader.GetString(1);
					finished = Convert.ToBoolean(reader[2]);
					keeptask = Convert.ToBoolean(reader[3]);
					taskObj.Add(new ToDoTasks(taskid, taskname, finished, keeptask));
				}
				reader.Close();
			}
			return taskObj;
		}

		// Check if there are events that are not completed yet, but expired then save them to a list to display next to the to do list... 
		public void DeleteOrKeepTasksAfterMidnight(int userid)
		{
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				MySqlCommand countOldEvents = conn.CreateCommand();
				var date = DateTime.Now.ToString("yyyy-MM-dd");

				//Checks to see if there are duplicate category values for category name
				countOldEvents.Parameters.AddWithValue("@userid", userid);
				//countOldEvents.Parameters.AddWithValue("@todaysdate", date);
				countOldEvents.CommandText = "select count(*) FROM Calendar_Schema.todo_tbl where user_id=@userid and task_date < curdate()";
				int nOldEvents = Convert.ToInt32(countOldEvents.ExecuteScalar());

				if (nOldEvents > 0)
				{
					using (MySqlCommand cmd = new MySqlCommand("select task_id from Calendar_Schema.todo_tbl where user_id=@userid and task_date <= curdate() - interval 1 day and complete = @finished", conn))
					{
						cmd.Parameters.AddWithValue("@userid", userid);
						cmd.Parameters.AddWithValue("@finished", true);
						using (var reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								using (MySqlConnection conn2 = GetConnection())
								{
									conn2.Open();
									using (MySqlCommand deleteTasks = new MySqlCommand("delete FROM Calendar_Schema.todo_tbl where user_id = @userid and task_id = @taskid", conn2))
									{
										deleteTasks.Parameters.AddWithValue("@userid", userid);
										deleteTasks.Parameters.AddWithValue("@taskid", reader[0]);
										deleteTasks.ExecuteNonQuery();
									}
								}
							}
						}
					}

					// Check for events that have not been completed and keep for next day
					using (MySqlCommand cmd = new MySqlCommand("select task_id from Calendar_Schema.todo_tbl where user_id=@userid and task_date <= curdate() - interval 1 day and complete = @finished", conn))
					{
						cmd.Parameters.AddWithValue("@userid", userid);
						cmd.Parameters.AddWithValue("@finished", false);
						using (var reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								using (MySqlConnection conn2 = GetConnection())
								{
									conn2.Open();
									using (MySqlCommand updateTasks = new MySqlCommand("update Calendar_Schema.todo_tbl set keepfornextday = true where task_id = @taskid and user_id = @userid", conn2))
									{
										updateTasks.Parameters.AddWithValue("@userid", userid);
										updateTasks.Parameters.AddWithValue("@taskid", reader[0]);
										updateTasks.ExecuteNonQuery();
									}
								}
							}
						}
					}

					using (MySqlCommand cmd = new MySqlCommand("select task_id from Calendar_Schema.todo_tbl where user_id=@userid and task_date <= Curdate() - interval 2 day and complete = @finished and keepfornextday = 1", conn))
					{
						cmd.Parameters.AddWithValue("@userid", userid);
						cmd.Parameters.AddWithValue("@finished", false);
						using (var reader = cmd.ExecuteReader())
						{
							while (reader.Read())
							{
								using (MySqlConnection conn2 = GetConnection())
								{
									conn2.Open();
									// Delete tasks that have been saved for two days
									using (MySqlCommand deleteTasksAfter2Days = new MySqlCommand("delete FROM Calendar_Schema.todo_tbl where user_id = @userid and task_id = @taskid", conn2))
									{
										deleteTasksAfter2Days.Parameters.AddWithValue("@userid", userid);
										deleteTasksAfter2Days.Parameters.AddWithValue("@taskid", reader[0]);
										deleteTasksAfter2Days.ExecuteNonQuery();
									}
								}
							}
						}
					}

				}


			}
		}







		public void DeleteTask(int taskid, int userid)
		{
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand deleteCategoryRow = conn.CreateCommand();
				deleteCategoryRow.Parameters.AddWithValue("@taskid", taskid);
				deleteCategoryRow.Parameters.AddWithValue("@userid", userid);
				deleteCategoryRow.CommandText = "delete FROM Calendar_Schema.todo_tbl where task_id = @taskid and user_id = @userid";
				deleteCategoryRow.ExecuteNonQuery();
			}
		}

		public void UpdateTask(int taskid, int userid, bool userCheckedBox)
		{
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand updateEventsTable = conn.CreateCommand();
				updateEventsTable.Parameters.AddWithValue("@taskid", taskid);
				updateEventsTable.Parameters.AddWithValue("@userid", userid);
				if (userCheckedBox)
				{
					updateEventsTable.CommandText = "update Calendar_Schema.todo_tbl set complete = false where task_id = @taskid and user_id = @userid";
				}
				else
					updateEventsTable.CommandText = "update Calendar_Schema.todo_tbl set complete = true where task_id = @taskid and user_id = @userid";

				updateEventsTable.ExecuteNonQuery();
			}
		}

		public bool TaskCompleteFromDB(int taskid, int nUserID)
		{
			bool bComplete = false;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand FindCategoryName = conn.CreateCommand();

				//Checks to see if there are duplicate category values for category name
				FindCategoryName.Parameters.AddWithValue("@taskid", taskid);
				FindCategoryName.Parameters.AddWithValue("@userid", nUserID);
				FindCategoryName.CommandText = "select complete from Calendar_Schema.todo_tbl where task_id = @taskid and user_id = @userid";
				MySqlDataReader reader = FindCategoryName.ExecuteReader();

				while (reader.Read())
				{
					bComplete = reader.GetBoolean(0);
				}
				reader.Close();
			}
			return bComplete;
		}

		public void SaveUserProfilePicDB(string filename, int userid)
		{
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand CheckUser = conn.CreateCommand();

				// Checks to see if there are duplicate usernames
				CheckUser.Parameters.AddWithValue("@filename", filename);
				CheckUser.Parameters.AddWithValue("@userid", userid);
				CheckUser.CommandText = "select count(*) from Calendar_Schema.user_tbl where profilePic = @filename and user_id = @userid";

				// if 1 then already exist
				int sameFilename = Convert.ToInt32(CheckUser.ExecuteScalar());

				if (sameFilename == 0)
				{
					// Inserting data into profilepic field of database
					MySqlCommand Query = conn.CreateCommand();
					Query.CommandText = "update Calendar_Schema.user_tbl set profilepic = @filename where user_id = @userid";
					Query.Parameters.AddWithValue("@filename", filename);
					Query.Parameters.AddWithValue("@userid", userid);

					Query.ExecuteNonQuery();
				}
			}
		}

		// Gets the profile pic based upon user id
		public string GetProfilePicFromDB(int nUserID)
		{
			string sRet = "";
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				// Inserting data into fields of database
				MySqlCommand FindProfilePic = conn.CreateCommand();
				FindProfilePic.CommandText = "select profilepic from Calendar_Schema.user_tbl where user_id = @userID;";
				FindProfilePic.Parameters.AddWithValue("@userID", nUserID);
				FindProfilePic.ExecuteNonQuery();

				// Execute the SQL command against the DB:
				MySqlDataReader reader = FindProfilePic.ExecuteReader();
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
		public List<UserView> GetSearchUser(string keyword)
		{
			var result = new List<UserView>();
			if (keyword != string.Empty && keyword != null)
			{
				using (MySqlConnection conn = GetConnection())
				{
					conn.Open();
					MySqlCommand FindEvents = conn.CreateCommand();
					FindEvents.CommandText = "SELECT user_id,username,email FROM Calendar_Schema.user_tbl  where username =" + "'" + keyword + "'" + " or  email like'%" + keyword + "%'";

					// Execute the SQL command against the DB:
					MySqlDataReader reader = FindEvents.ExecuteReader();
					while (reader.Read()) // Read returns false if the user does not exist!
					{
						result.Add(new UserView()
						{
							userid = reader[0].ToString(),
							username = reader[1].ToString(),
							email = reader[2].ToString(),
						});
					}
					reader.Close();
				}
			}
			else
			{
				using (MySqlConnection conn = GetConnection())
				{
					conn.Open();
					MySqlCommand FindEvents = conn.CreateCommand();
					FindEvents.CommandText = "SELECT user_id,username,email FROM Calendar_Schema.user_tbl ";

					// Execute the SQL command against the DB:
					MySqlDataReader reader = FindEvents.ExecuteReader();
					while (reader.Read()) // Read returns false if the user does not exist!
					{
						result.Add(new UserView()
						{
							userid = reader[0].ToString(),
							username = reader[1].ToString(),
							email = reader[2].ToString(),
						});
					}
					reader.Close();
				}
			}

			return result;
		}

		public bool AddFriend(int userid, int friendId)
		{
			bool status = false;

			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand currentQuery = conn.CreateCommand();
				currentQuery.Parameters.AddWithValue("@user_id", friendId);
				currentQuery.Parameters.AddWithValue("@friend_id", userid);
				currentQuery.CommandText = "insert into Calendar_Schema.friends_tbl (user_id, friend_id, request ) VALUES (@user_id, @friend_id, 0)";
				currentQuery.ExecuteNonQuery();
				//MySqlDataReader reader = currentQuery.ExecuteReader();
				//reader.Close();
				status = true;
			}
			return status;
		}
		public bool VerifyFriend(int userid, int friendId)
		{
			bool status = false;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand Query = conn.CreateCommand();
				Query.Parameters.AddWithValue("@user_id", userid);
				Query.Parameters.AddWithValue("@friend_id", friendId);
				Query.CommandText = "update Calendar_Schema.friends_tbl set request = 1 where user_id = @user_id and  friend_id = @friend_id;";
				Query.ExecuteNonQuery();

				MySqlCommand currentQuery = conn.CreateCommand();
				currentQuery.Parameters.AddWithValue("@user_id", friendId);
				currentQuery.Parameters.AddWithValue("@friend_id", userid);
				currentQuery.CommandText = "insert into Calendar_Schema.friends_tbl (user_id, friend_id, request ) VALUES (@user_id, @friend_id, 1)";
				currentQuery.ExecuteNonQuery();
				status = true;
			}
			return status;
		}


		public bool DeleteFriend(int userid, int friendId)
		{
			bool status = false;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand Query = conn.CreateCommand();
				Query.Parameters.AddWithValue("@user_id", userid);
				Query.Parameters.AddWithValue("@friend_id", friendId);
				Query.CommandText = "Delete from Calendar_Schema.friends_tbl where request =1 and (user_id = @user_id and  friend_id = @friend_id) or (user_id = @friend_id and  friend_id = @user_id) ;";
				Query.ExecuteNonQuery();
			}
			return status;
		}
		public bool DeleteRequest(int userid, int friendId)
		{
			bool status = false;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand Query = conn.CreateCommand();
				Query.Parameters.AddWithValue("@user_id", userid);
				Query.Parameters.AddWithValue("@friend_id", friendId);
				Query.CommandText = "Delete from Calendar_Schema.friends_tbl where request = 0 and user_id = @user_id and  friend_id = @friend_id ;";
				Query.ExecuteNonQuery();
			}
			return status;
		}
		public List<FriendsView> GetFriendsRequest(int userid)
		{
			var result = new List<FriendsView>();

			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand FindEvents = conn.CreateCommand();
				FindEvents.Parameters.AddWithValue("@user_id", userid);
				FindEvents.CommandText = "SELECT u.user_id,u.username,u.email,f.request,f.id,u.profilepic FROM Calendar_Schema.friends_tbl f join user_tbl u on f.friend_id = u.user_id where f.user_id = @user_id and f.request = 0;";

				MySqlDataReader reader = FindEvents.ExecuteReader();
				while (reader.Read()) // Read returns false if the user does not exist!
				{
					result.Add(new FriendsView()
					{
						friendid = Convert.ToInt32(reader[0]),
						friendname = reader[1].ToString(),
						friendemail = reader[2].ToString(),
						request = Convert.ToBoolean(reader[3].ToString()),
						id = Convert.ToInt32(reader[4]),
						profilepic = reader[5].ToString()
					});
				}
				reader.Close();
			}
			return result;
		}
		public List<FriendsView> GetFriends(int userid)
		{
			var result = new List<FriendsView>();

			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand FindEvents = conn.CreateCommand();
				FindEvents.Parameters.AddWithValue("@user_id", userid);
				FindEvents.CommandText = "SELECT u.user_id,u.username,u.email,f.request,f.id,u.profilepic FROM Calendar_Schema.friends_tbl f join user_tbl u on f.friend_id = u.user_id where f.user_id = @user_id and f.request = 1;";

				MySqlDataReader reader = FindEvents.ExecuteReader();
				while (reader.Read()) // Read returns false if the user does not exist!
				{
					result.Add(new FriendsView()
					{
						friendid = Convert.ToInt32(reader[0]),
						friendname = reader[1].ToString(),
						friendemail = reader[2].ToString(),
						request = Convert.ToBoolean(reader[3].ToString()),
						id = Convert.ToInt32(reader[4]),
						profilepic = reader[5].ToString()
					});
				}
				reader.Close();
			}
			return result;
		}

		public List<int> GetFriendIDs(int userid)
		{
			List<int> listids = new List<int>();
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				MySqlCommand findFriendIds = conn.CreateCommand();
				findFriendIds.Parameters.AddWithValue("@user_id", userid);
				findFriendIds.CommandText = "select friend_id from Calendar_Schema.friends_tbl where user_id = @user_id order by Rand() limit 2";
				MySqlDataReader reader = findFriendIds.ExecuteReader();
				while (reader.Read()) // Read returns false if the user does not exist!
				{
					listids.Add(Convert.ToInt32(reader[0]));
				}
				reader.Close();
			}
			return listids;
		}

		public string GetFriendName(int userid)
		{
			string username="";
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				MySqlCommand findFriendIds = conn.CreateCommand();
				findFriendIds.Parameters.AddWithValue("@friendid", userid);
				findFriendIds.CommandText = "select username from Calendar_Schema.user_tbl where user_id = @friendid";
				MySqlDataReader reader = findFriendIds.ExecuteReader();
				while (reader.Read()) // Read returns false if the user does not exist!
				{
					username = Convert.ToString(reader[0]);
				}
				reader.Close();
			}
			return username;
		}

		// Get events that represent your freetime or where other events do not line up
		public List<RcmdEvntsFrndsPg> GetRecommendedEvents(int nUserID, List<UserEvents> userevents, List<int> friendIDS)
		{
			int eventid;
			object[] eventDataList = new object[9];
			List<RcmdEvntsFrndsPg> eventData = new List<RcmdEvntsFrndsPg>();
			List<UserEvents> ue = new List<UserEvents>();
			// Friend ids;
			List<int> fi = new List<int>();
			ue = userevents;
			fi = friendIDS;
			List<string> usernames = new List<string>();
			// Selects the first entry in the grouping if there are duplicates
			var bar = new List<RcmdEvntsFrndsPg>();
			for (int counter = 0; counter < fi.Count; counter++)
			{
				usernames.Add(GetFriendName(fi[counter]));
			}
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				ue.RemoveAll(delegate (UserEvents ue)
				{
					return Convert.ToDateTime(ue.GetDate()) < Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
				});

				if (ue.Count != 0)
				{
					for (int counter = 0; counter < fi.Count; counter++)
					{
						for (int index = 0; index < ue.Count; index++)
						{
							MySqlCommand FindEventData = conn.CreateCommand();
							FindEventData.CommandText = "select * from Calendar_Schema.events_tbl where user_id=@userid and start_at not between @startat and @endat order by Rand() limit 10";
							//		FindEventData.CommandText = "select * from Calendar_Schema.events_tbl where user_id=@userid and start_at not between @startat and @endat and event_date != @date bacceptevent = true order by Rand() limit 2";
							FindEventData.Parameters.AddWithValue("@userid", fi[counter]);
							FindEventData.Parameters.AddWithValue("@startat", ue[counter].StartTime());
							FindEventData.Parameters.AddWithValue("@endat", ue[counter].EndTime());
							//FindEventData.Parameters.AddWithValue("@date", Convert.ToDateTime(ue[counter].GetDate()));

							FindEventData.ExecuteNonQuery();

							// Execute the SQL command against the DB:
							MySqlDataReader reader = FindEventData.ExecuteReader();
							while (reader.Read())
							{
								eventid = Convert.ToInt32(reader[1]);
								//eventname
								eventDataList[0] = (Convert.ToString(reader[2]));
								//event_date
								eventDataList[1] = (Convert.ToString(reader[3]));
								//start_at
								eventDataList[2] = (Convert.ToString(reader[4]));
								//end_at
								eventDataList[3] = (Convert.ToString(reader[5]));
								//location
								eventDataList[4] = (Convert.ToString(reader[6]));
								//description
								eventDataList[5] = (Convert.ToString(reader[7]));
								//categoryname
								eventDataList[6] = "Friends";
								//friendname
								eventDataList[7] = (usernames[counter]);
								//bacceptevent
								eventDataList[8] = (Convert.ToBoolean(reader[10]));

								eventData.Add(new RcmdEvntsFrndsPg(eventDataList, eventid));
							}
							reader.Close();
						}
					}

					eventData.RemoveAll(delegate (RcmdEvntsFrndsPg rc)
					{
						return Convert.ToDateTime(rc.GetDate()) < Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
					});
					bar = eventData.GroupBy(x => x.GetEventID()).Select(x => x.First()).ToList();
					return bar;
				}
				else
				{
					for (int counter = 0; counter < fi.Count; counter++)
					{
						MySqlCommand FindEventData = conn.CreateCommand();
						FindEventData.CommandText = "select * from Calendar_Schema.events_tbl where user_id=@userid order by Rand() limit 2";
						FindEventData.Parameters.AddWithValue("@userid", fi[counter]);

						FindEventData.ExecuteNonQuery();

						// Execute the SQL command against the DB:
						MySqlDataReader reader = FindEventData.ExecuteReader();
						while (reader.Read())
						{
							eventid = Convert.ToInt32(reader[1]);
							//eventname
							eventDataList[0] = (Convert.ToString(reader[2]));
							//event_date
							eventDataList[1] = (Convert.ToString(reader[3]));
							//start_at
							eventDataList[2] = (Convert.ToString(reader[4]));
							//end_at
							eventDataList[3] = (Convert.ToString(reader[5]));
							//location
							eventDataList[4] = (Convert.ToString(reader[6]));
							//description
							eventDataList[5] = (Convert.ToString(reader[7]));
							//categoryname
							eventDataList[6] = "Friends";
							//friendname
							eventDataList[7] = (usernames[counter]);
							//bacceptevent
							eventDataList[8] = (Convert.ToBoolean(reader[10]));

							eventData.Add(new RcmdEvntsFrndsPg(eventDataList, eventid));
						}
						reader.Close();
					}
				}
				return eventData;
			}
		}

		// Find all events where user has been invited too
		public List<RcmdEvntsFrndsPg> GetFriendInvitationEvents(int nUserID, List<int> friendIDS)
		{
			int eventid;
			object[] eventDataList = new object[9];
			List<RcmdEvntsFrndsPg> eventData = new List<RcmdEvntsFrndsPg>();
			List<UserEvents> ue = new List<UserEvents>();
			List<int> fi = new List<int>();
			fi = friendIDS;
			List<string> usernames = new List<string>();

			// Get usernames for each id
			for (int counter = 0; counter < fi.Count; counter++)
			{
				usernames.Add(GetFriendName(fi[counter]));
			}

			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				for (int counter = 0; counter < fi.Count; counter++)
				{
					MySqlCommand FindEventData = conn.CreateCommand();
					FindEventData.CommandText = "select * from Calendar_Schema.events_tbl where user_id=@friendid and bAcceptEvent = false";
					FindEventData.Parameters.AddWithValue("@friendid", fi[counter]);
					FindEventData.ExecuteNonQuery();

					// Execute the SQL command against the DB:
					MySqlDataReader reader = FindEventData.ExecuteReader();
					while (reader.Read())
					{
						eventid = Convert.ToInt32(reader[1]);
						//eventname
						eventDataList[0] = (Convert.ToString(reader[2]));
						//event_date
						eventDataList[1] = (Convert.ToString(reader[3]));
						//start_at
						eventDataList[2] = (Convert.ToString(reader[4]));
						//end_at
						eventDataList[3] = (Convert.ToString(reader[5]));
						//location
						eventDataList[4] = (Convert.ToString(reader[6]));
						//description
						eventDataList[5] = (Convert.ToString(reader[7]));
						//categoryname
						eventDataList[6] = "Friends";
						//friendname
						eventDataList[7] = (usernames[counter]);
						//bacceptevent
						eventDataList[8] = (Convert.ToBoolean(reader[10]));

						eventData.Add(new RcmdEvntsFrndsPg(eventDataList, eventid));
					}
					reader.Close();
				}
			}
			eventData.RemoveAll(delegate (RcmdEvntsFrndsPg rc)
			{
				return Convert.ToDateTime(rc.GetDate()) < Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
			});
			return eventData;
		}
	}
}
	