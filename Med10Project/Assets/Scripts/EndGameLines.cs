﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndGameLines : MonoBehaviour {

	private LineRenderer lineRenderer;
	private int lengthOfLineRenderer;

	private GameObject Grid;
	private GameObject GridBG;


	private HighscoreManager hsManager;

	private GameObject[] Nodes;

	private List<float> reactionMeans = new List<float>();

	[SerializeField] private GameObject SpawnNode;

	// Use this for initialization
	void Start () {
		Grid = GameObject.Find("Grid");
		GridBG = GameObject.Find("GridBG");
		hsManager = GameObject.Find("HighscoreManager").GetComponent<HighscoreManager>();
		lineRenderer = GetComponentInChildren<LineRenderer>();

		Grid.SetActive(false);
		GridBG.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.S))
		{
			EnableEndScreen();
			//DoHitMissScreen();
			DoReactionScreen();
		}
	
	}

	private void EnableEndScreen()
	{
		Grid.SetActive(true);
		GridBG.SetActive(true);
	}

	private void DoHitMissScreen()
	{
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
				SpawnNodes(i, 0.1f);
			}
		}
		
		StoreNodes();
		DrawLines();
	}

	private void DoReactionScreen()
	{
		GetReactions();

		for(int i = 0; i < reactionMeans.Count; i++)
		{
			if(reactionMeans[i] > 0.1f)
			{
				SpawnNodes(i, (reactionMeans[i]/2.5f)*5);
			}
			else{
				SpawnNodes(i, 0.1f);
			}
		}

		StoreNodes();
		DrawLines();
	}

	private void GetReactions()
	{
		reactionMeans = hsManager.GetAllReactionTimes();
	}

	private void SpawnNodes(int int1to10, float distance)
	{
		int angle = (36 * int1to10) - 18;
		transform.Rotate(transform.forward, (float) -angle);
		Quaternion rotation = transform.rotation;
		Vector3 position = transform.position + gameObject.transform.up * distance;
		transform.Rotate(transform.forward, (float) angle);
		Instantiate(SpawnNode, position, rotation);
	}

	private void StoreNodes()
	{
		Nodes = GameObject.FindGameObjectsWithTag("Node");
	}

	private void DrawLines()
	{
		lineRenderer.SetVertexCount(Nodes.Length+1);

		for (int i = 0; i < Nodes.Length; i++) {
			Vector3 pos = new Vector3(Nodes[i].transform.position.x, Nodes[i].transform.position.y, 1);
			lineRenderer.SetPosition(i, pos);
		}

		Vector3 endPos = new Vector3(Nodes[0].transform.position.x, Nodes[0].transform.position.y, 1);
		lineRenderer.SetPosition(10, endPos);
	}
}
