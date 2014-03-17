using UnityEngine;
using System.Collections;

public class ParticleObjectMovement : MonoBehaviour {

	private Vector3[] waypointArray = new Vector3[4];

	void Start () {
		waypointArray[0] = transform.position;
		waypointArray[3] = new Vector3(0,0,0);
		waypointArray[2] = new Vector3(XOffset(2), YOffset(2), transform.position.z*0.33f);
		waypointArray[1] = new Vector3(XOffset(1), YOffset(1), transform.position.z*0.66f);

		iTween.MoveTo(gameObject, iTween.Hash("path", waypointArray, "time", 0.5f, "easetype", iTween.EaseType.easeInCirc, "oncomplete", "DestroyObject"));
	}

	float XOffset(int WayPointNumber)
	{
		// float distance = waypointArray[WayPointNumber].normalized;
		//TODO: adjust randomerange values depending on length
		if(WayPointNumber == 2)
		{
			return transform.position.x*0.33f+Random.Range(-1.1f,1.1f);
		}
		else if(WayPointNumber == 1)
		{
			return transform.position.x*0.66f+Random.Range(-0.5f,0.5f);
		}
		return 0.0f;
	}

	float YOffset(int WayPointNumber)
	{
		if(WayPointNumber == 2)
		{
			return transform.position.y*0.33f+Random.Range(-1.1f,1.1f);
		}
		else if(WayPointNumber == 1)
		{
			return transform.position.y*0.66f+Random.Range(-0.5f,0.5f);
		}
		return 0.0f;
	}

	void DestroyObject()
	{
		Destroy (gameObject, 1.0f);
	}
}
