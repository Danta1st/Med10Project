using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MySQL_Test : MonoBehaviour {
	
	bool isSaving = false;
	bool isLoading = false;

	// MySQL instance specific items
	string constr = "Server=localhost;Database=demo;User ID=demo;Password=demo;Pooling=true";
	// connection object
	MySqlConnection con = null;
	// command object
	MySqlCommand cmd = null;
	// reader object
	MySqlDataReader rdr = null;

	private ArrayList UserNames = new ArrayList();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Keypad9))
			CreateTable(createUserTableQuery);
	}

	#region Public Methods
	#endregion

	#region Class Methods
	private void OpenConnection(string connectionString)
	{
		try
		{
			// setup the connection element
			con = new MySqlConnection(connectionString);
			
			// lets see if we can open the connection
			con.Open();
			Debug.Log("Connection State: " + con.State);
		}
		catch (Exception ex)
		{
			//TODO: Make user prompt, informing of playing without data recording
			Debug.Log(ex.ToString());
		}
	}

	private void CloseConnection()
	{
		Debug.Log("killing con");
		if (con != null)
		{
			if (con.State.ToString() != "Closed")
				con.Close();
			con.Dispose();
		}
	}
	
	void OnApplicationQuit()
	{
		CloseConnection();
	}

	string getUsersQuery = "";
	private void SingleSelectWhere(string tableName, string itemToSelect, 
	                               string whereColumn, string whereParam, string whereValue)
	{
		string query = "SELECT " + itemToSelect + " FROM " + tableName + " WHERE " + whereColumn + whereParam + whereValue;

	}

	string createUserTableQuery = "CREATE TABLE Users(UserID int NOT NULL, Name VARCHAR(25) NOT NULL, CompletedSessions int, PRIMARY KEY(UserID))";
	private void CreateTable(string query)
	{
		//OpenConnection(constr);
		// Error trapping in the simplest form
		try
		{
			if (con == null || con.State.ToString() != "Open")
				OpenConnection(constr);
			using (con)
			{
				using (cmd = new MySqlCommand(query, con))
				{
					cmd.ExecuteReader();
					Debug.Log("Created Table: Users");
				}
			}
		}
		catch (Exception ex)
		{
			if(ex != null)
				Debug.Log(ex.ToString());
			else
				Debug.Log("Exeption in null thing");
		}
		finally
		{
			CloseConnection();
		}


	}
	
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
