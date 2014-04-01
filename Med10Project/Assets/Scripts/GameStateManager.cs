using UnityEngine;
using System.Collections;

public class GameStateManager : MonoBehaviour 
{
	#region Editor Publics
	[SerializeField] private float spawnTime = 3.0f;
	[SerializeField] private GameObject CenterExplosion;
	//[SerializeField] private int countDownToSpawn = 3;
	#endregion

	#region Privates
	private GestureManager gManager;
	private SpawnManager sManager;
	private SoundManager soundManager;
	private ClockHandler clock;

	private float SpawnBegin;

	private int SpawnCount = 0;
	private bool allowPunching = true;
	private bool playModeActive = false;

	private int currentDistanceToSpawn = 2;


	[HideInInspector] public enum State {awaitCenterClick, awaitTargetSpawn, awaitTargetClick, awaitTargetReturnToCenter};
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

		clock = GameObject.Find("SpawnClock").GetComponent<ClockHandler>();
		clock.SetTime(spawnTime);
	}

	void Start ()
	{
		gManager.OnTapBegan += HandleOnTapBegan;
		gManager.OnTapEnded += TapCenter;
		
		InvokeRepeating("PunchCenter", 0, 1);

		ChangeState(State.awaitCenterClick);
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Play");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Pause");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Unpause");
	}

	void Update()
	{
		switch (state)
		{
		case State.awaitCenterClick:
			break;
		case State.awaitTargetSpawn:
			if(Time.time >= spawnTime + SpawnBegin)
			{
				ChangeState(State.awaitTargetClick);
				sManager.SpawnRandom();
			}
			break;
		case State.awaitTargetClick:
			break;
		case State.awaitTargetReturnToCenter:
			break;
		default:
			break;
		}
	}

	void HandleOnTapBegan (Vector2 screenPos)
	{
		if((state == State.awaitCenterClick) && playModeActive)
		{
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y, 0));
			RaycastHit hitInfo;
			if(Physics.Raycast(ray, out hitInfo))
			{
				if(hitInfo.collider == gameObject.collider)
				{
					soundManager.PlayTouchBegan();
					StartCoroutine(OnClickCenterPunching());
				}
			}
		}
	}

	#region Class Methods
	private void PunchCenter()
	{
		if(!(state == State.awaitTargetClick) && allowPunching && playModeActive)
		{
			ResetGOScale();
			iTween.PunchScale(gameObject, new Vector3(0.2f, 0.2f, 0.2f), 0.5f);
		}
	}

	private void TapCenter(Vector2 screenPos)
	{
		if((state == State.awaitCenterClick) && playModeActive)
		{
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y, 0));
			RaycastHit hitInfo;
			if(Physics.Raycast(ray, out hitInfo))
			{
				if(hitInfo.collider == gameObject.collider)
				{
					//Change the state to currently awaiting a spawn
					ChangeState(State.awaitTargetSpawn);
					//Begin clock rotation
					clock.BeginClock();
					//Play sound to indicate that the touch has ended
					soundManager.PlayTouchEnded();
				}
			}
		}
	}

	public void ChangeState(State plate)
	{
		switch (plate)
		{
		case State.awaitCenterClick:
			state = State.awaitCenterClick;
			iTween.ColorTo(gameObject, iTween.Hash("color", Color.green, "time", 0.2f, "includeChildren", false));
			gameObject.transform.position = new Vector3 (0,0,0);
			break;
		case State.awaitTargetSpawn:
			state = State.awaitTargetSpawn;
			SpawnBegin = Time.time;
			iTween.ColorTo(gameObject, iTween.Hash("color", Color.white, "time", 0.2f, "includeChildren", false));
			break;
		case State.awaitTargetClick:
			state = State.awaitTargetClick;
			iTween.ColorTo(gameObject, iTween.Hash("color", Color.white, "time", 0.4f, "includeChildren", false));
			break;
		case State.awaitTargetReturnToCenter:
			state = State.awaitTargetReturnToCenter;
			StartCoroutine(AwaitTargetToCenter());
			break;
		default:
			break;
		}
	}

	private IEnumerator AwaitTargetToCenter()
	{
		yield return new WaitForSeconds(0.5f);
		iTween.ColorTo(gameObject, iTween.Hash("color", Color.green, "time", 0.2f, "includeChildren", false));
		yield return new WaitForSeconds(0.5f);
		ChangeState(State.awaitCenterClick);
	}

	public IEnumerator SpawnCenterExplosion(Quaternion rotation)
	{
		toggleAllowPunching();
		transform.rotation = rotation;
		yield return new WaitForSeconds(0.5f);
		iTween.PunchPosition(gameObject, Vector3.down*1.01f, 0.5f);
		ResetGOScale();
		iTween.PunchScale(gameObject, new Vector3(0.4f, 1.0f, 0.4f), 0.5f);
		GameObject ExpClone = Instantiate(CenterExplosion, transform.position, transform.rotation) as GameObject;
		Destroy(ExpClone, 1.2f);
		yield return new WaitForSeconds(1.2f);
		toggleAllowPunching();
	}

	private IEnumerator OnClickCenterPunching()
	{
		toggleAllowPunching();
		ResetGOScale();
		iTween.PunchScale(gameObject, new Vector3(-0.5f, -0.5f, -0.5f), 0.5f);
		yield return new WaitForSeconds(0.5f);
		toggleAllowPunching();
	}

	private void toggleAllowPunching()
	{
		allowPunching = !allowPunching;
	}

	private void ResetGOScale()
	{
		iTween.Stop (gameObject, "scale");
		gameObject.transform.localScale = new Vector3 (2,2,2);
	}

	private void NC_Play()
	{
		playModeActive = true;
	}

	private void NC_Pause()
	{
		playModeActive = false;
	}

	private void NC_Unpause()
	{
		playModeActive = true;
	}

	#endregion
}
