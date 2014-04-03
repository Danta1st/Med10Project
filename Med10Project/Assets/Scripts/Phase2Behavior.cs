using UnityEngine;
using System.Collections;
using System;

public class Phase2Behavior : MonoBehaviour {

	private GameStateManager gameManager;
	[SerializeField] private GameObject SpawnObject;
	private GameObject[] Targets;

	private int currentAmountOfActiveTargets = 0;
	private int currentAmountOfHits = 0;
	private int currentAmountOfMisses = 0;
	private int startDistance = 2;
	private int currentDistance;
	private bool missRecieved = false;

	private System.Random _random = new System.Random();

	// Use this for initialization

	void Awake()
	{
		gameManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
	}

	void Start () {
		SpawnXTargets(10, 0);
		StoreTargets();
		ResetActiveTargets();
		currentDistance = startDistance;
		ChangeDistance(0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ResetActiveTargets()
	{
		//currentAmountOfActiveTargets = Random.Range(1,5);
		currentAmountOfActiveTargets = 3;
		StartCoroutine(SetTargetsActive(currentAmountOfActiveTargets));
		currentAmountOfHits = 0;
	}

	private IEnumerator SetTargetsActive(int amount)
	{
		yield return new WaitForSeconds(0.5f);
		int[] randomTargets = new int[10];

		for (int x = 0; x < randomTargets.Length; x++) 
		{
			randomTargets[x] = x;
		}

		Shuffle(randomTargets);

		for (int i = 0; i < amount; i++)
		{
			Targets[randomTargets[i]].GetComponent<Phase2Object>().SetActiveTarget();
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

	private void StoreTargets()
	{
		Targets = GameObject.FindGameObjectsWithTag("Phase2Object");
	}

	public void SendHit(){
		currentAmountOfHits++;
		if(currentAmountOfActiveTargets == currentAmountOfHits)
		{
			gameManager.ChangeState(GameStateManager.State.awaitTargetReturnToCenter);
			ChangeDistance(1);
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
			gameManager.ChangeState(GameStateManager.State.awaitCenterClick);
			ChangeDistance(-1);
		}
		yield return new WaitForSeconds(0.5f);
		missRecieved = false;
	}

	private void ChangeDistance(int distance)
	{
		currentDistance += distance;

		Debug.Log(currentDistance);

		foreach (GameObject target in Targets)
		{
			iTween.MoveTo(target, iTween.Hash("position", -target.transform.up*currentDistance, "time", 0.5f, "easetype", iTween.EaseType.linear));
		}
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
		go.GetComponent<Phase2Object>().SetDistance(distance);
		go.GetComponent<Phase2Object>().SetSpawnTime(Time.time);
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

}
