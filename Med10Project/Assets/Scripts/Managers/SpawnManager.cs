using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour 
{

	#region Editor Publics
	[SerializeField] GameObject spawnObject;
	[SerializeField] GameObject spawnObject2;
	#endregion

	#region Privates
	private GestureManager gManager;

	private bool isOccupied = false;

	private int objectCounter = 0;

	private float[] distances = new float[10] {2, 2, 2, 2, 2, 2, 2, 2, 2, 2};
	private float distanceChange = 2f;

	#endregion
	
	void Awake()
	{
		gManager = Camera.main.GetComponent<GestureManager>();
		if(gManager == null)
			Debug.LogError("No GestureManager was found on the main camera.");
	}
	
	#region Public Methods
	public void IncreaseDistanceInArray(int angle)
	{
		distances[angle] = distances[angle]+distanceChange;
		Debug.Log("Sat angle " + angle + " to " + distances[angle]);
	}

	public void DecreaseDistanceInArray(int angle)
	{
		distances[angle] = distances[angle]-distanceChange;
		Debug.Log("Sat angle " + angle + " to " + distances[angle]);
	}

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
		go.name = go.name+int1to10;
		//Set occupied
		isOccupied = true;
	}

	public void SpawnXAmount(int amountOfTargets, float distance)
	{
		for(int i = 1; i <= amountOfTargets; i++)
		{
			SpawnSpecific(i, distance);
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
			RotateSelf(angle);
			//Get Rotation
			Quaternion rotation = transform.rotation;
			//Get Position
			Vector3 position = gameObject.transform.up * distance;
			//Rotate Back
			RotateSelf(-angle);
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

	public void Phase1Stage1()
	{
		if(isOccupied == false)
		{
			//Get Random multiplier
			int multiplier = Random.Range(1, 10);
			//Get Random Angle
			int angle = GetAngle(multiplier);
			//Rotate GameObject
			RotateSelf(angle);
			//Get Rotation
			Quaternion rotation = transform.rotation;
			//Get Position
			Vector3 position = gameObject.transform.up * distances[multiplier];
			//Rotate Back
			RotateSelf(-angle);
			//Update Counter
			objectCounter++;
			//Instantiate Object
			GameObject go = (GameObject) Instantiate(spawnObject, position, rotation);
			//Set Object parameters
			go.GetComponent<ObjectHandler>().SetAngle((int) angle);
			go.GetComponent<ObjectHandler>().SetID(objectCounter);
			go.GetComponent<ObjectHandler>().SetDistance(distances[multiplier]);
			go.GetComponent<ObjectHandler>().SetSpawnTime(Time.time);
			go.GetComponent<ObjectHandler>().SetMultiplier(multiplier);
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
		int angle = (36 * int1to10) - 18;
		return angle;
	}

	private void RotateSelf(int angle)
	{
		transform.Rotate(0, 0, (float) -angle);
	}
	#endregion
}
