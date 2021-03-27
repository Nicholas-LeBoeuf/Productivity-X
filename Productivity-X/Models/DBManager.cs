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

	//Create Event Button:
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
				CheckEvent.CommandText = "select count(*) from Calendar_Schema.events_tbl where eventname = @event_name and event_date = @event_date and start_at = @start_at and end_at = @end_at";

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
						"location, description, categoryname, guest, friend) VALUES (@user_id, @eventname, @event_date, @start_at, @end_at, @notification, @reminder, @location, @description, " +
						"@categoryname, @guest, @friend)";

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
					Query.Parameters.AddWithValue("@guest", ce.guest);
					Query.Parameters.AddWithValue("@friend", ce.friend);

					Query.ExecuteNonQuery();

					if (ce.notification == true)
					{
						// Send notification to user via email based upon the current time and amount of minutes before starting time
					}

					if (ce.guest)
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
					}
/*					else
					{
						nEventID = GetEventID(ce.eventName, ce.event_date, ce.start_at, ce.end_at);
					}
*/
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

				public bool DeleteEvent(int eventid)
				{
					bool bRet = true;
					using (MySqlConnection conn = GetConnection())
					{
						conn.Open();
						MySqlCommand CheckData = conn.CreateCommand();
						// Checks to see if user invited any guests or friends
						CheckData.Parameters.AddWithValue("@inviteguest", true);
						CheckData.CommandText = "SELECT eventid FROM Calendar_schema.events_tbl where guest = @inviteguest";
						CheckData.ExecuteNonQuery();
						// Execute the SQL command against the DB:
						MySqlDataReader reader = CheckData.ExecuteReader();
						if (reader.Read())
						{			
							MySqlCommand deleteGuestRow = conn.CreateCommand();
							CheckData.Parameters.AddWithValue("@eventid", Convert.ToString(reader[0]));
							deleteGuestRow.CommandText = "delete FROM Calendar_Schema.guest_tbl where event_id = @eventid";
							deleteGuestRow.ExecuteNonQuery();
							reader.Close();
						}
						MySqlCommand deleteEventRow = conn.CreateCommand();
						deleteEventRow.Parameters.AddWithValue("@eventid", DBObject.eventid);
						deleteEventRow.CommandText = "delete FROM Calendar_Schema.events_tbl where event_id = @eventid";
						deleteEventRow.ExecuteNonQuery();
					}
					return bRet;
				}
		*/

		public List<Events> EventData(int nUserID)
		{
			int eventid;
			int reminder;
			object[] eventDataList = new object[11];
			List<Events> eventData = new List<Events>();
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				MySqlCommand FindEventData = conn.CreateCommand();
				FindEventData.CommandText = "select * from Calendar_Schema.events_tbl where user_id = @userid";
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
					eventDataList[9] = (Convert.ToBoolean(reader[11]));
					eventDataList[10] = (Convert.ToBoolean(reader[12]));
					eventData.Add(new Events(eventDataList, eventid, reminder));
				}
				reader.Close();
			}
			return eventData;
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
			// Find total account created
			public int CountUsers()
			{
				int nRet = -1;
				using (MySqlConnection conn = GetConnection())
				{
					conn.Open();
					// Inserting data into fields of database
					MySqlCommand FindTotalUsers = conn.CreateCommand();
					FindTotalUsers.CommandText = "SELECT count(*) FROM Calendar_Schema.user_tbl";
					FindTotalUsers.ExecuteNonQuery();
					// Execute the SQL command against the DB:
					MySqlDataReader reader = FindTotalUsers.ExecuteReader();
					if (reader.Read()) // Read returns false if the verificationcode does not exist!
					{
						// Read the DB values:
						Object[] values = new object[1];
						int fieldCount = reader.GetValues(values);
						if (1 == fieldCount)
						{
							nRet = Int32.Parse(values[0].ToString());
						}
					}
					reader.Close();
				}
				return nRet;
			}
*/
		//----------Category Button------------------
		// Create category with fields
		// Edit category with fields
		// Delete a certain category
		// Pass back Category names that match userid -> can be used for dropdown box in create event form
		/*			public int TotalCategories(int UserID)
					{
						int numCategories = -1;
						using (MySqlConnection conn = GetConnection())
						{
							conn.Open();
							MySqlCommand FindCategories = conn.CreateCommand();
							FindCategories.CommandText = "select * count from Calendar_Schema.category_tbl where user_id = @userid";
							FindCategories.Parameters.AddWithValue("@userid", UserID);
							FindCategories.ExecuteNonQuery();
							// Execute the SQL command against the DB:
							MySqlDataReader reader = FindCategories.ExecuteReader();
							while (reader.Read())
							{
								numCategories = Convert.ToInt32(reader[0]);
							}
							reader.Close();
						}
						return numCategories;
					}
		*/

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
		public List<Categories> CategoryData(int userid)
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
		//----------Friend Button---------------

		public List<WeelyEventsView> GetWeeklyEvents(int userid)
		{
			var result = new List<WeelyEventsView>();
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand FindEvents = conn.CreateCommand();

				// Checks to see if there are duplicate usernames
				FindEvents.Parameters.AddWithValue("@user_id", userid);
				FindEvents.CommandText = "SELECT * FROM Calendar_Schema.events_tbl where user_id = @user_id";

				// Execute the SQL command against the DB:
				MySqlDataReader reader = FindEvents.ExecuteReader();
				while (reader.Read()) // Read returns false if the user does not exist!
				{
					// Read the DB values:
					result.Add(new WeelyEventsView() 
					{
						name = reader[2].ToString(),
						start = reader[3].ToString().Substring(0, 10).Replace("/", "-") + reader[4].ToString(),
						end = reader[3].ToString().Substring(0, 10).Replace("/", "-") + reader[5].ToString(),
						GetColor = reader[3].ToString().Substring(0, 10).Replace("/", "-") + reader[5].ToString(),
						categoryid = reader[3].ToString().Substring(0, 10).Replace("/", "-") + reader[5].ToString(),
					});
				}
				reader.Close();
			}
			return result;
		}
	}
}