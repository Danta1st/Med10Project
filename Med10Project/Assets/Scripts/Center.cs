﻿using UnityEngine;
using System.Collections;

public class Center : MonoBehaviour 
{

	#region Privates
	private BpmManager bManager;
	private GestureManager gManager;
	private SpawnManager sManager;
	#endregion

	void Awake()
	{
		bManager = GameObject.Find("BpmManager").GetComponent<BpmManager>();
		if(bManager == null)
			Debug.LogError("No BpmManager was found in the scene.");
		
		gManager = Camera.main.GetComponent<GestureManager>();
		if(gManager == null)
			Debug.LogError("No GestureManager was found on the main camera.");

		sManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
		if(sManager == null)
			Debug.LogError("No SpawnManager was found in the scene.");
	}

	void Start ()
	{
		gManager.OnSwipeUp += bManager.ToggleBeats;
		bManager.OnBeat4th1 += PunchCenter;
		bManager.OnBeat4th3 += PunchCenter;
		bManager.OnBeat4th4 += sManager.SpawnObjectRandom;
	}

	#region Class Methods
	private void PunchCenter()
	{
		audio.Play();
		iTween.PunchScale(gameObject, new Vector3(0.2f, 0.2f, 0.2f), 0.5f);
	}


	#endregion
}
