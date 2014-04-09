using UnityEngine;
using System.Collections;
using System;

public class Phase2Behavior : MonoBehaviour {

	private GameStateManager gameManager;
	private SpawnManager spawnManager;
	private GUIManager guiManager;
	private SoundManager sManager;

	[SerializeField] private GameObject SpawnObject;
	private GameObject[] Targets;

	private int currentAmountOfActiveTargets = 0;
	private int currentAmountOfHits = 0;
	private int currentAmountOfMisses = 0;
	private int startDistance = 10;
	private int currentDistance;
	private bool missRecieved = false;

	private enum Stage {Right, Left, Both};
	private Stage stage = Stage.Right;

	private System.Random _random = new System.Random();

	// Use this for initialization

	void Awake()
	{
		gameManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
		spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
		guiManager =  GameObject.Find("3DGUICamera").GetComponent<GUIManager>();
		sManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
	}

	void Start () {
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Restart");
		SpawnXTargets(10, 0);
		StoreTargets();
		StartCoroutine("StartStage");
		currentDistance = startDistance;

		//Determine curtain blocking
		if(stage == Stage.Right)
		{
			guiManager.BlockRightHalf(false);
			guiManager.BlockLeftHalf(true);
		}
		else if(stage == Stage.Left)
		{
			guiManager.BlockRightHalf(true);
			guiManager.BlockLeftHalf(false);
		}
		else if(stage == Stage.Both)
			guiManager.BlockAll(false);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private void StoreTargets()
	{
		Targets = GameObject.FindGameObjectsWithTag("Phase2Object");
	}

	public IEnumerator StartStage()
	{
		yield return new WaitForSeconds(1.0f);
		ChangeDistance(0);
		ResetActiveTargets();
	}

	public void ResetActiveTargets()
	{
		currentAmountOfActiveTargets = UnityEngine.Random.Range(2,4); //Random range on int is exclusive max

		SetTargetsActive(currentAmountOfActiveTargets);

		currentAmountOfHits = 0;

	}

	private void SetTargetsActive(int amount)
	{
		sManager.PlayNewTargetSpawned();

		int[] randomTargets = new int[5];

		if(stage == Stage.Right)
		{
			randomTargets = new int[5]{5,6,7,8,9};
		}
		else if(stage == Stage.Left)
		{
			randomTargets = new int[5]{0,1,2,3,4};
		}
		else if(stage == Stage.Both)
		{
			randomTargets = new int[10];
			for (int x = 0; x < randomTargets.Length; x++) 
			{
				randomTargets[x] = x;
			}
		}

		Shuffle(randomTargets);
		
		for (int i = 0; i < amount; i++)
		{
			Targets[randomTargets[i]].GetComponent<Phase2Object>().SetActiveTarget();

			if(stage == Stage.Right)
			{
				Targets[randomTargets[i]].GetComponent<Phase2Object>().SetObjectType(Phase2Object.ObjectTypes.P2_Right);
			}
			else if(stage == Stage.Left)
			{
				Targets[randomTargets[i]].GetComponent<Phase2Object>().SetObjectType(Phase2Object.ObjectTypes.P2_Left);
			}
			else if(stage == Stage.Both)
			{
				Targets[randomTargets[i]].GetComponent<Phase2Object>().SetObjectType(Phase2Object.ObjectTypes.P2_Both);
			}
		}

	}

	public void Shuffle<T>(T[] array)
	{
		var random = _random;
		for (int i = array.Length; i > 1; i--)
		{
			// Pick random element to swap.
			int j = random.Next(i); // 0 <= j <= i-1
			// Swap.
			T tmp = array[j];
			array[j] = array[i - 1];
			array[i - 1] = tmp;
		}
	}


	public void SendHit(){
		currentAmountOfHits++;
		if(currentAmountOfActiveTargets == currentAmountOfHits)
		{
			gameManager.ChangeCenterState(GameStateManager.State.awaitTargetReturnToCenter);
			ChangeDistance(10);
		}
	}

	public void SendMiss()
	{
		StartCoroutine(WaitForAllMisses());
	}

	private IEnumerator WaitForAllMisses()
	{
		if (!missRecieved){
			missRecieved = true;
			currentAmountOfHits = 0;
			gameManager.ChangeCenterState(GameStateManager.State.awaitCenterClick);
			ChangeDistance(-10);
		}
		yield return new WaitForSeconds(0.5f);
		missRecieved = false;
	}

	private void ChangeDistance(int distance)
	{
		currentDistance += distance;

		if(currentDistance < 20)
		{
			currentDistance = 20;
		}
		if(currentDistance > 100)
		{
			currentDistance = 20;
   			StartCoroutine(ChangeStage());
		}

		foreach (GameObject target in Targets)
		{
			float maxDistanceForThisAngle = (spawnManager.GetAbsMaxDist(target.GetComponent<Phase2Object>().GetMultiplier()));
			float maxDistanceForShortestAngle = spawnManager.GetAbsMaxDist(1);
			float adjustedDistance = (((currentDistance/100.0f)*maxDistanceForShortestAngle)/2)
									+(((currentDistance/100.0f)*maxDistanceForThisAngle)/2);
			iTween.MoveTo(target, iTween.Hash("position", -target.transform.up*adjustedDistance, "time", 0.5f, "easetype", iTween.EaseType.easeInBack));
			target.GetComponent<Phase2Object>().SetDistance(adjustedDistance);
		}
	}

	private IEnumerator ChangeStage()
	{
		if(stage == Stage.Right){
			stage = Stage.Left;
			yield return new WaitForSeconds(1.0f);
			guiManager.BlockRightHalf(true);
			yield return new WaitForSeconds(1.0f);
			guiManager.BlockLeftHalf(false);
		}
		else if(stage == Stage.Left){
			stage = Stage.Both;
			yield return new WaitForSeconds(1.0f);
			guiManager.BlockAll(false);
		}
		else if(stage == Stage.Both){
			stage = Stage.Both;
		}
	}

	public void SetStageRight()
	{
		stage = Stage.Right;
		guiManager.BlockLeftHalf(true);
		guiManager.BlockRightHalf(false);
	}

	public void SetStageLeft()
	{
		stage = Stage.Left;
		guiManager.BlockLeftHalf(false);
		guiManager.BlockRightHalf(true);
	}

	public void SetStageBoth()
	{
		stage = Stage.Both;
		guiManager.BlockAll(false);
	}

	private void SpawnSpecific(int int1to10, float distance)
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
		GameObject go = (GameObject) Instantiate(SpawnObject, position, rotation);
		go.transform.parent = transform;
		//Set Object Parameters
		go.GetComponent<Phase2Object>().SetAngle((int) angle);
		//go.GetComponent<Phase2Object>().SetDistance(distance);
		//go.GetComponent<Phase2Object>().SetSpawnTime(Time.time);
		go.GetComponent<Phase2Object>().SetMultiplier(int1to10);
		go.GetComponent<Phase2Object>().SetTargetDisabled();
		go.name = go.name+int1to10;
		//Set occupied
	}
	
	private void SpawnXTargets(int amountOfTargets, float distance)
	{
		for(int i = 1; i <= amountOfTargets; i++)
		{
			SpawnSpecific(i, distance);
		}
	}

	private int GetAngle(int int1to10)
	{
		int angle = (36 * int1to10) - 18;
		return angle;
	}
	
	private void RotateSelf(int angle)
	{
		transform.Rotate(0, 0, (float) -angle);
	}

	private void NC_Restart()
	{
		Destroy(gameObject, 0.5f);
	}

}