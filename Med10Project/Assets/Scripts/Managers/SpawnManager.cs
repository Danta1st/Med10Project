using UnityEngine;
using System.Collections;
//using System.Linq;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour 
{

	#region Editor Publics
	[SerializeField] SpawnObjects spawnObjects;
	[SerializeField] SpawnThresholds spawnThresholds;
	//Distance metrics for Phase1State1
	[SerializeField] private float incrementThreshold = 1.0f;
	[SerializeField] private float incrementValue = 1.0f;
	#endregion

	#region Privates
	private GestureManager gManager;
	private SoundManager sManager;

	private bool isOccupied = false;

	private int objectCounter = 0;
	//Distance handling for Phase1State1
	private List<float> LongestHits = new List<float>();
	private List<float> ShortestFails = new List<float>();
	#endregion
	
	void Awake()
	{
		gManager = Camera.main.GetComponent<GestureManager>();
		if(gManager == null)
			Debug.LogError("No GestureManager was found on the main camera.");

		sManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();

		//Initialise distance lists
		ResetLists();
	}

	void Start()
	{
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Restart");
	}

	#region Public Methods

	//Methods for adjusting distance metrics.
	public void ReportHit(int int1to10, float distance)
	{
		var i = int1to10 - 1;

		//Record new longest hit
		if(distance > LongestHits[i])
			LongestHits[i] = distance;

		//Adjust shortest fails if best hit is longer
		if(LongestHits[i] > ShortestFails[i])
			ShortestFails[i] = LongestHits[i];
	}

	public void ReportMiss(int int1to10, float distance)
	{
		var i = int1to10 - 1;

		//Record new shortest fail
		if(distance < ShortestFails[i])
			ShortestFails[i] = distance;
	}
	//---------------------------------------

	//Methods for spawning
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
		Debug.Log("Target spawned with angleID: "+int1to10+" and distance: "+distance);
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
		//Get random target
		int multiplier = Random.Range(1,10);
		//Adjust the index for lists that begin at 0
		var index = multiplier - 1;

		float distance;

		//Calculate distance. If space between hits and fails are to narrow, 
		//and we have more space to the border of the screen, increment by a factor of 1.0f on fails side
		if(ShortestFails[index] - LongestHits[index] < incrementThreshold)
		{
			if(ShortestFails[index] + incrementValue <= GetAbsMaxDist(multiplier))
				distance = Random.Range(LongestHits[index], ShortestFails[index]) + incrementValue;
			else
			{
				ShortestFails[index] = GetAbsMaxDist(multiplier);
				distance = Random.Range(LongestHits[index], ShortestFails[index]);
			}
		}
		else
		{
			distance = Random.Range(LongestHits[index], ShortestFails[index]);
		}

		//Spawn target
		SpawnSpecific(multiplier, distance);
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

	private void ResetLists()
	{
		LongestHits.Clear();
		ShortestFails.Clear();

		for(int i = 1; i <= 10; i++)
		{
			LongestHits.Add(2.0f);
			ShortestFails.Add(GetAbsMaxDist(i));
		}

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

	//Method for getting max distances
	private float GetAbsMaxDist(int int1to10)
	{
		switch(int1to10)
		{
		case 1:
		case 5:
		case 6:
		case 10:
		{
			return spawnThresholds.MinMax_1_5_6_10.y;
		}
		case 2:
		case 4:
		case 7:
		case 9:
		{
			return spawnThresholds.MinMax_2_4_7_9.y;
		}
		case 3:
		case 8:
		{
			return spawnThresholds.MinMax_3_8.y;
		}
		default:
			return 5;
		}
	}
	#endregion

	#region Notification Methods
	private void NC_Restart()
	{
		ResetLists();
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
		public Vector2 MinMax_1_5_6_10 = new Vector2(2.0f, 10.0f);
		public Vector2 MinMax_2_4_7_9 = new Vector2(2.0f, 16.0f);
		public Vector2 MinMax_3_8 = new Vector2(2.0f, 15.0f);
	}
	#endregion
}
