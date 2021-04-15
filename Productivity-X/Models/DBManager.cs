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
					Query.CommandText = "insert into Calendar_Schema.events_tbl (user_id, eventname, event_date, start_at, end_at, notification, reminder, " +
						"location, description, categoryname, friendname) VALUES (@user_id, @eventname, @event_date, @start_at, @end_at, @notification, @reminder, @location, @description, " +
						"@categoryname, @friendname)";

					Query.Parameters.AddWithValue("@user_id", nUserID);
					Query.Parameters.AddWithValue("@eventname", ce.eventName);
					// Saved as date
					Query.Parameters.AddWithValue("@event_date", ce.event_date);//Convert.ToDateTime(ce.event_date));
																				// Saved as time
					Query.Parameters.AddWithValue("@start_at", ce.start_at);
					// Saved as time
					Query.Parameters.AddWithValue("@end_at", ce.end_at);
					Query.Parameters.AddWithValue("@notification", ce.notification);
					Query.Parameters.AddWithValue("@reminder", ce.reminder);
					Query.Parameters.AddWithValue("@location", ce.location);
					Query.Parameters.AddWithValue("@description", ce.description);
					Query.Parameters.AddWithValue("@categoryname", ce.category);
					//Query.Parameters.AddWithValue("@guest", ce.guest);
					Query.Parameters.AddWithValue("@friendname", ce.friendUsername);

					Query.ExecuteNonQuery();

					if (ce.notification == true)
					{
						// Send notification to user via email based upon the current time and amount of minutes before starting time
					}

					/*if (ce.guest)
					{
						MySqlCommand InsertIntoGuestTable = conn.CreateCommand();
						InsertIntoGuestTable.CommandText = "insert into Calendar_Schema.guest_tbl (user_id, event_id, guest_username, guest_email, isFriend) VALUES (@user_id, @eventid, @guestusername, " +
							"@guestemail, @isfriend)";

						InsertIntoGuestTable.Parameters.AddWithValue("@user_id", nUserID);
						InsertIntoGuestTable.Parameters.AddWithValue("@eventid", GetEventID(ce.eventName, ce.event_date, ce.start_at, ce.end_at, nUserID));
						InsertIntoGuestTable.Parameters.AddWithValue("@guestusername", ce.guestUsername);
						InsertIntoGuestTable.Parameters.AddWithValue("@guestemail", ce.guestEmail);
						InsertIntoGuestTable.Parameters.AddWithValue("@isfriend", ce.friend);
						InsertIntoGuestTable.ExecuteNonQuery();
					}*/
				}
			}
			return bRet;
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
				/*				MySqlCommand CheckData = conn.CreateCommand();
								// Checks to see if user invited any guests or friends
								CheckData.Parameters.AddWithValue("@inviteguest", true);
								CheckData.Parameters.AddWithValue("@userid", userid);
								CheckData.CommandText = "SELECT event_id FROM Calendar_Schema.events_tbl where guest = @inviteguest and user_id=@userid";
								CheckData.ExecuteNonQuery();
								// Execute the SQL command against the DB:
								MySqlDataReader reader = CheckData.ExecuteReader();
								// If can find and read eventid, will delete event connection to guest table...
								if (reader.Read())
								{
									int id = Convert.ToInt32(reader[0]);
									reader.Close();
									MySqlCommand deleteGuestRow = conn.CreateCommand();
									deleteGuestRow.Parameters.AddWithValue("@eventid", id);
									deleteGuestRow.Parameters.AddWithValue("@userid", userid);
									deleteGuestRow.CommandText = "delete FROM Calendar_Schema.guest_tbl where event_id = @eventid and user_id=@userid";

									deleteGuestRow.ExecuteNonQuery();
								}
								else
									reader.Close();
				*/
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



		/*
				public bool EditEvent(EditEvent ee)
				{
					bool bRet = true;
					using (MySqlConnection conn = GetConnection())
					{
						conn.Open();
						MySqlCommand UpdateEvent = conn.CreateCommand();
						UpdateEvent.CommandText = "update Calendar_Schema.events_tbl set user_id = @user_id, event_date = @event_date, " +
							"start_at = @start_at, end_at = @end_at, notification = @notification, reminder = @reminder, location = @location, description = @description, categoryname = @categoryname, " +
							"guest = @guest, friend = @friendbool where eventname = @eventname and event_id = @eventid";
						UpdateEvent.Parameters.AddWithValue("@user_id", DBObject.id);
						UpdateEvent.Parameters.AddWithValue("@eventname", ee.m_sEventName);
						UpdateEvent.Parameters.AddWithValue("@eventid", DBObject.eventid);
						// Saved as date
						UpdateEvent.Parameters.AddWithValue("@event_date", Convert.ToDateTime(ee.m_sEventDate));
						// Saved as time
						UpdateEvent.Parameters.AddWithValue("@start_at", ee.m_sTimeStartAt);
						// Saved as time
						UpdateEvent.Parameters.AddWithValue("@end_at", ee.m_sTimeEndAt);
						UpdateEvent.Parameters.AddWithValue("@notification", ee.m_nNotification);
						UpdateEvent.Parameters.AddWithValue("@reminder", ee.m_nReminder);
						UpdateEvent.Parameters.AddWithValue("@location", ee.m_sLocation);
						UpdateEvent.Parameters.AddWithValue("@description", ee.m_sDescription);
						UpdateEvent.Parameters.AddWithValue("@categoryname", ee.m_sCategory);
						UpdateEvent.Parameters.AddWithValue("@guest", ee.m_bGuest);
						UpdateEvent.Parameters.AddWithValue("@friendbool", ee.m_bIsFriend);
						UpdateEvent.ExecuteNonQuery();
						if (ee.m_nNotification == true)
						{
							// Send notification to user via email based upon the current time and amount of minutes before starting time
						}
						if (ee.m_bGuest)
						{
							MySqlCommand InsertIntoGuestTable = conn.CreateCommand();
							InsertIntoGuestTable.CommandText = "update Calendar_Schema.guest_tbl user_id = @user_id, event_id = @eventid, guest_username = @guestusername, " +
								"guest_email = @guestemail, isfriend = @isfriend)";
							InsertIntoGuestTable.Parameters.AddWithValue("@user_id", DBObject.id);
							InsertIntoGuestTable.Parameters.AddWithValue("@eventid", DBObject.eventid);
							InsertIntoGuestTable.Parameters.AddWithValue("@guestusername", ee.m_sGuestUsername);
							InsertIntoGuestTable.Parameters.AddWithValue("@guestemail", ee.m_sGuestEmail);
							InsertIntoGuestTable.Parameters.AddWithValue("@isfriend", ee.m_bIsFriend);
							InsertIntoGuestTable.ExecuteNonQuery();
						}
						else
						{
							MySqlCommand deleteGuestRow = conn.CreateCommand();
							deleteGuestRow.CommandText = "delete FROM Calendar_Schema.guest_tbl where event_id = @eventid";
							deleteGuestRow.Parameters.AddWithValue("@eventid", DBObject.eventid);
							deleteGuestRow.ExecuteNonQuery();
						}
					}
					// Returns true if successful
					return bRet;
				}

				
		*/

		public List<Events> GetEventsFromDB(int nUserID)
		{
			int eventid;
			int reminder;
			object[] eventDataList = new object[11];
			List<Events> eventData = new List<Events>();
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
					eventDataList[4] = (Convert.ToBoolean(reader[6]));
					reminder = Convert.ToInt32(reader[7]);
					eventDataList[6] = (Convert.ToString(reader[8]));
					eventDataList[7] = (Convert.ToString(reader[9]));
					eventDataList[8] = (Convert.ToString(reader[10]));
					eventDataList[9] = (Convert.ToString(reader[11]));
					eventData.Add(new Events(eventDataList, eventid, reminder));
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
				//FindEvents.CommandText = "SELECT e.eventName,e.event_date,e.start_at,e.end_at,e.categoryname, c.color FROM Calendar_Schema.events_tbl e  left join  Calendar_Schema.category_tbl c on e.categoryname = c.categoryname where e.user_id = @user_id and c.user_id = @user_id";
				FindEvents.CommandText = "SELECT e.eventName,e.event_date,e.start_at,e.end_at,e.categoryname FROM Calendar_Schema.events_tbl e where e.user_id = @user_id";

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
					else if(category == "Friends")
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
					}) ;
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

		// Find today's events for today page...
		public List<TodayEventView> GetTodayEvents(int userid)
		{
			var result = new List<TodayEventView>();
			string color = "", categoryname = "";

			using (MySqlConnection conn = GetConnection()) { 
				conn.Open();
				var date = DateTime.Now.ToString("yyyy-MM-dd");
				// Find events that match todays date
				using (MySqlCommand cmd = new MySqlCommand("SELECT e.eventName,e.event_date,e.start_at,e.end_at,e.categoryname FROM Calendar_Schema.events_tbl e where e.user_id = @user_id and e.event_date= " + "'" + date + "'",conn))
				{
					cmd.Parameters.AddWithValue("@user_id", userid);

					using (var reader = cmd.ExecuteReader())
					{
						while (reader.Read()) // Read returns false if the event does not exist!
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
				AmountTasksCreated.CommandText = "select count(*) from Calendar_Schema.todo_tbl where user_id = @userid";
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
				countOldEvents.CommandText = "select count(*) FROM Calendar_Schema.todo_tbl where user_id=@userid and task_date < now() - interval 1 DAY";
				int nOldEvents = Convert.ToInt32(countOldEvents.ExecuteScalar());

				if (nOldEvents > 0)
				{
					using (MySqlCommand cmd = new MySqlCommand("select task_id from Calendar_Schema.todo_tbl where user_id=@userid and task_date = curdate() - interval 1 day and complete = @finished", conn))
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
					using (MySqlCommand cmd = new MySqlCommand("select task_id from Calendar_Schema.todo_tbl where user_id=@userid and task_date = curdate() - interval 1 day and complete = @finished", conn))
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

					using (MySqlCommand cmd = new MySqlCommand("select task_id from Calendar_Schema.todo_tbl where user_id=@userid and task_date = Curdate() - interval 2 day and complete = @finished", conn))
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
			bool bRet = false;
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

				if (sameFilename >= 1)
				{
					bRet = true;
				}
				else
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
	}
}