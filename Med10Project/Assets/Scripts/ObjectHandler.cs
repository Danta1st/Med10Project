using UnityEngine;
using System.Collections;

public class ObjectHandler : MonoBehaviour 
{
	#region Editor Publics
	[SerializeField] private int Lifetime = 2;
	#endregion


	#region Privates
	private GestureManager gManager;
	private SpawnManager sManager;
	private SoundManager soundManager;
	private HighscoreManager highScoreManager;
//	private GA_Submitter gaSubmitter;
//	private XmlData xmlLogger;
	private GameStateManager gameManager;
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

	[SerializeField]
	private GameObject ParticleObject;

	private bool hideHitTargets = true;

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
		
//		gaSubmitter = GameObject.Find("GA_Submitter").GetComponent<GA_Submitter>();
//		xmlLogger = GameObject.Find("XMLlogger").GetComponent<XmlData>();
		gameManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();
		//Initiliase
		lifeCounter = Lifetime;
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
	
	void Start ()
	{
		//gameObject.renderer.material.color = InvisibleColor;
		gameObject.transform.localScale = Vector3.zero;
		gManager.OnTapBegan += Hit;
		//gManager.OnTapEnded += Hit;
		FadeIn();
		InvokeRepeating("DecreaseLifetime", 1, 1);
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Restart");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Pause");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Unpause");
	}

	void FadeIn()
	{
		iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one, "easetype", iTween.EaseType.easeOutBack, "time", 0.3f));

		if(hideHitTargets)
		{
			FadeOut(Lifetime);
		}
	}

	void FadeOut(float fadeTime)
	{
		iTween.ColorTo(gameObject, iTween.Hash("delay", 0.3f, "color", InvisibleColor, "time", fadeTime));
		iTween.ScaleTo(gameObject, iTween.Hash("delay", 0.3f, "scale", Vector3.zero, "easetype", iTween.EaseType.easeInBack, "time", fadeTime));
	}

	void HandleOnTapBegan (Vector2 screenPos)
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
	
	#region Class Methods	
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

//				//Submit Data to GA
//				gaSubmitter.Angle(objectID, angle);
//				gaSubmitter.Distance(objectID, distance);
//				gaSubmitter.ReactionTime(objectID, Time.time - spawnTime);
//				gaSubmitter.PositionSucces(objectID, transform.position);
//				gaSubmitter.ForceSubmit();
				
				Unsubscribe();
//				//Log Data to XML writer
//				xmlLogger.SetPassed(true);
//				xmlLogger.SetAngle(angle);
//				xmlLogger.SetDistance(distance);
//				xmlLogger.SetReactionTime(Time.time - spawnTime);
//				xmlLogger.SetPosition(transform.position);
//				xmlLogger.WriteTargetDataToXml();

				gameManager.ChangeState(GameStateManager.State.awaitTargetReturnToCenter);
				//sManager.IncreaseSucces(); //TODO: Implement proper highscore system as independent object
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

				sManager.IncreaseDistanceInArray(anglemultiplier);

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
//		//Submit Data
//		gaSubmitter.Angle(objectID, angle);
//		gaSubmitter.Distance(objectID, distance);
//		gaSubmitter.ReactionTime(objectID, Time.time - spawnTime);
//		gaSubmitter.PositionFailed(objectID, transform.position);
//		gaSubmitter.ForceSubmit();

		Unsubscribe();
		gameManager.ChangeState(GameStateManager.State.awaitCenterClick);

//		//Log Data to XML writer
//		xmlLogger.SetPassed(false);
//		xmlLogger.SetAngle(angle);
//		xmlLogger.SetDistance(distance);
//		xmlLogger.SetReactionTime(0);
//		xmlLogger.SetPosition(transform.position);
//		xmlLogger.WriteTargetDataToXml();

		sManager.DecreaseDistanceInArray(anglemultiplier);
		sManager.AllowSpawning();
		FadeOut(0.3f);
		Destroy(gameObject);
	}

	private void Unsubscribe()
	{
		//gManager.OnTapEnded -= Hit;
		//gManager.OnTapBegan -= HandleOnTapBegan;
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

	private void PlaySucces()
	{

	}

	private void PlayMiss()
	{

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
