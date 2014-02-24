using UnityEngine;
using System.Collections;

public class GA_Submitter : MonoBehaviour {

	private float testCounter = 0;

	public void PerformTest()
	{
		testCounter++;
		GA.API.Design.NewEvent("Tests:MainTest:T", testCounter);
		GA_Queue.ForceSubmit();
	}

	public void Succes(Transform _transform)
	{
		GA.API.Design.NewEvent("Tests:Complete:Succes", _transform.position);
		GA_Queue.ForceSubmit();
	}	
}
