using UnityEngine;
using System.Collections;

public class Phase2Object : MonoBehaviour 
{
	#region Editor Publics
	private int Lifetime = 3;
	#endregion


	#region Privates
	private GestureManager gManager;
	private SoundManager soundManager;
	private HighscoreManager highScoreManager;
	private GameStateManager gameManager;
	private Phase2Behavior phase2Center;

	//Object Information - Passed from spawner
	private int angle;
	private int objectID;
	private float distance;
	private float spawnTime;
	private int anglemultiplier;

	private int lifeCounter;

	private Color InvisibleColor = new Color(0,1.0f,0,0);
	private Color FullGreenColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
	private Color DisabledColor = new Color(1,1,1,1);

	private bool playModeActive = true;

	[SerializeField] private GameObject ParticleObject;
	private bool activeTarget = false;

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

		highScoreManager = GameObject.Find("HighscoreManager").GetComponent<HighscoreManager>();
		if(highScoreManager == null)
			Debug.LogError("No HighscoreManager was found in the scene.");
		
//		gaSubmitter = GameObject.Find("GA_Submitter").GetComponent<GA_Submitter>();
//		xmlLogger = GameObject.Find("XMLlogger").GetComponent<XmlData>();
		gameManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
		phase2Center = GameObject.Find("Phase2Center(Clone)").GetComponent<Phase2Behavior>();
	}

	void Start ()
	{
		lifeCounter = Lifetime;
		gameObject.renderer.material.color = DisabledColor;
		gameObject.transform.localScale = Vector3.zero;
		gManager.OnTapBegan += Hit;
		FadeIn();
		InvokeRepeating("DecreaseLifetime", 1, 1);
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Restart");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Pause");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Unpause");
	}

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
		anglemultiplier = multiplier;
	}

	void FadeIn()
	{
		iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one, "easetype", iTween.EaseType.easeOutBack, "time", 0.3f));
	}

	#region Class Methods	
	private void Hit(Vector2 screenPos)
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y, 0));
		RaycastHit hitInfo;
		if(Physics.Raycast(ray, out hitInfo))
		{
			if(hitInfo.collider == gameObject.collider && activeTarget)
			{
				soundManager.PlayTouchEnded();
				soundManager.PlayTargetSuccessHit();
				
				//Unsubscribe();

				phase2Center.SendHit();

				SpawnParticle();
				gameManager.StartCoroutine("SpawnCenterExplosion", transform.rotation);

				SetTargetDisabled();

				highScoreManager.AddScore(13, true);
				highScoreManager.IncreaseMultiplier();

			}
		}
	}

	private void HideTarget()
	{
		iTween.ColorTo(gameObject, iTween.Hash("color", InvisibleColor, "time", 0.0f));
		Destroy(gameObject, 2);
	}

	public void SetTargetDisabled()
	{
		iTween.ColorTo(gameObject, iTween.Hash("color", DisabledColor, "time", 0.3f));
		activeTarget = false;
	}

	public void SetActiveTarget()
	{
		iTween.ColorTo(gameObject, iTween.Hash("color", FullGreenColor, "time", 0.3f));
		activeTarget = true;
		lifeCounter = Lifetime;
	}

	private void SpawnParticle()
	{
		Instantiate(ParticleObject, transform.position, transform.rotation);
	}

	private void Miss()
	{
		SetTargetDisabled();
		phase2Center.SendMiss();
	}

	private void Unsubscribe()
	{
		gManager.OnTapBegan -= Hit;
	}

	private void DecreaseLifetime()
	{
		if(playModeActive && activeTarget){
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
