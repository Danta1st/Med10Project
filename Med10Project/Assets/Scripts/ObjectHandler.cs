using UnityEngine;
using System.Collections;

public class ObjectHandler : MonoBehaviour 
{

	#region Privates
	private BpmManager bManager;
	private GestureManager gManager;
	private SpawnManager sManager;
	private GA_Submitter gaSubmitter;
	#endregion
	
	void Awake()
	{
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
	}

	private int angle;
	private int objectID;
	private float distance;
	private float spawnTime;

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
		audio.Play();
		gManager.OnTap += DestroySelf;
	}
	
	#region Class Methods	
	private void DestroySelf(Vector2 screenPos)
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y, 0));
		RaycastHit hitInfo;
		if(Physics.Raycast(ray, out hitInfo))
		{
			if(hitInfo.collider == gameObject.collider)
			{
				//Submit Data
				gaSubmitter.Angle(objectID, angle);
				gaSubmitter.Distance(objectID, distance);
				gaSubmitter.CompletionTime(objectID, Time.time - spawnTime);
				gaSubmitter.Position(objectID, transform.position);
				gaSubmitter.ForceSubmit();

				sManager.AllowSpawning();
				gManager.OnTap -= DestroySelf;
				Destroy(gameObject);
			}
		}
	}
	#endregion
}
