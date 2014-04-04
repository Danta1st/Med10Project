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
	private GameStateManager gStateManager;
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
		gStateManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();

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
	public void SpawnSpecific(GameObject spawnObject, int int1to10)
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

	public void SpawnSpecific(GameObject spawnObject, int int1to10, float distance)
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

	public void SpawnRandom(GameObject spawnObject)
	{
		int multiplier = Random.Range(1,11);
		float distance = Random.Range(2.0f, 8.5f);

		SpawnSpecific(spawnObject, multiplier, distance);
	}

	public void Phase1Stage1(int int1to10)
	{
		//Adjust the index for lists that begin at 0
		var index = int1to10 - 1;

		float distance;

		//Calculate distance. If space between hits and fails are to narrow, 
		//and we have more space to the border of the screen, increment by a factor of 1.0f on fails side
		if(ShortestFails[index] - LongestHits[index] < incrementThreshold)
		{
			if(ShortestFails[index] + incrementValue <= GetAbsMaxDist(int1to10))
				distance = Random.Range(LongestHits[index], ShortestFails[index]) + incrementValue;
			else
			{
				ShortestFails[index] = GetAbsMaxDist(int1to10);
				distance = Random.Range(LongestHits[index], ShortestFails[index]);
			}
		}
		else
		{
			distance = Random.Range(LongestHits[index], ShortestFails[index]);
		}

		//Check if distance is still below incremenent threshold
		if(GetAbsMaxDist(int1to10) - LongestHits[index] < incrementThreshold)
		{
			//Flag for state 2
			gStateManager.SetAngleState(int1to10, 0);

			//Spawn sequential target
			SpawnSpecific(spawnObjects.SequentialTarget, int1to10, distance);
			//SpawnSpecific(spawnObjects.SingleTarget, int1to10, distance);
		}
		else
		{
			//Spawn target normally
			SpawnSpecific(spawnObjects.SingleTarget, int1to10, distance);
		}
	}

	public void Phase1Stage2(int int1to10)
	{
		int index;
		//Calculate opposite index
		if(int1to10 + 5 > 10)
			index = int1to10 + 5 - 10;
		else
			index = int1to10 + 5;

		//Calculate distance based on opposite progress
		float distance = GetAbsMaxDist(int1to10) - LongestHits[index - 1];

		if(distance < 2.0f)
		{
			//Spawn stage 2 item with minimum distance
			SpawnSpecific(spawnObjects.SingleTarget, int1to10, 2.0f);
			//Flag this angle for multiple Targets
			gStateManager.SetAngleState(int1to10, 1); //Goes from Sequential to Multitarget
		}
		else
		{
			//Spawn stage 2 item
			SpawnSpecific(spawnObjects.SingleTarget, int1to10, distance);
		}
	}

	public void Phase1Stage3(int int1to10)
	{
		//TODO: Implement Phase1Stage3
		float dist = Random.Range(2.0f, GetAbsMaxDist(1));

		for(int i = -1; i <= 1; i++)
		{
			int j;

			//Catch out of bound errors
			if(int1to10 + i > 10)
				j = int1to10 + i - 10;
			else if(int1to10 + i < 1)
				j = int1to10 + i + 10;
			else
				j = int1to10 + i;

			//Spawn Objects
			SpawnSpecific(spawnObjects.MultiTarget, j, dist);
		}
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
