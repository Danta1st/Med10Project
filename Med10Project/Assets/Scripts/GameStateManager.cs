using UnityEngine;
using System.Collections;

public class GameStateManager : MonoBehaviour 
{
	#region Editor Publics
	[SerializeField] private int countDownToSpawn = 3;
	#endregion

	#region Privates
	private GestureManager gManager;
	private SpawnManager sManager;
	private SoundManager soundManager;

	private int SpawnCount = 0;
	private bool isGettingHit = false;

	[SerializeField]
	private GameObject CenterExplosion;

	[HideInInspector] public enum State {awaitCenterClick, awaitTargetSpawn, awaitTargetClick};
	[HideInInspector] public State state;
	#endregion

	void Awake()
	{
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

		gManager.OnTapBegan += HandleOnTapBegan;
		gManager.OnTapEnded += TapCenter;
		
		InvokeRepeating("PunchCenter", 0, 1);

		ChangeState(State.awaitCenterClick);
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
		if(!(state == State.awaitTargetClick) && !(isGettingHit))
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
			StartCoroutine(DelayedColorChange());
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

	private IEnumerator DelayedColorChange()
	{
		yield return new WaitForSeconds(0.5f);
		iTween.ColorTo(gameObject, iTween.Hash("color", Color.green, "time", 0.2f));
	}

	public IEnumerator SpawnCenterExplosion(Quaternion rotation)
	{
		isGettingHit = true;
		transform.rotation = rotation;
		yield return new WaitForSeconds(0.5f);
		iTween.PunchPosition(gameObject, Vector3.down*1.01f, 1.1f);
		iTween.PunchScale(gameObject, new Vector3(0.4f, 1.1f, 0.4f), 0.5f);
		GameObject ExpClone = Instantiate(CenterExplosion, transform.position, transform.rotation) as GameObject;
		Destroy(ExpClone, 1.2f);
		yield return new WaitForSeconds(1.2f);
		isGettingHit = false;
	}

	#endregion
}
