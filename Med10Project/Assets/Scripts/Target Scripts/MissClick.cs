using UnityEngine;
using System.Collections;

public class MissClick : MonoBehaviour 
{
	
	#region Publics
	#endregion
	
	#region Editor Publics
	#endregion
	
	#region Privates
	//Script connectivity
	private GestureManager gManager;
	private GameTimerManager timeManager;
	private SoundManager soundManager;
	private WriteToTXT txtWriter;
	private string missClick = "MissClick";
	#endregion
	
	void Awake()
	{
		gManager = Camera.main.GetComponent<GestureManager>();		
		soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
		txtWriter = GameObject.Find("WriteToTXT").GetComponent<WriteToTXT>();
		timeManager = GameObject.Find("GameTimerManager").GetComponent<GameTimerManager>();
	}

	void Start () 
	{
		//Subscribe to Tap Gesture
		gManager.OnTapBegan += HandleOnTapBegan;
		//Begin Observing for default notifications
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Play");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Pause");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Unpause");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Restart");
	
	}

	void HandleOnTapBegan (Vector2 screenPosition)
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(screenPosition.x, screenPosition.y, 0));
		RaycastHit hitInfo;
		if(Physics.Raycast(ray, out hitInfo) == false)
		{				
			soundManager.PlayTouchBegan();
			Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));
			Debug.Log("Missclick at: "+worldPos);
			txtWriter.LogData(missClick, 0.0f, 0, 0.0f, worldPos, screenPosition, missClick, 0, 0);
		}
	}

	
	void Update () 
	{
		
	}
	
	private void NC_Play()
	{
	}
	
	private void NC_Pause()
	{
	}
	
	private void NC_Unpause()
	{
	}
	
	private void NC_Restart()
	{
	}	

}
