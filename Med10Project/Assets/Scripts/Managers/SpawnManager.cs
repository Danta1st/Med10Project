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

	private bool isOccupied = false;

	private int objectCounter = 0;
	#endregion
	
	void Awake()
	{
		bManager = GameObject.Find("BpmManager").GetComponent<BpmManager>();
		if(bManager == null)
			Debug.LogError("No BpmManager was found in the scene.");
		
		gManager = Camera.main.GetComponent<GestureManager>();
		if(gManager == null)
			Debug.LogError("No GestureManager was found on the main camera.");
	}
	
	#region Public Methods
	public void SpawnSpecific(int int1to10)
	{
		if(isOccupied == false)
		{
			//Get specific angle
			int angle = GetAngle(int1to10);
			//Get Random distance
			float distance = Random.Range(2.0f, 8.5f);
			//Rotate self by specific angle
			RotateSelf(angle);
			//Record spawn rotation & Position
			Quaternion rotation = transform.rotation;
			Vector3 position = gameObject.transform.up * distance;
			//Rotate self back by specific angle
			RotateSelf(-angle);
			//Instantiate game object
			GameObject go = (GameObject) Instantiate(spawnObject, position, rotation);
			//Set Object Parameters
			go.GetComponent<ObjectHandler>().SetAngle((int) angle);
			go.GetComponent<ObjectHandler>().SetID(objectCounter);
			go.GetComponent<ObjectHandler>().SetDistance(distance);
			go.GetComponent<ObjectHandler>().SetSpawnTime(Time.time);
			//Set occupied
			isOccupied = true;
		}
	}
	public void SpawnSpecific(int int1to10, float distance)
	{
		if(isOccupied == false)
		{
			//Get specific angle
			int angle = GetAngle(int1to10);
			//Rotate self by specific angle
			RotateSelf(angle);
			//Record spawn rotation & Position
			Quaternion rotation = transform.rotation;
			Vector3 position = gameObject.transform.up * distance;
			//Rotate self back by specific angle
			RotateSelf(-angle);
			//Instantiate game object
			GameObject go = (GameObject) Instantiate(spawnObject, position, rotation);
			//Set Object Parameters
			go.GetComponent<ObjectHandler>().SetAngle((int) angle);
			go.GetComponent<ObjectHandler>().SetID(objectCounter);
			go.GetComponent<ObjectHandler>().SetDistance(distance);
			go.GetComponent<ObjectHandler>().SetSpawnTime(Time.time);
			//Set occupied
			isOccupied = true;
		}
	}

	public void SpawnRandom()
	{
		if(isOccupied == false)
		{
			//Get Random multiplier
			int multiplier = Random.Range(1, 10);
			//Get Random Angle
			int angle = GetAngle(multiplier);
			//Get Random Distance
			float distance = Random.Range(2.0f, 8.5f);
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

	public void SpawnRightRandom()
	{

	}
	public void SpawnLeftRandom()
	{

	}
	public void SpawnRightSpecific(int int1to5)
	{

	}
	public void SpawnLeftSpecific(int int1to5)
	{

	}

	public void AllowSpawning()
	{
		isOccupied = false;
	}
	#endregion

	#region Class Methods
	private int GetAngle(int int1to10)
	{
		int angle = 36 * int1to10 - 18;
		return angle;
	}

	private void RotateSelf(int angle)
	{
		transform.Rotate(0, 0, (float) angle);
	}
	#endregion
}
