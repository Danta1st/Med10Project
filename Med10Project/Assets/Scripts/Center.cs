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

	[HideInInspector] public enum State {green, yellow, red};
	[HideInInspector] public State state;
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

		gManager.OnTap += TapCenter;

		bManager.OnBeat4th1 += PunchCenter;
		bManager.OnBeat4th3 += PunchCenter;

		ChangeState(State.green);
		//bManager.OnBeat4th4 += sManager.SpawnObjectRandom;
	}

	#region Class Methods
	private void PunchCenter()
	{
		audio.Play();
		IncreaseSpawnCount();
		iTween.PunchScale(gameObject, new Vector3(0.2f, 0.2f, 0.2f), 0.5f);
	}

	private void IncreaseSpawnCount()
	{
		if(state == State.yellow)
		{
			if(SpawnCount >= countDownToSpawn)
			{
				ChangeState(State.red);
				sManager.SpawnObjectRandom();
				SpawnCount = 0;
			}
			else
				SpawnCount++;
		}
	}

	private void TapCenter(Vector2 screenPos)
	{
		if(state == State.green)
		{
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y, 0));
			RaycastHit hitInfo;
			if(Physics.Raycast(ray, out hitInfo))
			{
				if(hitInfo.collider == gameObject.collider)
				{
					ChangeState(State.yellow);
				}
			}
		}
	}

	//TODO: Fix proper color mechanism for polish, texture?
	public void ChangeState(State plate)
	{
		switch (plate)
		{
		case State.green:
			state = State.green;
			iTween.ColorTo(gameObject, iTween.Hash("color", Color.green, "time", 0.2f));
			break;
		case State.yellow:
			state = State.yellow;
			iTween.ColorTo(gameObject, iTween.Hash("color", Color.yellow, "time", 0.2f));
			break;
		case State.red:
			state = State.red;
			iTween.ColorTo(gameObject, iTween.Hash("color", Color.red, "time", 0.4f));
			break;
		default:
			break;
		}
	}

	#endregion
}
