using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MySQL_Test : MonoBehaviour {

	#region Privates
	// MySQL connection string
	private string constr = "Server=localhost;Database=demo;User ID=demo;Password=demo;Pooling=true";
	// MySQL Objects
	private MySqlConnection con = null;
	private MySqlCommand cmd = null;
	private MySqlDataReader rdr = null;

	// Standard table names
	private enum tables
	{
		TestUsers,
		Session
	}
	// User Names
	private List<string> userNames = new List<string>();
	#endregion

	// Use this for initialization
	void Start () {
		userNames = GetUsers();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Keypad9))
		{
			GetUsers();
		}
	}
	
	void OnApplicationQuit()
	{
		CloseConnection();
	}

	#region Public Methods
	// Method for creating a new User Table, //TODO: Make check to see if table already exists
	public void CreateUserTable()
	{
		OpenConnection();
		var tableName = tables.TestUsers.ToString();
		string[] columnNames = {"userID","userName","completedSessions"};
		string[] columnTypes = {"int PRIMARY KEY AUTO_INCREMENT NOT NULL","VARCHAR(25) NOT NULL", "int NOT NULL"};
		CreateTable(tableName,columnNames,columnTypes);
		CloseConnection();
	}

	public void CreateSessionTable()
	{
	}

	// Method for getting all userNames registered in the database
	public List<string> GetUsers()
	{
		OpenConnection();
		var tableName = tables.TestUsers.ToString();
		string query = "SELECT userName FROM "+tableName;
		cmd = new MySqlCommand(query, con);
		rdr = cmd.ExecuteReader();

		List<string> readArray = new List<string>();

		while(rdr.Read())
		{
			readArray.Add(rdr.GetString(rdr.GetOrdinal("userName")));
		}
		CloseConnection();
		return readArray;
	}

	// Method for getting the amount of completed sessions by a specific user
	public int GetUserSessionCount(string userName)
	{
		OpenConnection();

		int sessions = -1;
		string query = "SELECT completedSessions FROM "+tables.TestUsers.ToString()+" WHERE userName = '"+ userName +"'";

		cmd = new MySqlCommand(query, con);
		rdr = cmd.ExecuteReader();

		while(rdr.Read())
		{
			sessions = rdr.GetInt32(rdr.GetOrdinal("completedSessions"));
		}
		CloseConnection();

		return sessions;
	}

	public void GetUserData()
	{
		//TODO: GetUserData() Method
	}

	//Method for adding a new user to the Database
	public void AddNewUser(string userName)
	{
		OpenConnection();
		string[] userDetails = {"0","'"+userName+"'", ""+0};
		InsertInto(tables.TestUsers.ToString(), userDetails);
		CloseConnection();
	}
	#endregion

	#region Class Methods
	private void OpenConnection()
	{
		try
		{
			// setup the connection element
			con = new MySqlConnection(constr);
			
			// lets see if we can open the connection
			con.Open();
		}
		catch (Exception ex)
		{
			//TODO: Make user prompt, informing of playing without data recording
			Debug.Log(ex.ToString());
		}
	}

	private void CloseConnection()
	{
		if (con != null)
		{
			if (con.State.ToString() != "Closed")
				con.Close();
			con.Dispose();
		}
	}

	//TODO: Needs testing.
	private MySqlDataReader BasicQuery(string query, bool _return)
	{
		cmd = new MySqlCommand(query, con);
		rdr = cmd.ExecuteReader();

		if(_return){
			return rdr;
		}
		else {
			return null;
		}
	}

	//Method for creating a table. Remember to open and close connection when using. //FIXME: implement try-catch error catching
	private void CreateTable(string tableName, string[] columns, string[] columnTypes)
	{
		string query = "CREATE TABLE " + tableName + "(" + columns[0] + " " + columnTypes[0];
		for(int i = 1; i < columns.Length; i++)
		{
			query += ", " + columns[i] + " " + columnTypes[i];
		}
		query += ")";
		cmd = new MySqlCommand(query, con);
		cmd.ExecuteReader();
		Debug.Log("Created table: "+tableName);
	}
	
	//TODO: Needs testing, remember to open connection beforehand
	private void InsertIntoSingle(string tableName, string columnName, string value)
	{
		string query = "INSERT INTO " + tableName + "(" + columnName + ") " + "VALUES (" + value + ")";
		cmd = new MySqlCommand(query, con);
		cmd.ExecuteReader();
	}

	//TODO: Needs testing, remember to open connection beforehand, remmeber to mark string values with ''
	private void InsertIntoSpecific(string tableName, ArrayList colums, ArrayList values)
	{
		string query = "INSERT INTO " + tableName + "(" + colums[0];
		for(int i = 1; i < colums.Count; i++)
		{
			query += "; " + colums[i];
		}
		query += ") VALUES (" + values[0];
		for(int i = 1; i < colums.Count; i++)
		{
			query += "; " + values[i];
		}
		query += ")";

		cmd = new MySqlCommand(query, con);
		cmd.ExecuteReader();
	}

	//TODO: Needs testing, remember to open connection beforehand
	private void InsertInto(string tableName, string[] values)
	{
		string query = "INSERT INTO " + tableName + " VALUES (" + values[0];
		for(int i = 1; i < values.Length; i++)
		{
			query += ", " + values[i];
		}
		query += ")";
		cmd = new MySqlCommand(query, con);
		cmd.ExecuteReader();
	}

	//TODO: Needs testing, remember to open connection beforehand, remember to place string values in ''
	private string SingleSelectWhere(string tableName, string itemToSelect, 
	                               string whereColumn, string whereParam, string whereValue)
	{
		string query = "SELECT " + itemToSelect + " FROM " + tableName + " WHERE " + whereColumn + whereParam + whereValue;
		cmd = new MySqlCommand(query, con);
		rdr = cmd.ExecuteReader();
		string readValue = "";
		while(rdr.Read())
		{
			readValue = rdr.GetString(0);
		}
		return readValue;
	}

	private int SingleSelectIntWhere(string tableName, string itemToSelect, 
	                                 string whereColumn, string whereParam, string whereValue)
	{
		string query = "SELECT " + itemToSelect + " FROM " + tableName + " WHERE " + whereColumn + whereParam + whereValue;
		cmd = new MySqlCommand(query, con);
		rdr = cmd.ExecuteReader();
		int readValue = -1;
		while(rdr.Read())
		{
			readValue = rdr.GetInt32(0);
		}
		return readValue;
	}



