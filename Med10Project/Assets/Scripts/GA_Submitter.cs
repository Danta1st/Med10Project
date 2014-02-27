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
			//Best logic
			GA.API.Design.NewEvent("Angle:"+sessionID+":"+userID, angle);
			GA.API.Design.NewEvent("Angle:"+sessionID+":"+userID+":"+ID, angle);
			
			//Test logic
			GA.API.Design.NewEvent(userID+":"+sessionID+":Angle", angle);
			GA.API.Design.NewEvent(userID+":"+sessionID+":Angle:"+ID, angle);
			
			//More test logic
			GA.API.Design.NewEvent("Complete:Angle:"+ID, angle);
		}
	}

	public void Distance(int ID, float distance)
	{
		if(SendData == true)
		{
			//Best logic
			GA.API.Design.NewEvent("Distance:"+sessionID+":"+userID, distance);
			GA.API.Design.NewEvent("Distance:"+sessionID+":"+userID+":"+ID, distance);
			
			//Test logic
			GA.API.Design.NewEvent(userID+":"+sessionID+":Distance", distance);
			GA.API.Design.NewEvent(userID+":"+sessionID+":Distance:"+ID, distance);
			
			//More test logic
			GA.API.Design.NewEvent("Complete:Distance:"+ID, distance);
		}
	}

	public void Position(int ID, Vector3 position)
	{
		if(SendData == true)
		{
			//Best logic
			GA.API.Design.NewEvent("Position:"+sessionID+":"+userID, position);
			GA.API.Design.NewEvent("Position:"+sessionID+":"+userID+":"+ID, position);
			
			//Test logic
			GA.API.Design.NewEvent(userID+":"+sessionID+":Position", position);
			GA.API.Design.NewEvent(userID+":"+sessionID+":Position:"+ID, position);
			
			//More test logic
			GA.API.Design.NewEvent("Complete:Position:"+ID, position);
		}
	}

	public void CompletionTime(int ID, float time)
	{
		if(SendData == true)
		{
			//Best logic
			GA.API.Design.NewEvent("Time:"+sessionID+":"+userID, time);
			GA.API.Design.NewEvent("Time:"+sessionID+":"+userID+":"+ID, time);

			//Test logic
			GA.API.Design.NewEvent(userID+":"+sessionID+":Time", time);
			GA.API.Design.NewEvent(userID+":"+sessionID+":Time:"+ID, time);

			//More test logic
			GA.API.Design.NewEvent("Complete:Time:"+ID, time);
		}
	}

	public void ForceSubmit()
	{
		if(SendData == true)
			GA_Queue.ForceSubmit();
	}
}
