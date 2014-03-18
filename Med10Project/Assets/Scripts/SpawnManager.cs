using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour 
{

	#region Editor Publics
	[SerializeField] GameObject spawnObject;
	#endregion

	#region Privates
	private GestureManager gManager;
	private bool isOccupied = false;
	private int objectCounter = 0;
	private int successCounter = 0;
	#endregion
	
	void Awake()
	{
		gManager = Camera.main.GetComponent<GestureManager>();
		if(gManager == null)
			Debug.LogError("No GestureManager was found on the main camera.");
	}
	
	void Start ()
	{
		//TODO: Remove on delivery
		//gManager.OnSwipeRight += SpawnObjectRandom;
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Restart");
	}
	
	#region Class Methods
	private void SpawnObject()
	{

	}

	public void SpawnObjectRandom()
	{
		if(isOccupied == false)
		{
			//Get Random multiplier
			int multiplier = Random.Range(0, 35);
			//Get Random Angle
			int angle = 10 * multiplier;
			//Get Random Distance
			float distance = Random.Range(2.0f, 9.0f);
			//Rotate GameObject
			transform.Rotate(0, 0, (float) angle);
			//Get Rotation
			Quaternion rotation = transform.rotation;
			//Get Position
			Vector3 position = gameObject.transform.up * distance;
			//Rotate Back
			transform.Rotate(0, 0, angle);
			//Update Counter
			objectCounter++;
			//Instantiate Object
			GameObject go = (GameObject) Instantiate(spawnObject, position, rotation);
			//Set Object parameters
			go.GetComponent<ObjectHandler>().SetAngle((int) angle);
			go.GetComponent<ObjectHandler>().SetID(objectCounter);
			go.GetComponent<ObjectHandler>().SetDistance(distance);
			go.GetComponent<ObjectHandler>().SetSpawnTime(Time.time);
			//Set occupied
			isOccupied = true;
		}
	}

	public void AllowSpawning()
	{
		isOccupied = false;
	}
	public void IncreaseSucces()
	{
		successCounter++;

		if(successCounter > 10)
		{
			Application.Quit();
		}
	}

	private void NC_Restart()
	{
		successCounter = 0;
	}
	#endregion
}
