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
	private SoundManager soundManager;

	private int SpawnCount = 0;

	[SerializeField]
	private GameObject CenterExplosion;

	[HideInInspector] public enum State {awaitCenterClick, awaitTargetSpawn, awaitTargetClick};
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

		soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
		if(soundManager == null)
			Debug.LogError("No SoundManager was found in the scene.");
	}

	void Start ()
	{
		//TODO: Change to "start game" or something similar
		//gManager.OnSwipeUp += bManager.ToggleBeats;

		gManager.OnTapBegan += HandleOnTapBegan;
		gManager.OnTapEnded += TapCenter;

		bManager.OnBeat4th1 += PunchCenter;
		bManager.OnBeat4th3 += PunchCenter;

		ChangeState(State.awaitCenterClick);
		//bManager.OnBeat4th4 += sManager.SpawnObjectRandom;
	}

	void HandleOnTapBegan (Vector2 screenPos)
	{
		if(state == State.awaitCenterClick)
		{
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y, 0));
			RaycastHit hitInfo;
			if(Physics.Raycast(ray, out hitInfo))
			{
				if(hitInfo.collider == gameObject.collider)
				{
					soundManager.PlayTouchBegan();
				}
			}
		}
	}

	#region Class Methods
	private void PunchCenter()
	{
		if(!(state == State.awaitTargetClick))
		{
			IncreaseSpawnCount();
			iTween.PunchScale(gameObject, new Vector3(0.2f, 0.2f, 0.2f), 0.5f);
		}
	}

	private void IncreaseSpawnCount()
	{
		if(state == State.awaitTargetSpawn)
		{
			if(SpawnCount >= countDownToSpawn)
			{
				ChangeState(State.awaitTargetClick);
				sManager.SpawnObjectRandom();
				SpawnCount = 0;
			}
			else
				SpawnCount++;
		}
	}

	private void TapCenter(Vector2 screenPos)
	{
		if(state == State.awaitCenterClick)
		{
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y, 0));
			RaycastHit hitInfo;
			if(Physics.Raycast(ray, out hitInfo))
			{
				if(hitInfo.collider == gameObject.collider)
				{
					ChangeState(State.awaitTargetSpawn);
					soundManager.PlayTouchEnded();
				}
			}
		}
	}

	//TODO: Fix proper color mechanism for polish, texture?
	public void ChangeState(State plate)
	{
		switch (plate)
		{
		case State.awaitCenterClick:
			state = State.awaitCenterClick;
			iTween.ColorTo(gameObject, iTween.Hash("color", Color.green, "time", 0.2f));
			break;
		case State.awaitTargetSpawn:
			state = State.awaitTargetSpawn;
			iTween.ColorTo(gameObject, iTween.Hash("color", Color.white, "time", 0.2f));
			break;
		case State.awaitTargetClick:
			state = State.awaitTargetClick;
			iTween.ColorTo(gameObject, iTween.Hash("color", Color.white, "time", 0.4f));
			break;
		default:
			break;
		}
	}

	public IEnumerator SpawnCenterExplosion(float time)
	{
		yield return new WaitForSeconds(time);
		GameObject ExpClone = Instantiate(CenterExplosion, transform.position, transform.rotation) as GameObject;
		Destroy(ExpClone, 1.0f);
	}

	#endregion
}
