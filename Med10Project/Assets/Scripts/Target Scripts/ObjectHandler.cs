using UnityEngine;
using System.Collections;

public class ObjectHandler : MonoBehaviour 
{
	#region Editor Publics
	[SerializeField] private ObjectTypes objectType = ObjectTypes.SingleTarget;
	[SerializeField] private float Lifetime = 2;
	[SerializeField] private GameObject ParticleObject;
	#endregion


	#region Privates
	//Connectivity & References
	private GestureManager gManager;
	private SpawnManager sManager;
	private SoundManager soundManager;
	private HighscoreManager highScoreManager;
	private GameStateManager gameManager;

	//Object Information - Passed from spawner
	private int angle;
	private int objectID;
	private float distance;
	private float spawnTime;
	private int angleIdentifier;

	//Animation times and punch count
	private float fadeInTime = 0.3f;
	private float fadeOutTime = 0.3f;
	private float punchTime = 0.5f;
	private int punches = 0;

	//Colors
	private Color InvisibleColor = new Color(0,1.0f,0,0);
	private Color FullGreenColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
	private Color FullBlueColor = Color.cyan; // new Color(0.0f, 0.0f, 1.0f, 1.0f);
	private Color DisabledColor = new Color(1,1,1,1);

	//Flags
	private bool playModeActive = true;
	private bool isPunching = true;

	private enum ObjectTypes {SingleTarget, SequentialTarget, MultiTarget};
	#endregion
	
	void Awake()
	{
		//Get references
		soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
		if(soundManager == null)
			Debug.LogError("No SoundManager was found in the scene.");

		gManager = Camera.main.GetComponent<GestureManager>();
		if(gManager == null)
			Debug.LogError("No GestureManager was found on the main camera.");
		
		sManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
		if(sManager == null)
			Debug.LogError("No SpawnManager was found in the scene.");

		highScoreManager = GameObject.Find("HighscoreManager").GetComponent<HighscoreManager>();
		if(highScoreManager == null)
			Debug.LogError("No HighscoreManager was found in the scene.");

		gameManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();

	}

	void Start ()
	{
		//Initialise object at scale zero - invisible
		gameObject.transform.localScale = Vector3.zero;

		//Initialise object with default color.
		if(objectType == ObjectTypes.SingleTarget)
			renderer.material.color = FullGreenColor;
		else if(objectType == ObjectTypes.SequentialTarget)
			renderer.material.color = FullBlueColor;

		//Subscribe to gestureManager
		gManager.OnTapBegan += Hit;

		//Calculate Punches
		CalculatePunches();

		//Subscribe to observable events
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Restart");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Pause");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Unpause");

		FadeIn();
	}

	void Update()
	{
		if(playModeActive)
		{
			//Should we punch?
			if(isPunching == false && Time.time < spawnTime + fadeInTime + punchTime * punches)
			{
				PunchObject();
			}
			//should we record a miss?
			else if(isPunching == false && Time.time >= spawnTime + Lifetime - fadeOutTime)
			{
				Miss();
			}
		}
	}
	
	#region Public Methods
	public void SetAngle(int degrees)
	{
		angle = degrees;
	}
	
	public void SetID(int ID)
	{
		objectID = ID;
	}
	
	public void SetDistance(float _distance)
	{
		distance = _distance;
	}
	
	public void SetSpawnTime(float time)
	{
		spawnTime = time;
	}
	
	public void SetMultiplier(int multiplier)
	{
		angleIdentifier = multiplier;
	}
	#endregion

	#region Class Methods
	private void CalculatePunches()
	{
		punches = (int) ((Lifetime - fadeInTime - fadeOutTime) / (punchTime));
	}

	private void FadeIn()
	{
		iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one, "easetype", iTween.EaseType.easeOutBack, "time", 0.3f, 
		                                       "oncomplete", "AllowPunching", "oncompletetarget", gameObject));
	}

	private void FadeOut(float fadeTime)
	{
		iTween.ColorTo(gameObject, iTween.Hash("color", InvisibleColor,"easetype", iTween.EaseType.easeInQuint, "time", fadeTime));
		iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.zero, "easetype", iTween.EaseType.easeInQuint, "time", fadeTime, 
		                                       "oncomplete", "DestroySelf", "oncompletetarget", gameObject));
	}

	private void PunchObject()
	{
		isPunching = true;

		Vector3 scale = new Vector3(0.2f, 0.2f, 0.2f);

		iTween.PunchScale(gameObject, iTween.Hash("amount", scale, "time", punchTime, 
		                                          "oncomplete", "AllowPunching", "oncompletetarget", gameObject));
	}

	private void AllowPunching()
	{
		isPunching = false;
	}

	private void DestroySelf()
	{
		Destroy(gameObject);
	}

	private void Hit(Vector2 screenPos)
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y, 0));
		RaycastHit hitInfo;
		if(Physics.Raycast(ray, out hitInfo))
		{
			if(hitInfo.collider == gameObject.collider)
			{
				soundManager.PlayTouchEnded();
				soundManager.PlayTargetSuccessHit();

				//Do stuff depending on object type
				if(objectType == ObjectTypes.SingleTarget)
				{
					gameManager.ChangeCenterState(GameStateManager.State.awaitCenterClick);
					sManager.ReportHit(angleIdentifier, distance);
					//TODO: Add score, reset multiplier
				}
				else if(objectType == ObjectTypes.SequentialTarget)
				{
					sManager.Phase1Stage2(angleIdentifier);
					//TODO: Add score, increase multiplier
				}
				else if(objectType == ObjectTypes.MultiTarget)
				{
					gameManager.IncreaseMultiTargetCounter();
				}

				SpawnParticle();
				gameManager.StartCoroutine("SpawnCenterExplosion", transform.rotation);
				
				Unsubscribe();
				Destroy(gameObject);
			}
		}
	}
	
	private void Miss()
	{
		Unsubscribe();
		gameManager.ChangeCenterState(GameStateManager.State.awaitCenterClick);
		
		sManager.ReportMiss(angleIdentifier, distance);
		FadeOut(fadeOutTime);
	}

	private void SetTargetDisabled()
	{
		iTween.ColorTo(gameObject, iTween.Hash("color", DisabledColor, "time", 0.3f));
	}

	private void SpawnParticle()
	{
		Instantiate(ParticleObject, transform.position, transform.rotation);
	}

	private void Unsubscribe()
	{
		gManager.OnTapBegan -= Hit;
	}

	private void NC_Restart()
	{
		Miss();
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
