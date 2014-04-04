using UnityEngine;
using System.Collections;

public class ObjectHandler : MonoBehaviour 
{
	#region Editor Publics
	[SerializeField] private ObjectTypes objectType = ObjectTypes.SingleTarget;
	[SerializeField] private int Lifetime = 2;
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

	private int lifeCounter;

	//Colors
	private Color InvisibleColor = new Color(0,1.0f,0,0);
	private Color FullGreenColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
	private Color FullBlueColor = new Color(0.0f, 0.0f, 1.0f, 1.0f);
	private Color DisabledColor = new Color(1,1,1,1);

	private bool playModeActive = true;
	private bool hideHitTargets = true;

	private enum ObjectTypes {SingleTarget, SequentialTarget};
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

		//Initiliase
		lifeCounter = Lifetime;
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

	void Start ()
	{
		//Initialise object at scale zero - invisible
		gameObject.transform.localScale = Vector3.zero;

		//Initialise object with default color.
		if(objectType == ObjectTypes.SingleTarget)
			renderer.material.color = FullGreenColor;
		else if(objectType == ObjectTypes.SequentialTarget)
			renderer.material.color = FullBlueColor;

		gManager.OnTapBegan += Hit;
		FadeIn();
		InvokeRepeating("DecreaseLifetime", 1, 1);

		//Subscripe to observable events
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Restart");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Pause");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Unpause");
	}
	
	#region Class Methods
	private void FadeIn()
	{
		iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one, "easetype", iTween.EaseType.easeOutBack, "time", 0.3f));

		if(hideHitTargets)
		{
			FadeOut(Lifetime);
		}
	}

	private void FadeOut(float fadeTime)
	{
		iTween.ColorTo(gameObject, iTween.Hash("delay", 0.3f, "color", InvisibleColor, "time", fadeTime));
		iTween.ScaleTo(gameObject, iTween.Hash("delay", 0.3f, "scale", Vector3.zero, "easetype", iTween.EaseType.easeInBack, "time", fadeTime));
	}

	private void HandleOnTapBegan (Vector2 screenPos)
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
				
				Unsubscribe();

				//Do stuff depending on object type
				if(objectType == ObjectTypes.SingleTarget)
				{
					gameManager.ChangeCenterState(GameStateManager.State.awaitTargetReturnToCenter);
					sManager.ReportHit(angleIdentifier, distance);
					//Add score
				}
				else if(objectType == ObjectTypes.SequentialTarget)
				{
					sManager.Phase1Stage2(angleIdentifier);
					//Add score, increase multiplier
				}
				sManager.AllowSpawning();
				SpawnParticle();
				gameManager.StartCoroutine("SpawnCenterExplosion", transform.rotation);

				if(hideHitTargets)
				{
					HideTarget();
				}
				else{
					SetTargetDisabled();
				}
			}
		}
	}

	private void HideTarget()
	{
		iTween.ColorTo(gameObject, iTween.Hash("color", InvisibleColor, "time", 0.0f));
		Destroy(gameObject, 2);
	}

	private void SetTargetDisabled()
	{
		iTween.ColorTo(gameObject, iTween.Hash("color", DisabledColor, "time", 0.3f));
	}

	private void SpawnParticle()
	{
		Instantiate(ParticleObject, transform.position, transform.rotation);
	}

	private void Miss()
	{
		Unsubscribe();
		gameManager.ChangeCenterState(GameStateManager.State.awaitCenterClick);

		sManager.ReportMiss(angleIdentifier, distance);
		sManager.AllowSpawning();
		FadeOut(0.3f);
		Destroy(gameObject);
	}

	private void Unsubscribe()
	{
		gManager.OnTapBegan -= Hit;
	}

	private void DecreaseLifetime()
	{
		if(playModeActive){
			if(lifeCounter <= 0)
			{
				Miss();
			}
			else
			{
				lifeCounter--;
				PunchObject();
			}
		}
	}

	private void PunchObject()
	{
		iTween.PunchScale(gameObject, new Vector3(0.2f, 0.2f, 0.2f), 0.5f);
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
