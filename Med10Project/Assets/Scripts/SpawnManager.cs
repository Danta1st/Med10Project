using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour 
{

	#region Editor Publics
	[SerializeField] GameObject spawnObject;
	#endregion

	#region Privates
	private BpmManager bManager;
	private GestureManager gManager;
	private GA_Submitter gaSubmitter;

	private bool isOccupied = false;
	#endregion
	
	void Awake()
	{
		bManager = GameObject.Find("BpmManager").GetComponent<BpmManager>();
		if(bManager == null)
			Debug.LogError("No BpmManager was found in the scene.");
		
		gManager = Camera.main.GetComponent<GestureManager>();
		if(gManager == null)
			Debug.LogError("No GestureManager was found on the main camera.");

		gaSubmitter = GameObject.Find("GA_Submitter").GetComponent<GA_Submitter>();
		if(gaSubmitter == null)
			Debug.LogError("No GA_Submitter was found in the scene");
	}
	
	void Start ()
	{
		gManager.OnSwipeRight += SpawnObjectRandom;
	}
	
	#region Class Methods
	private void SpawnObject()
	{

	}

	public void SpawnObjectRandom()
	{
		if(isOccupied == false)
		{
			gaSubmitter.PerformTest();
			//Get Random Angle
			float angle = Random.Range(0.0f, 360.0f);
			//Get Random Distance
			float distance = Random.Range(2.0f, 9.0f);
			//Rotate GameObject
			transform.Rotate(0, 0, angle);
			//Get Rotation
			Quaternion rotation = transform.rotation;
			//Get Position
			Vector3 position = gameObject.transform.up * distance;
			//Rotate Back
			transform.Rotate(0, 0, angle);
			//Instantiate Object
			Instantiate(spawnObject, position, rotation);
			//Set occupied
			isOccupied = true;
		}
	}

	public void AllowSpawning()
	{
		isOccupied = false;
	}
	#endregion
}
