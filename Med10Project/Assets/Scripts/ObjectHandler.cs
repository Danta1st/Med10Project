using UnityEngine;
using System.Collections;

public class ObjectHandler : MonoBehaviour 
{
	#region Editor Publics
	[SerializeField] private int Lifetime = 5;
	#endregion


	#region Privates
	private BpmManager bManager;
	private GestureManager gManager;
	private SpawnManager sManager;
	private SoundManager soundManager;
	private GA_Submitter gaSubmitter;
	private XmlData xmlLogger;
	private Center center;
	//Object Information - Passed from spawner
	private int angle;
	private int objectID;
	private float distance;
	private float spawnTime;

	private int lifeCounter;

	private Color InvisibleColor = new Color(0,1.0f,0,0);
	private Color FullGreenColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);

	#endregion
	
	void Awake()
	{
		//Get references
		soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
		if(soundManager == null)
			Debug.LogError("No SoundManager was found in the scene.");

		bManager = GameObject.Find("BpmManager").GetComponent<BpmManager>();
		if(bManager == null)
			Debug.LogError("No BpmManager was found in the scene.");

		gManager = Camera.main.GetComponent<GestureManager>();
		if(gManager == null)
			Debug.LogError("No GestureManager was found on the main camera.");
		
		sManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
		if(sManager == null)
			Debug.LogError("No SpawnManager was found in the scene.");
		
		gaSubmitter = GameObject.Find("GA_Submitter").GetComponent<GA_Submitter>();
		xmlLogger = GameObject.Find("XMLlogger").GetComponent<XmlData>();
		center = GameObject.Find("Center").GetComponent<Center>();
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
		bManager.OnBeat8th3 += DecreaseLifetime;
		FadeIn();
	}

	void FadeIn()
	{
		iTween.ColorTo(gameObject, iTween.Hash("color", FullGreenColor, "time", 0.3f));
		iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one, "easetype", iTween.EaseType.easeOutBack, "time", 0.3f));
	}

	void FadeOut()
	{
		iTween.ColorTo(gameObject, iTween.Hash("color", InvisibleColor, "time", 0.4f));
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

				//Submit Data to GA
				gaSubmitter.Angle(objectID, angle);
				gaSubmitter.Distance(objectID, distance);
				gaSubmitter.ReactionTime(objectID, Time.time - spawnTime);
				gaSubmitter.PositionSucces(objectID, transform.position);
				gaSubmitter.ForceSubmit();
				
				Unsubscribe();
				//Log Data to XML writer
				xmlLogger.SetPassed(true);
				xmlLogger.SetAngle(angle);
				xmlLogger.SetDistance(distance);
				xmlLogger.SetReactionTime(Time.time - spawnTime);
				xmlLogger.SetPosition(transform.position);
				xmlLogger.WriteTargetDataToXml();

				center.ChangeState(Center.State.awaitCenterClick);
				sManager.IncreaseSucces(); //TODO: Implement proper highscore system as independent object
				sManager.AllowSpawning();

				FadeOut();
				Destroy(gameObject, 2);
			}
		}
	}

	private void Miss()
	{
		//Submit Data
		gaSubmitter.Angle(objectID, angle);
		gaSubmitter.Distance(objectID, distance);
		gaSubmitter.ReactionTime(objectID, Time.time - spawnTime);
		gaSubmitter.PositionFailed(objectID, transform.position);
		gaSubmitter.ForceSubmit();

		Unsubscribe();
		center.ChangeState(Center.State.awaitCenterClick);
		//Log Data to XML writer
		xmlLogger.SetPassed(false);
		xmlLogger.SetAngle(angle);
		xmlLogger.SetDistance(distance);
		xmlLogger.SetReactionTime(0);
		xmlLogger.SetPosition(transform.position);
		xmlLogger.WriteTargetDataToXml();

		sManager.AllowSpawning();
		Destroy(gameObject);
	}

	private void Unsubscribe()
	{
		gManager.OnTapEnded -= Hit;
		gManager.OnTapBegan -= HandleOnTapBegan;
		bManager.OnBeat8th3 -= DecreaseLifetime;
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
	#endregion
}
