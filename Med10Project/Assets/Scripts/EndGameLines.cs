using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndGameLines : MonoBehaviour {

	private LineRenderer lineRenderer;
	private int lengthOfLineRenderer;

	private GameObject Grid;
	private GameObject GridWithLabels;
	private GameObject GridBG;
	
	private HighscoreManager hsManager;
	private SpawnManager spManager;

	private GameObject[] NodeArray;
	private GameObject[] ScatterArray;

	private List<float> reactionMeans = new List<float>();

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
		lineRenderer = GetComponentInChildren<LineRenderer>();

		Grid.SetActive(false);
		GridWithLabels.SetActive(false);
		GridBG.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.S))
		{
			DoReactionScreen();
		}

		if(Input.GetKeyDown(KeyCode.F))
		{
			DisableEndScreen();
		}
	
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
		lineRenderer.SetVertexCount(0);
		DeleteOldNodes();
		DeleteOldScatters();
	}

	/*public void DoHitMissScreen()
	{
		GridWithLabels.SetActive(false);
		Grid.SetActive(true);

		EnableEndBackground();

		float hits = 0;
		float misses = 0;
		float total = 0;
		
		for(int i = 1; i <= 10; i++)
		{
			if(hsManager.GetHitCount(i) > 0 || hsManager.GetMissCount(i) > 0)
			{
				hits = hsManager.GetHitCount(i);;
				misses = hsManager.GetMissCount(i);;
				total = hits+misses;

				SpawnNodes(i, ((hits/total)*5));
			}
			else
			{
				SpawnNodes(i, 5f);
			}
		}
		
		StoreNodes();
		DrawLines();
	}*/

	public void DoHitMissScreen()
	{
		DeleteOldScatters();
		DeleteOldNodes();
		GridWithLabels.SetActive(false);
		Grid.SetActive(true);
		
		EnableEndBackground();
		
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

		
//		lineRenderer.SetVertexCount(Nodes.Length+1);
//
//		for (int i = 0; i < Nodes.Length; i++) {
//			Vector3 pos = new Vector3(Nodes[i].transform.position.x, Nodes[i].transform.position.y, 1);
//			lineRenderer.SetPosition(i, pos);
//		}
//
//		Vector3 endPos = new Vector3(Nodes[0].transform.position.x, Nodes[0].transform.position.y, 1);
//		lineRenderer.SetPosition(10, endPos);
//
//		DeleteOldNodes();
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
