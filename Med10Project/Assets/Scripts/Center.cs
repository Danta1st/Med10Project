using UnityEngine;
using System.Collections;

public class Center : MonoBehaviour 
{
	#region Editor Publics
	[SerializeField] private int countDownToSpawn = 3;
	#endregion

	#region Privates
	private BpmManager bManager;
	private GestureManager gManager;
	private SpawnManager sManager;

	private int SpawnCount = 0;

	private enum colors {green, yellow, red};
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
		//TODO: Change to "start game" or something similar
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

	private void IncreaseSpawnCount()
	{

	}

	//TODO: Fix proper color mechanism for polish, texture?
	private void ChangeState(colors color)
	{
		switch (color)
		{
		case colors.green:
			gameObject.renderer.material.color = Color.green;
			break;
		case colors.yellow:
			gameObject.renderer.material.color = Color.yellow;
			break;
		case colors.red:
			gameObject.renderer.material.color = Color.red;
			break;
		default:
			break;
		}
	}

	#endregion
}
