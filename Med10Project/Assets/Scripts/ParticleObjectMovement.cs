using UnityEngine;
using System.Collections;

public class ParticleObjectMovement : MonoBehaviour {

	/*private Transform[] waypointArray;
	float percentsPerSecond = 0.02f; // %2 of the path moved per second
	float currentPathPercent = 0.0f; //min 0, max 1
	
	void Update () 
	{
		currentPathPercent += percentsPerSecond * Time.deltaTime;
		iTween.PutOnPath(gameObject, waypointArray, currentPathPercent);
	}*/

	void Start () {
		iTween.MoveTo(gameObject, iTween.Hash("position", Vector3.zero, "time", .5f, "easetype", iTween.EaseType.easeInBack, "oncomplete", "DestroyObject"));
	
	}

	void DestroyObject()
	{
		Destroy (gameObject, 1.0f);
	}
}
