using UnityEngine;
using System.Collections;

public class ObjectHandler : MonoBehaviour 
{
	#region Editor Publics
	[SerializeField] private int Lifetime = 5;
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

	private int lifeCounter;

	private Color InvisibleColor = new Color(0,1.0f,0,0);
	private Color FullGreenColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);

	[SerializeField]
	private GameObject ParticleObject;

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
	
	void Start ()
	{
		gameObject.renderer.material.color = InvisibleColor;
		gameObject.transform.localScale = Vector3.zero;
		gManager.OnTapBegan += HandleOnTapBegan;
		gManager.OnTapEnded += Hit;
		FadeIn();
		InvokeRepeating("DecreaseLifetime", 1, 1);
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Restart");
	}

	void FadeIn()
	{
		iTween.ColorTo(gameObject, iTween.Hash("color", FullGreenColor, "time", 0.3f));
		iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one, "easetype", iTween.EaseType.easeOutBack, "time", 0.3f));
	}

	void FadeOut(float fadeTime)
	{
		iTween.ColorTo(gameObject, iTween.Hash("color", InvisibleColor, "time", fadeTime));
		iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.zero, "easetype", iTween.EaseType.easeInBack, "time", 0.3f));
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

				gameManager.ChangeState(GameStateManager.State.awaitCenterClick);
				sManager.IncreaseSucces(); //TODO: Implement proper highscore system as independent object
				sManager.AllowSpawning();

				SpawnParticle();
				gameManager.StartCoroutine("SpawnCenterExplosion", transform.rotation);
				FadeOut(0.05f);
				highScoreManager.AddScore(13, true);
				highScoreManager.IncreaseMultiplier();
				Destroy(gameObject, 2);
			}
		}
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

		sManager.AllowSpawning();
		FadeOut(0.3f);
		Destroy(gameObject, 1);
	}

	private void Unsubscribe()
	{
		gManager.OnTapEnded -= Hit;
		gManager.OnTapBegan -= HandleOnTapBegan;
	}

	private void DecreaseLifetime()
	{
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
	#endregion
}
