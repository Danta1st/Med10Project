using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndGameLines : MonoBehaviour {

	private GameObject Grid;
	private GameObject GridWithLabels;
	private GameObject GridBG;
	
	private HighscoreManager hsManager;
	private SpawnManager spManager;

	private GameObject[] NodeArray;
	private GameObject[] ScatterArray;

	private List<float> reactionMeans = new List<float>();

	private float timeBetweenSpawns = 0;
	[SerializeField] private float timeToSpawnAll = 0.5f;

	[SerializeField] private GameObject SpawnNode;
	[SerializeField] private GameObject SpawnHit;
	[SerializeField] private GameObject SpawnMiss;

	// Use this for initialization
	void Start () {
		Grid = GameObject.Find("Grid");
		GridWithLabels = GameObject.Find("GridWithLabels");
		GridBG = GameObject.Find("GridBG");
		hsManager = GameObject.Find("HighscoreManager").GetComponent<HighscoreManager>();
		spManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

		Grid.SetActive(false);
		GridWithLabels.SetActive(false);
		GridBG.SetActive(false);
	}

	private void EnableEndBackground()
	{
		GridBG.SetActive(true);
	}

	public void DisableEndScreen()
	{
		Grid.SetActive(false);
		GridWithLabels.SetActive(false);
		GridBG.SetActive(false);
		DeleteOldNodes();
		DeleteOldScatters();
	}

	public void DoHitMissScreen()
	{
		DeleteOldScatters();
		DeleteOldNodes();
		GridWithLabels.SetActive(false);
		Grid.SetActive(true);
		
		EnableEndBackground();
		
		timeBetweenSpawns = timeToSpawnAll/(hsManager.GetHitCount() + hsManager.GetMissCount());
		Debug.Log(timeBetweenSpawns);
		
		for(int i = 1; i <= 10; i++)
		{
			List<float> angleHits = hsManager.GetHitDistances(i);
			foreach (float hit in angleHits)
			{
				SpawnNodes(i, (hit/spManager.GetAbsMaxDist(i))*5, SpawnHit);
			}
			
			List<float> angleMisses = hsManager.GetMissDistances(i);
			foreach (float miss in angleMisses)
			{
				SpawnNodes(i, (miss/spManager.GetAbsMaxDist(i))*5, SpawnMiss);
			}
		}
		
		StoreScatters();
	}

	public void DoReactionScreen()
	{
		//StopCoroutine("DelayedScatterSpawning");
		StopAllCoroutines();
		StoreScatters();
		DeleteOldNodes();
		DeleteOldScatters();
		GridWithLabels.SetActive(true);
		Grid.SetActive(false);
		EnableEndBackground();

		GetReactions();

		for(int i = 1; i <= reactionMeans.Count; i++)
		{
			int index = i -1;
			if(reactionMeans[index] > 0.1f)
			{
				SpawnNodes(i, (reactionMeans[index]/2.5f)*5, SpawnNode);
			}
			else{
				SpawnNodes(i, 0.1f, SpawnNode);
			}
		}

		StoreNodes();
		DrawLines();
	}

	private void GetReactions()
	{
		reactionMeans = hsManager.GetAllReactionTimes();
	}

	private void SpawnNodes(int int1to10, float distance, GameObject spawnObject)
	{
		int angle = (36 * int1to10) - 18;
		transform.Rotate(transform.forward, (float) -angle);
		Quaternion rotation = transform.rotation;
		Vector3 position = transform.position + gameObject.transform.up * distance;
		transform.Rotate(transform.forward, (float) angle);
		Instantiate(spawnObject, position, rotation);
	}

	private void StoreNodes()
	{
		NodeArray = GameObject.FindGameObjectsWithTag("Node");
	}

	private void StoreScatters()
	{
		ScatterArray = GameObject.FindGameObjectsWithTag("ScatterPlot");
	}

	private void DrawLines()
	{
		for (int i = 0; i < NodeArray.Length; i++) 
		{
			if(i == NodeArray.Length - 1)
				NodeArray[i].GetComponent<nodeBehaviour>().DrawLine(NodeArray[0].transform.position);
			else
				NodeArray[i].GetComponent<nodeBehaviour>().DrawLine(NodeArray[i+1].transform.position);
		}
	}

	private void DeleteOldNodes()
	{
		if(NodeArray != null)
		{
			foreach (GameObject node in NodeArray)
			{
				Destroy(node);
			}
		}
	}

	private void DeleteOldScatters()
	{
		if(ScatterArray != null)
		{
			foreach (GameObject scatter in ScatterArray)
			{
				Destroy(scatter);
			}
		}
	}
}
