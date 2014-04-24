using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EndGameLines : MonoBehaviour {

	[SerializeField] private bool useAdaptiveRtFigure = false;
	[SerializeField] private GameObject Labels;
	[SerializeField] private SpawnObjects spawnObjects;

	private GameObject Grid;
	private GameObject GridWithLabels;
	private GameObject GridBG;
	
	private HighscoreManager hsManager;
	private SpawnManager spManager;

	private GameObject[] NodeArray;
	private GameObject[] ScatterArray;
	private List<GameObject> LabelListGrid = new List<GameObject>();
	private List<GameObject> LabelListAngles = new List<GameObject>();

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

	private void SpawnGridLabels(float incrementValue)
	{
		ClearLabelList(LabelListGrid);

		for(int i = 1; i <= 5; i++)
		{
			GameObject go = SpawnNodeReturn(2, 1 * i, spawnObjects.NodeLabel);
			//Set label text
			go.GetComponent<NodeLabel>().SetText("" + incrementValue * i + "s");
			go.transform.name += ""+i;
			//Set parent to keep hierachy clean
			go.transform.parent = Labels.transform;
			//Stuff in array for cleaning later
			LabelListGrid.Add(go);		
		}
	}

	private void SpawnAngleLabels()
	{
		ClearLabelList(LabelListAngles);
		
		for(int i = 1; i <= 10; i++)
		{
			//Spawn angleID labels
			GameObject go = SpawnNodeReturn(i, 5.5f, spawnObjects.AngleLabel);
			//Set label text
			go.GetComponent<NodeLabel>().SetText("" + (36 * i - 18));
			go.transform.name = "AngleLabel"+i;
			//Set parent to keep hierachy clean
			go.transform.parent = Labels.transform;
			//Stuff in array for cleaning later
			LabelListAngles.Add(go);
		}
	}

	private void ClearLabelList(List<GameObject> list)
	{
		foreach(GameObject go in list)
			Destroy(go);

		list.Clear();
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
		ClearLabelList(LabelListGrid);
		
		EnableEndBackground();
		Grid.SetActive(true);

		if(LabelListAngles.Count == 0)
			SpawnAngleLabels();

		
		timeBetweenSpawns = timeToSpawnAll/(hsManager.GetHitCount() + hsManager.GetMissCount());
		Debug.Log(timeBetweenSpawns);
		
		for(int i = 1; i <= 10; i++)
		{
			List<float> angleHits = hsManager.GetHitDistances(i);
			foreach (float hit in angleHits)
			{
				SpawnNode(i, (hit/spManager.GetAbsMaxDist(i))*5, spawnObjects.SpawnHit);
			}
			
			List<float> angleMisses = hsManager.GetMissDistances(i);
			foreach (float miss in angleMisses)
			{
				SpawnNode(i, (miss/spManager.GetAbsMaxDist(i))*5, spawnObjects.SpawnMiss);
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
		
		EnableEndBackground();
		Grid.SetActive(true);

		if(LabelListAngles.Count == 0)
			SpawnAngleLabels();
		
		reactionMeans = hsManager.GetAllReactionTimes();

		float incrementValue = 0.0f;

		//Find the highest value reaction time
		foreach(float value in reactionMeans)
		{
			if(value != null && value > incrementValue)
				incrementValue = value;
		}
			
		//What is the grid values? 
		if(useAdaptiveRtFigure == true)
		{
			incrementValue = incrementValue/5.0f;
		}
		else if(incrementValue > 2.5f)
		{
			incrementValue = 1.0f;
		}
		else
			incrementValue = 0.5f;

		//Spawn grid labels
		SpawnGridLabels(incrementValue);

		//Spawn line nodes
		for(int i = 1; i <= reactionMeans.Count; i++)
		{

			int index = i -1;
			if(reactionMeans[index] > 0.1f)
			{
				SpawnNode(i, (reactionMeans[index]/(incrementValue*5.0f))*5.0f, spawnObjects.SpawnNode);
			}
			else{
				SpawnNode(i, 0.1f, spawnObjects.SpawnNode);
			}
		}

		StoreNodes();
		DrawLines();
	}

	private void SpawnNode(int int1to10, float distance, GameObject spawnObject)
	{
		int angle = (36 * int1to10) - 18;
		transform.Rotate(transform.forward, (float) -angle);
		Quaternion rotation = transform.rotation;
		Vector3 position = transform.position + gameObject.transform.up * distance;
		transform.Rotate(transform.forward, (float) angle);
		Instantiate(spawnObject, position, rotation);
	}

	private GameObject SpawnNodeReturn(int int1to10, float distance, GameObject spawnObject)
	{
		int angle = (36 * int1to10) - 18;
		transform.Rotate(transform.forward, (float) -angle);
		Quaternion rotation = transform.rotation;
		Vector3 position = transform.position + gameObject.transform.up * distance;
		transform.Rotate(transform.forward, (float) angle);
		GameObject go = (GameObject) Instantiate(spawnObject, position, rotation);
		return go;
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
	
	#region Subclasses
	[System.Serializable]
	public class SpawnObjects
	{
		public GameObject SpawnNode;
		public GameObject SpawnHit;
		public GameObject SpawnMiss;
		public GameObject NodeLabel;
		public GameObject AngleLabel;

	}
	#endregion
	
}
