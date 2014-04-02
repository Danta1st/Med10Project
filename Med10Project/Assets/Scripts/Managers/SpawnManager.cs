using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour 
{

	#region Editor Publics
	[SerializeField] SpawnObjects spawnObjects;
	[SerializeField] SpawnThresholds spawnThresholds;
	#endregion

	#region Privates
	private GestureManager gManager;
	private SoundManager sManager;

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

		sManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
	}
	
	#region Public Methods
	public void IncreaseDistanceInArray(int angle)
	{
		if(CheckMax(angle, distances[angle]+distanceChange))
			distances[angle] = distances[angle]+distanceChange;

		Debug.Log("Target: "+angle+" has a distance of: "+ distances[angle]);
	}

	public void DecreaseDistanceInArray(int angle)
	{
		if(CheckMin(angle, distances[angle]-distanceChange))
			distances[angle] = distances[angle]-distanceChange;

		Debug.Log("Target: "+angle+" has a distance of: "+ distances[angle]);
	}

	public void SpawnSpecific(int int1to10)
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
		GameObject go = (GameObject) Instantiate(spawnObjects.SingleTarget, position, rotation);
		//Play Sound
		sManager.PlayNewTargetSpawned();
		//Set Object Parameters
		go.GetComponent<ObjectHandler>().SetAngle((int) angle);
		go.GetComponent<ObjectHandler>().SetID(objectCounter);
		go.GetComponent<ObjectHandler>().SetDistance(distance);
		go.GetComponent<ObjectHandler>().SetSpawnTime(Time.time);
		go.GetComponent<ObjectHandler>().SetMultiplier(int1to10);
		go.name = go.name+int1to10;
		//Set occupied
		isOccupied = true;
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
		GameObject go = (GameObject) Instantiate(spawnObjects.SingleTarget, position, rotation);
		//Play Sound
		sManager.PlayNewTargetSpawned();
		//Set Object Parameters
		go.GetComponent<ObjectHandler>().SetAngle((int) angle);
		go.GetComponent<ObjectHandler>().SetID(objectCounter);
		go.GetComponent<ObjectHandler>().SetDistance(distance);
		go.GetComponent<ObjectHandler>().SetSpawnTime(Time.time);
		go.GetComponent<ObjectHandler>().SetMultiplier(int1to10);
		go.name = go.name+int1to10;
		//Set occupied
		isOccupied = true;
	}

	public void SpawnXTargets(int amountOfTargets, float distance)
	{
		for(int i = 1; i <= amountOfTargets; i++)
		{
			SpawnSpecific(i, distance);
		}
	}

	public void SpawnRandom()
	{
		int multiplier = Random.Range(1,10);
		float distance = Random.Range(2.0f, 8.5f);

		SpawnSpecific(multiplier, distance);
	}

	public void Phase1Stage1()
	{
		int multiplier = Random.Range(1,10);
		SpawnSpecific(multiplier, distances[multiplier]);
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

	//Method for checking if we are above maximum
	private bool CheckMax(int angle, float distance)
	{
		switch(angle)
		{
		case 1:
		case 5:
		case 6:
		case 10:
		{
			if(distance < spawnThresholds.MinMax_1_5_6_10.y)
				return true;
			else
				return false;
		}
		case 2:
		case 4:
		case 7:
		case 9:
		{
			if(distance < spawnThresholds.MinMax_2_4_7_9.y)
				return true;
			else
				return false;
		}
		case 3:
		case 8:
		{
			if(distance < spawnThresholds.MinMax_3_8.y)
				return true;
			else
				return false;
		}
		default:
				return false;
		}
	}

	//Method for checking if we are below minimum
	private bool CheckMin(int angle, float distance)
	{
		switch(angle)
		{
		case 1:
		case 5:
		case 6:
		case 10:
		{
			if(distance > spawnThresholds.MinMax_1_5_6_10.x)
				return true;
			else
				return false;
		}
		case 2:
		case 4:
		case 7:
		case 9:
		{
			if(distance > spawnThresholds.MinMax_2_4_7_9.x)
				return true;
			else
				return false;
		}
		case 3:
		case 8:
		{
			if(distance > spawnThresholds.MinMax_3_8.x)
				return true;
			else
				return false;
		}
		default:
			return false;
		}
	}
	#endregion

	#region Subclasses
	[System.Serializable]
	public class SpawnObjects
	{
		public GameObject SingleTarget;
		public GameObject SequentialTarget;
		public GameObject MultiTarget;
	}

	[System.Serializable]
	public class SpawnThresholds
	{
		public Vector2 MinMax_1_5_6_10 = new Vector2(2.0f, 11.5f);
		public Vector2 MinMax_2_4_7_9 = new Vector2(2.0f, 17.0f);
		public Vector2 MinMax_3_8 = new Vector2(2.0f, 15.0f);
	}
	#endregion
}
