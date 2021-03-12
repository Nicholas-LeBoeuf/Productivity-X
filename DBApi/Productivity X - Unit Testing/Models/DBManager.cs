using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Productivity_X___Unit_Testing.Models
{
	class DBManager
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

		// Saves user info to database
		public bool SaveUser(UserCreateAccount uc)
		{
			bool bRet = true;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand CheckUser = conn.CreateCommand();

				// Checks to see if there are duplicate usernames
				CheckUser.Parameters.AddWithValue("@username", uc.UserName);
				CheckUser.CommandText = "select count(*) from Calendar_Schema.user_tbl where userName = @userName";

				// if 1 then already exist
				int UserExist = Convert.ToInt32(CheckUser.ExecuteScalar());

				if (UserExist >= 1)
				{
					bRet = false;
				}
				else
				{
					// Hash password
					uc.password = BCrypt.Net.BCrypt.HashPassword(uc.password);
					uc.confirmPassword = BCrypt.Net.BCrypt.HashPassword(uc.confirmPassword);

					// Inserting data into fields of database
					MySqlCommand Query = conn.CreateCommand();
					Query.CommandText = "insert into Calendar_Schema.user_tbl (firstname, lastname, username, email, password, confirmpassword, verificationcode) VALUES (@firstname,@lastname, @username, @email, @password, @confirmpassword, @verificationcode)";
					//					Query.Parameters.AddWithValue("@userID", uc.userID);
					Query.Parameters.AddWithValue("@firstname", uc.fname);
					Query.Parameters.AddWithValue("@lastname", uc.lname);
					Query.Parameters.AddWithValue("@username", uc.UserName);
					Query.Parameters.AddWithValue("@email", uc.email);
					Query.Parameters.AddWithValue("@password", uc.password);
					Query.Parameters.AddWithValue("@confirmpassword", uc.confirmPassword);
					Query.Parameters.AddWithValue("@verificationcode", " ");

					Query.ExecuteNonQuery();
				}
			}
			return bRet;
		}

		public int GetUserID()
		{
			int nUserID = -1;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand FindUser = conn.CreateCommand();

				// Checks to see if there are duplicate usernames
				FindUser.Parameters.AddWithValue("@username", DBObject.username);
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
						DBObject.id = nUserID;
					}
				}
				reader.Close();
			}
			return nUserID;
		}

		// Gets the userid from Database
		public int GetUserID(ForgotPassword forgotpassword)
		{
			int nUserID = -1;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand FindUser = conn.CreateCommand();

				// Checks to see if there are duplicate usernames
				FindUser.Parameters.AddWithValue("@username", DBObject.username);
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
						DBObject.id = nUserID;
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
				CheckData.Parameters.AddWithValue("@username", DBObject.username);
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
						DBObject.id = Convert.ToInt32(values[0]);
						dbUser.password = values[1].ToString();

						bRet = true;
					}
				}
				reader.Close();
			}

			return bRet;
		}


		public string GetUserName()
		{
			string sRet = "";
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				// Inserting data into fields of database
				MySqlCommand FindUsername = conn.CreateCommand();
				FindUsername.CommandText = "select username from calendar_schema.user_tbl where user_id = @userID;";
				FindUsername.Parameters.AddWithValue("@userID", DBObject.id);
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

		public string GetPassword()
		{
			string sRet = "";
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				// Inserting data into fields of database
				MySqlCommand FindPassword = conn.CreateCommand();
				FindPassword.CommandText = "select password from calendar_schema.user_tbl where user_id = @userID;";
				FindPassword.Parameters.AddWithValue("@userID", DBObject.id);
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
				CheckData.Parameters.AddWithValue("@username", DBObject.username);
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
						DBObject.id = Convert.ToInt32(values[0]);
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
		// Update password and confirm password fields in database
		public void UpdatePassword(ForgotPassword forgotPassword)
		{
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				// Inserting data into fields of database
				MySqlCommand Query = conn.CreateCommand();
				Query.CommandText = "update Calendar_Schema.user_tbl set password = @newpassword, confirmpassword = @confirmpassword where (user_id = @userid)";

				// Hash passwords
				forgotPassword.password = BCrypt.Net.BCrypt.HashPassword(forgotPassword.password);
				forgotPassword.confirmPassword = BCrypt.Net.BCrypt.HashPassword(forgotPassword.confirmPassword);
				Query.Parameters.AddWithValue("@newpassword", forgotPassword.password);
				Query.Parameters.AddWithValue("@confirmpassword", forgotPassword.confirmPassword);
				Query.Parameters.AddWithValue("@userid", DBObject.id);
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
				Query.Parameters.AddWithValue("@userID", DBObject.id);
				Query.Parameters.AddWithValue("@verificationcode", sCode);

				Query.ExecuteNonQuery();
			}
		}

		// Get the security code from user table in database
		public bool CheckSecurityCode(ForgotPassword forgotpassword)
		{
			bool bRet = false;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				// Inserting data into fields of database
				MySqlCommand FindSecurityCode = conn.CreateCommand();
				FindSecurityCode.CommandText = "SELECT verificationcode FROM Calendar_Schema.user_tbl where user_id = @userid";
				FindSecurityCode.Parameters.AddWithValue("@userID", DBObject.id);
				FindSecurityCode.ExecuteNonQuery();

				// Execute the SQL command against the DB:
				MySqlDataReader reader = FindSecurityCode.ExecuteReader();
				if (reader.Read()) // Read returns false if the verificationcode does not exist!
				{
					// Read the DB values:
					Object[] values = new object[1];
					int fieldCount = reader.GetValues(values);
					if (1 == fieldCount && values[0].ToString() == forgotpassword.verificationCode)
					{
						bRet = true;
					}
				}
				reader.Close();
			}
			return bRet;
		}


	//-----------CreateEventButton, delete and edit, create new event (fill in criteria)----------------

		// Saves event info to database
		public bool SaveEvent(CreateEvent ce)
		{
			bool bRet = true;
			using (MySqlConnection conn = GetConnection())
			{
				int nEventExists = 0;
				conn.Open();
				MySqlCommand CheckEvent = conn.CreateCommand();

				// Checks to see if there are duplicate values and if event already created
				CheckEvent.Parameters.AddWithValue("@event_name", ce.m_sEventName);
				CheckEvent.Parameters.AddWithValue("@event_date", Convert.ToDateTime(ce.m_sEventDate));
				CheckEvent.Parameters.AddWithValue("@start_at", ce.m_sTimeStartAt);
				CheckEvent.Parameters.AddWithValue("@end_at", ce.m_sTimeEndAt);
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
						"location, description, color, guest, friend) VALUES (@user_id, @eventname, @event_date, @start_at, @end_at, @notification, @reminder, @location, @description, " +
						"@color, @guest, @friend)";

					Query.Parameters.AddWithValue("@user_id", DBObject.id);
					Query.Parameters.AddWithValue("@eventname", ce.m_sEventName);
					// Saved as date
					Query.Parameters.AddWithValue("@event_date", Convert.ToDateTime(ce.m_sEventDate));
					// Saved as time
					Query.Parameters.AddWithValue("@start_at", ce.m_sTimeStartAt);
					// Saved as time
					Query.Parameters.AddWithValue("@end_at", ce.m_sTimeEndAt);
					Query.Parameters.AddWithValue("@notification", ce.m_nNotification);
					Query.Parameters.AddWithValue("@reminder", ce.m_nReminder);
					Query.Parameters.AddWithValue("@location", ce.m_sLocation);
					Query.Parameters.AddWithValue("@description", ce.m_sDescription);
					Query.Parameters.AddWithValue("@color", ce.m_sColor);
					Query.Parameters.AddWithValue("@guest", ce.m_bGuest);
					Query.Parameters.AddWithValue("@friend", ce.m_bIsFriend);

					Query.ExecuteNonQuery();

					if (ce.m_nNotification == true)
					{
						// Send notification to user via email based upon the current time and amount of minutes before starting time
					}

					if (ce.m_bGuest)
					{
						MySqlCommand InsertIntoGuestTable = conn.CreateCommand();
						InsertIntoGuestTable.CommandText = "insert into Calendar_Schema.guest_tbl (user_id, event_id, guest_username, guest_email, isFriend) VALUES (@user_id, @eventid, @guestusername, " +
							"@guestemail, @isfriend)";


						InsertIntoGuestTable.Parameters.AddWithValue("@user_id", DBObject.id);
						InsertIntoGuestTable.Parameters.AddWithValue("@eventid", GetEventID(ce.m_sEventName, ce.m_sEventDate, ce.m_sTimeStartAt, ce.m_sTimeEndAt));
						InsertIntoGuestTable.Parameters.AddWithValue("@guestusername", ce.m_sGuestUsername);
						InsertIntoGuestTable.Parameters.AddWithValue("@guestemail", ce.m_sGuestEmail);
						InsertIntoGuestTable.Parameters.AddWithValue("@isfriend", ce.m_bIsFriend);
						InsertIntoGuestTable.ExecuteNonQuery();
					}
				}
			}
			return bRet;
		}

		public int GetEventID(string eventname, string eventdate, string startat, string endat)
		{
			int nRet = 0;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				// Inserting data into fields of database
				MySqlCommand FindEventID = conn.CreateCommand();
				FindEventID.CommandText = "select event_ID from calendar_schema.events_tbl where user_id = @userID and eventname = @eventname and event_date = @event_date " +
					"and start_at = @start_at and end_at = @end_at";
				FindEventID.Parameters.AddWithValue("@userID", DBObject.id);
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
						DBObject.eventid = nRet;
					}
				}
				reader.Close();
			}
			return nRet;
		}

		public bool EditEvent(EditEvent ee)
		{
			bool bRet = true;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				MySqlCommand UpdateEvent = conn.CreateCommand();
				UpdateEvent.CommandText = "update Calendar_Schema.events_tbl set user_id = @user_id, event_date = @event_date, " +
					"start_at = @start_at, end_at = @end_at, notification = @notification, reminder = @reminder, location = @location, description = @description, color = @color, " +
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
				UpdateEvent.Parameters.AddWithValue("@color", ee.m_sColor);
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

		public bool DeleteEvent()
		{
			bool bRet = true;
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();
				MySqlCommand CheckData = conn.CreateCommand();
				// Checks to see if user invited any guests or friends
				CheckData.Parameters.AddWithValue("@inviteguest", true);
				CheckData.CommandText = "SELECT guest FROM Calendar_schema.events_tbl where guest = @inviteguest";
				CheckData.ExecuteNonQuery();

				// Execute the SQL command against the DB:
				MySqlDataReader reader = CheckData.ExecuteReader();
				if (reader.Read())
				{			
					MySqlCommand deleteGuestRow = conn.CreateCommand();
					CheckData.Parameters.AddWithValue("@eventid", DBObject.eventid);
					deleteGuestRow.CommandText = "delete FROM Calendar_Schema.guest_tbl where event_id = @eventid";
					deleteGuestRow.ExecuteNonQuery();	
				}
				reader.Close();
				MySqlCommand deleteEventRow = conn.CreateCommand();
				deleteEventRow.Parameters.AddWithValue("@eventid", DBObject.eventid);
				deleteEventRow.CommandText = "delete FROM Calendar_Schema.events_tbl where event_id = @eventid";
				deleteEventRow.ExecuteNonQuery();
			}
			return bRet;
		}

		// Not sure how we want to implement below, but can call the GetEventID function with parameters
		public List<string> FindEventInfo(int eventid)
		{
			List<string> eventData = new List<string>();
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				MySqlCommand UpdateEvent = conn.CreateCommand();
				UpdateEvent.CommandText = "select * from Calendar_Schema.events_tbl where user_id = @userid and event_id = @eventid";
				UpdateEvent.Parameters.AddWithValue("@userid", DBObject.id);
				UpdateEvent.Parameters.AddWithValue("@eventid", eventid);//DBObject.eventid);
				UpdateEvent.ExecuteNonQuery();

				// Execute the SQL command against the DB:
				MySqlDataReader reader = UpdateEvent.ExecuteReader();
/*				while (reader.Read())
				{
					eventData.Add(Convert.ToString(reader[0]));
				}
*/
				if (reader.Read()) // Read returns false if the verificationcode does not exist!
				{
					// Read the DB values:
					Object[] values = new object[13];
					int fieldCount = reader.GetValues(values);
					if (13 == fieldCount)
					{
						for(int counter = 0; counter < values.Length; counter++)
						{
							eventData.Add(values[counter].ToString());
						}

						// Successfully retrieved the user from the DB:
						// string to bool...  = bool.Parse(eventdata)
						// string to int....  = Convert.ToInt32(values[0]);
					}
				}

				reader.Close();
			}
			return eventData;
		}

		//-----------TodayButton, find events with todays date, pass back event id----------------
		public List<string> FindTodaysEvents(string todaysDate)
		{
			List<string> eventData = new List<string>();
			using (MySqlConnection conn = GetConnection())
			{
				conn.Open();

				MySqlCommand UpdateEvent = conn.CreateCommand();
				UpdateEvent.CommandText = "select event_id, event_name from Calendar_Schema.events_tbl where user_id = @userid and event_date = @eventdate";
				UpdateEvent.Parameters.AddWithValue("@userid", DBObject.id);
				UpdateEvent.Parameters.AddWithValue("@eventdate", todaysDate);
				UpdateEvent.ExecuteNonQuery();

				// Execute the SQL command against the DB:
				MySqlDataReader reader = UpdateEvent.ExecuteReader();
				while (reader.Read())
				{
					eventData.Add(Convert.ToString(reader[0]));
				}


				Object[] values = new object[1];
				if (reader.Read()) // Read returns false if the verificationcode does not exist!
				{
					nRetID = Convert.ToInt32(values[0]);
				}

				reader.Close();
			}
			return nRetID;
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
	}
}
