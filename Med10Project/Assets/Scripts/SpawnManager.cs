using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour 
{

	#region Editor Publics
	[SerializeField] GameObject spawnObject;
	#endregion

	#region Privates
	private BpmManager bManager;
	private GestureManager gManager;
	private GA_Submitter gaSubmitter;

	private bool isOccupied = false;

	private int objectCounter = 0;
	#endregion
	
	void Awake()
	{
		bManager = GameObject.Find("BpmManager").GetComponent<BpmManager>();
		if(bManager == null)
			Debug.LogError("No BpmManager was found in the scene.");
		
		gManager = Camera.main.GetComponent<GestureManager>();
		if(gManager == null)
			Debug.LogError("No GestureManager was found on the main camera.");

		gaSubmitter = GameObject.Find("GA_Submitter").GetComponent<GA_Submitter>();
		if(gaSubmitter == null)
			Debug.LogError("No GA_Submitter was found in the scene");
	}
	
	void Start ()
	{
		//TODO: Remove on delivery
		//gManager.OnSwipeRight += SpawnObjectRandom;
	}
	
	#region Class Methods
	private void SpawnObject()
	{

	}

	public void SpawnObjectRandom()
	{
		if(isOccupied == false)
		{
			//Get Random multiplier
			int multiplier = Random.Range(0, 35);
			//Get Random Angle
			int angle = 10 * multiplier;
			//Get Random Distance
			float distance = Random.Range(2.0f, 9.0f);
			//Rotate GameObject
			transform.Rotate(0, 0, (float) angle);
			//Get Rotation
			Quaternion rotation = transform.rotation;
			//Get Position
			Vector3 position = gameObject.transform.up * distance;
			//Rotate Back
			transform.Rotate(0, 0, angle);
			//Update Counter
			objectCounter++;
			//Instantiate Object
			GameObject go = (GameObject) Instantiate(spawnObject, position, rotation);
			//Set Object parameters
			go.GetComponent<ObjectHandler>().SetAngle((int) angle);
			go.GetComponent<ObjectHandler>().SetID(objectCounter);
			go.GetComponent<ObjectHandler>().SetDistance(distance);
			go.GetComponent<ObjectHandler>().SetSpawnTime(Time.time);
			//Set occupied
			isOccupied = true;
		}
	}

	public void AllowSpawning()
	{
		isOccupied = false;
	}

	//TODO: Integrate proper highscore system as seperate object
	private int succesCounter = 0;
	private float pWidth = 100;
	private float pHeight = 80;
	private Rect highScoreRect;
	public GUIStyle tempStyle;

	public void IncreaseSucces()
	{
		succesCounter++;

		if(succesCounter > 10)
		{
			Application.Quit();
		}
	}

//	void OnGUI()
//	{
//		highScoreRect = new Rect(0,0, pWidth, pHeight);
//		highScoreRect.center = new Vector2(Screen.width/2, 20);
//		GUI.Label(highScoreRect, ""+succesCounter, tempStyle);
//	}	
	#endregion
}
