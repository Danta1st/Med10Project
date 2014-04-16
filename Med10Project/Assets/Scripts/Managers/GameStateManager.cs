using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameStateManager : MonoBehaviour 
{
	#region Publics
	#endregion

	#region Editor Publics
	[SerializeField] private float spawnTime = 3.0f;
	[SerializeField] private GameObject CenterExplosion;
	[SerializeField] private int calibrationAmount = 15;
	#endregion

	#region Privates
	//Script connectivity
	private GestureManager gManager;
	private GUIManager guiManager;
	private SpawnManager sManager;
	private SoundManager soundManager;
	private ClockHandler clock;
	private HighscoreManager highscoreManager;

	//Time logs
	private float SpawnBegin;

	//Booleans
	private bool allowPunching = true;
	private bool playModeActive = false;

	//States
	[HideInInspector] public enum State {awaitCenterClick, awaitTargetSpawn, awaitTargetClick, awaitTargetReturnToCenter};
	[HideInInspector] public State state;
	private Phase1States phase1States = new Phase1States();
	public enum Phases {Phase0, Phase1, Phase2, Phase3};
	private Phases phase;

	//Counters
	private int calibrationCounter = 0;
	private int multiTargetCounter = 0;
	#endregion

	void Awake()
	{
		gManager = Camera.main.GetComponent<GestureManager>();
		if(gManager == null)
			Debug.LogError("No GestureManager was found on the main camera.");

		guiManager = GameObject.Find("3DGUICamera").GetComponent<GUIManager>();
		highscoreManager = GameObject.Find("HighscoreManager").GetComponent<HighscoreManager>();

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
		//Begin at Phase 0 - Calibration
		phase = Phases.Phase0;
		//Subscribe to Tap Gesture
		gManager.OnTapBegan += HandleOnTapBegan;
		//Begin Punching
		InvokeRepeating("PunchCenter", 0, 1.25f);

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
				//Change the state
				ChangeCenterState(State.awaitTargetClick);

				//Spawn according to state
				if(phase == Phases.Phase0)
				{
					if(calibrationCounter < calibrationAmount)
					{
						//Spawn calibration Target
						sManager.SpawnCalibration();
						soundManager.PlayNewTargetSpawned();
						//Increase Count
						calibrationCounter++;
					}
					else
					{
						float rt = highscoreManager.GetAverageFloatReactiontime();
						//Calibration done. Set user Mean reaction Time in SpawnManager
						sManager.SetAverageReactionTime(rt);
						//Flag for phase1
						phase = Phases.Phase1;
						sManager.SpawnCalibration();
					}

				}
				else if(phase == Phases.Phase1)
				{
					//Get random target
					int int1to10 = GetWeightedRandom();

					//Spawn target according to current state
					if(phase1States.GetAngleState(int1to10) == Phase1States.States.SingleTarget ||
					   phase1States.GetAngleState(int1to10) == Phase1States.States.SequentialTarget)
					{
						soundManager.PlayNewTargetSpawned();
						sManager.Phase1Stage1(int1to10);
					}
					else if(phase1States.GetAngleState(int1to10) == Phase1States.States.MultipleTargets)
					{
						soundManager.PlayNewTargetSpawned();
						sManager.Phase1Stage3(int1to10);
					}
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
				ChangeCenterState(State.awaitCenterClick);
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

	#region Class Methods
	private void PunchCenter()
	{
		if(!(state == State.awaitTargetClick) && allowPunching && playModeActive)
		{
			ResetGOScale();
			iTween.PunchScale(gameObject, new Vector3(0.2f, 0.2f, 0.2f), 0.5f);
//			soundManager.PlayCenterPunch();
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

	private List<Phase1States.States> lastSpawnStates = new List<Phase1States.States>();
	private int GetWeightedRandom()
	{
		if(lastSpawnStates.Count() == 2 && lastSpawnStates[0] == lastSpawnStates[1])
		{
			if(lastSpawnStates[0] == Phase1States.States.MultipleTargets || lastSpawnStates[0] == Phase1States.States.SequentialTarget)
			{
				List<int> singleTargetList = new List<int>();
				List<int> sequentialTargetList = new List<int>();

				for(int i = 1; i <= 10; i++)
				{
					//Gather all current singleTarget angles
					if(phase1States.GetAngleState(i) == Phase1States.States.SingleTarget)
					{
						singleTargetList.Add(i);
					}
					//Gather all current SequentialTarget angles
					else if(phase1States.GetAngleState(i) == Phase1States.States.SequentialTarget)
					{
						sequentialTargetList.Add(i);
					}
				}

				if(singleTargetList.Count() != 0)
				{
					//Get random singleTarget from current angles of single targets
					int j = Random.Range(1, singleTargetList.Count() + 1);
					//Add item at the end of latest spawn list
					lastSpawnStates.Add(phase1States.GetAngleState(singleTargetList[j-1]));
					//If more than two items in list, remove the front index
					if(lastSpawnStates.Count() > 2)
						lastSpawnStates.RemoveAt(0);
					//Return angle ID to spawn
					return singleTargetList[j-1];
				}
				else if(sequentialTargetList.Count() != 0)
				{
					//Get random sequentialTarget from current angles of single targets
					int j = Random.Range(1, sequentialTargetList.Count() + 1);
					//Add item at the end of latest spawn list
					lastSpawnStates.Add(phase1States.GetAngleState(sequentialTargetList[j-1]));
					//If more than two items in list, remove the front index
					if(lastSpawnStates.Count() > 2)
						lastSpawnStates.RemoveAt(0);
					//Return angle ID to spawn
					return sequentialTargetList[j-1];
				}
			}
		}

		//If none of the above. Spawn total random
		int k = Random.Range(1, 11);
		//Add item at the end of latest spawn list
		lastSpawnStates.Add(phase1States.GetAngleState(k));
		//If more than two items in list, remove the front index
		if(lastSpawnStates.Count() > 2)
			lastSpawnStates.RemoveAt(0);
		//Return angle ID to spawn
		return k;
	}
	
	//Begin at Specific Phase -----------------------------------
	private void BeginState_1()
	{
		phase1States.SetStates(Phase1States.States.SingleTarget);
		ChangeCenterState(State.awaitCenterClick);
	}
	
	private void BeginState_2()
	{
		for(int i = 1; i <= 10; i += 2)
		{
			sManager.SetLongestHit(i);
			phase1States.SetAngleState(i, Phase1States.States.SequentialTarget);
		}
		
		ChangeCenterState(State.awaitCenterClick);
	}
	
	private void BeginState_3()
	{
		for(int i = 1; i <= 10; i += 2)
		{
			phase1States.SetAngleState(i, Phase1States.States.MultipleTargets);
		}
		ChangeCenterState(State.awaitCenterClick);
	}
	
	private void BeginState_4()
	{
		phase = Phases.Phase2;
		ChangeCenterState(State.awaitTargetClick);
		sManager.Phase2Stage1();
		GameObject.Find("Phase2Center(Clone)").GetComponent<Phase2Behavior>().SetStageRight();
//		ChangeCenterState(State.awaitCenterClick);
	}

	private void BeginState_5()
	{
		phase = Phases.Phase2;
		ChangeCenterState(State.awaitTargetClick);
		sManager.Phase2Stage1();
		GameObject.Find("Phase2Center(Clone)").GetComponent<Phase2Behavior>().SetStageLeft();
	}

	private void BeginState_6()
	{
		phase = Phases.Phase2;
		ChangeCenterState(State.awaitTargetClick);
		sManager.Phase2Stage1();
		GameObject.Find("Phase2Center(Clone)").GetComponent<Phase2Behavior>().SetStageBoth();
	}

	private void BeginAtShortCut()
	{
		string stageToBeginAt = guiManager.GetStage();

		switch(stageToBeginAt)
		{
		case "Single Target":
			BeginState_1();
			break;
		case "Sequential Target":
			BeginState_2();
			break;
		case "Multi Target":
			BeginState_3();
			break;
		case "Identify Right":
			BeginState_4();
			break;
		case "Identify Left":
			BeginState_5();
			break;
		case "Identify Both":
			BeginState_6();
			break;
		}
	}
	//-----------------------------------------------------------

	private void NC_Play()
	{
		BeginAtShortCut();
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

		public void SetStates(States state)
		{
			for(int i = 0; i < stateArray.Length; i++)
			{
				stateArray[i] = state;
			}
		}
	}
	#endregion
}
