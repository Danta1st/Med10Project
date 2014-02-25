using UnityEngine;
using System.Collections;

public class GA_Submitter : MonoBehaviour {

	[SerializeField] private bool SendData = false;

	private string userID = "Danny";
	private float testCounter = 0;

	void Start()
	{
		if(SendData == true)
			GA.SettingsGA.SetCustomUserID(userID);
	}

	public void PerformTest()
	{
		testCounter++;
		GA.API.Design.NewEvent("Tests:MainTest:T", testCounter);
		GA_Queue.ForceSubmit();
	}

	public void Succes(Transform _transform, int angle, int ID, float distance)
	{
		GA.API.Design.NewEvent("Complete:"+ID+":Succes", testCounter, _transform.position);
		GA.API.Design.NewEvent("Complete:"+ID+":Time", Time.timeSinceLevelLoad);
		GA.API.Design.NewEvent("Complete:"+ID+":Position", _transform.position);
		GA.API.Design.NewEvent("Complete:"+ID+":Angle", (float) angle);
		GA.API.Design.NewEvent("Complete:"+ID+":Distance", distance);

		GA.API.Design.NewEvent("Complete:Time:"+ID, Time.timeSinceLevelLoad);
		GA.API.Design.NewEvent("Complete:Angle:"+ID, angle);
		GA.API.Design.NewEvent("Complete:Distance:"+ID, distance);
		GA.API.Design.NewEvent("Complete:Position:"+ID, _transform.position);
		GA_Queue.ForceSubmit();
	}	
}