//	string createUserTableQuery = "CREATE TABLE Users(UserID int NOT NULL, Name VARCHAR(25) NOT NULL, CompletedSessions int, PRIMARY KEY(UserID))";
//	private void CreateTable(string query)
//	{
//		// Error trapping in the simplest form
//		try
//		{
//			if (con == null || con.State.ToString() != "Open")
//				OpenConnection(constr);
//			using (con)
//			{
//				using (cmd = new MySqlCommand(query, con))
//				{
//					cmd.ExecuteReader();
//					Debug.Log("Created Table: Users");
//				}
//			}
//		}
//		catch (Exception ex)
//		{
//			if(ex != null)
//				Debug.Log(ex.ToString());
//			else
//				Debug.Log("Exeption in Creating User Table");
//		}
//		finally
//		{
//			CloseConnection();
//		}
//	}
	
//	// Insert new entries into the table
//	void InsertEntries()
//	{
//		string query = string.Empty;
//		// Error trapping in the simplest form
//		try
//		{
//			query = "INSERT INTO demo_table (ID, Name, levelname, objectType, posx, posy, tranx, trany) VALUES (?ID, ?Name, ?levelname, ?objectType, ?posx, ?posy, ?tranx, ?trany)";
//			if (con.State.ToString() != "Open")
//				con.Open();
//			using (con)
//			{
//				foreach (data itm in _GameItems)
//				{
//					using (cmd = new MySqlCommand(query, con))
//					{
//
//					}
//				}
//			}
//		}
//		catch (Exception ex)
//		{
//			Debug.Log(ex.ToString());
//		}
//		finally
//		{
//		}
//	}

	#endregion
}
