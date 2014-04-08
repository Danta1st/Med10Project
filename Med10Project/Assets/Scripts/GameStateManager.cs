using UnityEngine;
using System.Collections;

public class GameStateManager : MonoBehaviour 
{
	#region Publics
	#endregion

	#region Editor Publics
	[SerializeField] private float spawnTime = 3.0f;
	[SerializeField] private GameObject CenterExplosion;
	#endregion

	#region Privates
	//Script connectivity
	private GestureManager gManager;
	private SpawnManager sManager;
	private SoundManager soundManager;
	private ClockHandler clock;

	//Time logs
	private float SpawnBegin;

	//Booleans
	private bool allowPunching = true;
	private bool playModeActive = false;

	//States
	[HideInInspector] public enum State {awaitCenterClick, awaitTargetSpawn, awaitTargetClick, awaitTargetReturnToCenter};
	[HideInInspector] public State state;
	private Phase1States phase1States = new Phase1States();
	public enum Phases {Phase1, Phase2, Phase3};
	private Phases phase;

	//Counters
	private int multiTargetCounter = 0;
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
		//Begin at Phase 1
		phase = Phases.Phase1;
		//Subscribe to Tap Gesture
		gManager.OnTapBegan += HandleOnTapBegan;
		//Begin Punching
		InvokeRepeating("PunchCenter", 0, 1);

		ChangeCenterState(State.awaitCenterClick);
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Play");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Pause");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Unpause");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Restart");
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
				ChangeCenterState(State.awaitTargetClick);

				//Spawn according to state TODO: Move to own method taking int1to10 as input
				if(phase == Phases.Phase1){
					//Get random target
					int int1to10 = Random.Range(1, 11); //Remember, max is exlusive for ints
					Debug.Log("Rolled: "+int1to10);

					if(phase1States.GetAngleState(int1to10) == Phase1States.States.SingleTarget ||
					   phase1States.GetAngleState(int1to10) == Phase1States.States.SequentialTarget)
						sManager.Phase1Stage1(int1to10);
					else if(phase1States.GetAngleState(int1to10) == Phase1States.States.MultipleTargets)
						sManager.Phase1Stage3(int1to10);
				}
				else if(phase == Phases.Phase2)
				{
					sManager.Phase2Stage1();
				}
			}
			break;
		case State.awaitTargetClick:
			if(multiTargetCounter >= 3)
			{
				multiTargetCounter = 0;
				ChangeCenterState(State.awaitTargetReturnToCenter);
			}
			break;
		case State.awaitTargetReturnToCenter:
			break;
		default:
			break;
		}
	}

	void DecreaseSpawnTime()
	{
		//TODO: This should rather be based on a player's reaction time than just played time.
		if(spawnTime > 0.5f)
		{
			spawnTime -= 0.1f;
			clock.SetTime(spawnTime);
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
					//Change the state to currently awaiting a spawn
					ChangeCenterState(State.awaitTargetSpawn);
					//Begin clock rotation
//					clock.BeginClock();

					soundManager.PlayTouchBegan();
					StartCoroutine(OnClickCenterPunching());
				}
			}
		}
	}

	#region Public Methods
	public void IncreaseMultiTargetCounter()
	{
		multiTargetCounter++;
	}

	public void SetAngleState(int int1to10, int state)
	{
		if(state == 0 && phase1States.GetAngleState(int1to10) == Phase1States.States.SingleTarget)
			phase1States.SetAngleState(int1to10, Phase1States.States.SequentialTarget);
		else if(state == 1 && phase1States.GetAngleState(int1to10) == Phase1States.States.SequentialTarget)
			phase1States.SetAngleState(int1to10, Phase1States.States.MultipleTargets);
		else if(state == 2 && phase1States.GetAngleState(int1to10) == Phase1States.States.MultipleTargets)
			phase = Phases.Phase2;
	}

	public int GetAngleState(int int1to10)
	{
		return (int) phase1States.GetAngleState(int1to10);
	}

	//Public Method for changing the center state.
	public void ChangeCenterState(State plate)
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
	#endregion

	#region Class Methods
	private void PunchCenter()
	{
		if(!(state == State.awaitTargetClick) && allowPunching && playModeActive)
		{
			ResetGOScale();
			iTween.PunchScale(gameObject, new Vector3(0.2f, 0.2f, 0.2f), 0.5f);
		}
	}

	private IEnumerator AwaitTargetToCenter()
	{
//		Debug.Log("AwaitingTargetToCenter");
		yield return new WaitForSeconds(0.5f);
		iTween.ColorTo(gameObject, iTween.Hash("color", Color.green, "time", 0.2f, "includeChildren", false));
		yield return new WaitForSeconds(0.3f);
		ChangeCenterState(State.awaitCenterClick);
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

	private void NC_Restart()
	{
		phase1States.ResetStates();
		phase = Phases.Phase1;

		ChangeCenterState(State.awaitCenterClick);
	}	

	public void CheckPhase1Ended()
	{
		for(int i = 1; i <= 10; i++)
		{
			if(GetAngleState(i) == 0 || GetAngleState(i) == 1)
			{
				return;
			}
		}
		phase = Phases.Phase2;
	}
	#endregion

	#region Subclasses
	[System.Serializable]
	public class Phase1States
	{
		public enum States {SingleTarget, SequentialTarget, MultipleTargets};

		private States[] stateArray = new States[10] {States.SingleTarget, States.SingleTarget, States.SingleTarget, 
													 States.SingleTarget, States.SingleTarget, States.SingleTarget, 
													 States.SingleTarget, States.SingleTarget, States.SingleTarget, 
													 States.SingleTarget};

		public void SetAngleState(int int1to10, States state)
		{
			var i = int1to10-1;
//			Debug.Log("Setting state: "+state+" with index: "+i);
			stateArray[i] = state;
		}
		public States GetAngleState(int int1to10)
		{
			var i = int1to10-1;
//			Debug.Log("Returning state: "+stateArray[i]+" with index: "+i);
			return stateArray[i];
		}

		public void ResetStates()
		{
			for(int i = 0; i < stateArray.Length; i++)
			{
				stateArray[i] = States.SingleTarget;
			}
		}
	}
	#endregion
}
