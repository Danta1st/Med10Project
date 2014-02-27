using UnityEngine;
using System.Collections;

public class GA_Submitter : MonoBehaviour {

	[SerializeField] private bool SendData = false;

	private string userID = "Danny";
	private int sessionID = 0; 

	void Start()
	{
		if(SendData == true)
		{
			BeginTracing();
		}
	}

	private void BeginTracing()
	{
		//Check for userID and sessionID in playerPrefs
		if(PlayerPrefs.HasKey(userID))
			sessionID = PlayerPrefs.GetInt(userID) +1;

		//Update userID and sessionID in playerPrefs
		PlayerPrefs.SetInt(userID, sessionID);
		//Set userID in GameAnalytics
		GA.SettingsGA.SetCustomUserID(userID);
	}

	public void Angle(int ID, int angle)
	{
		if(SendData == true)
		{
			//GA.API.Design.NewEvent("Angle:Session"+sessionID+":"+userID, angle);
			GA.API.Design.NewEvent("Angle:Session"+sessionID+":"+userID+":"+ID, angle);
		}
	}

	public void Distance(int ID, float distance)
	{
		if(SendData == true)
		{
			//GA.API.Design.NewEvent("Distance:Session"+sessionID+":"+userID, distance);
			GA.API.Design.NewEvent("Distance:Session"+sessionID+":"+userID+":"+ID, distance);
		}
	}

	public void PositionSucces(int ID, Vector3 position)
	{
		if(SendData == true)
		{
			//Succes positions
			GA.API.Design.NewEvent("Success:Session"+sessionID+":"+userID, position);
		}
	}
	public void PositionFailed(int ID, Vector3 position)
	{
		if(SendData == true)
		{
			//Failed positions
			GA.API.Design.NewEvent("Failed:Session"+sessionID+":"+userID, position);
		}
	}

	public void CompletionTime(int ID, float time)
	{
		if(SendData == true)
		{
			//GA.API.Design.NewEvent("Time:Session"+sessionID+":"+userID, time);
			GA.API.Design.NewEvent("Time:Session"+sessionID+":"+userID+":"+ID, time);
		}
	}

	public void ForceSubmit()
	{
		if(SendData == true)
			GA_Queue.ForceSubmit();
	}
}
