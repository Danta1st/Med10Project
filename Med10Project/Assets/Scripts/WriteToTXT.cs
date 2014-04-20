using UnityEngine;
using System.Collections;
using System.IO;

public class WriteToTXT : MonoBehaviour {

	private GUIManager gManager;
	private GameTimerManager gtManager;

	private string directorypath;
	private string filename;

	private string currentStringToWrite;

	private int userID;					
	private int sessionID;				
	private string stage;				
	private string time;				
	private string reactiontime;
	private string angle;
	private string distance;
	private string targetpositionX;
	private string targetpositionY;
	private string touchpositionX;
	private string touchpositionY;
	private string hitType;

	// Use this for initialization
	void Start () {
		gManager = GameObject.Find("3DGUICamera").GetComponent<GUIManager>();
		gtManager = GameObject.Find("GameTimerManager").GetComponent<GameTimerManager>();
		directorypath = Application.persistentDataPath+"/Data/";
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Play");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private void CreateDataFolder()
	{
		if(!Directory.Exists(directorypath)){
			Directory.CreateDirectory(directorypath);
		}
	}

	private void CreateNewTXTFile(){
		string userPrefix = "User" + gManager.GetUserID();
		filename = userPrefix+" - "+System.DateTime.Now.ToString()+".txt";
		filename = filename.Replace("/","-");
		filename = filename.Replace(":","-");

//		using (StreamWriter writer = File.AppendText(directorypath + filename))
//		{
//			writer.WriteLine("userID;sessionID;targetID;stage;hitTime;reactionTime;angleID;angle;distance;targetPositionX;targetPositionY;touchPositionX;touchPositionY;hitType");
//		}
	}

	public void UpdateSessionID(int _userID)
	{
		string userID = "User"+_userID;
		//Check for userID and sessionID in playerPrefs
		if(PlayerPrefs.HasKey(userID))
			sessionID = PlayerPrefs.GetInt(userID) + 1;
		
		//Update userID and sessionID in playerPrefs
		PlayerPrefs.SetInt(userID, sessionID);
	}

	public void LogData(string _stage, float _reactiontime, int _angle, float _distance, 
	                    Vector3 _targetposition, Vector2 _touchposition, string _hitType, int targetID, int angleID)
	{
		userID = gManager.GetUserID();
		stage = _stage;
		hitType = _hitType;
		time = gtManager.GetCurrentPlayTime().ToString("#.00");

		if(_reactiontime < 1)
		{
			reactiontime = "0"+_reactiontime.ToString("#.000");
		}
		else
		{
			reactiontime = _reactiontime.ToString("#.000");
		}

		angle = _angle.ToString("#.00");
		distance = _distance.ToString("#.00");

		Vector3 tempVector = Camera.main.WorldToScreenPoint(_targetposition);
		targetpositionX = tempVector.x.ToString("#.0");
		targetpositionY = tempVector.y.ToString("#.0");

		touchpositionX = _touchposition.x.ToString("#.0");
		touchpositionY = _touchposition.y.ToString("#.0");

		currentStringToWrite = ""+userID+";"+sessionID+";"+targetID+";"+stage+";"+time+";"+reactiontime+";"+angleID+";"+angle+";"+distance+";"+targetpositionX+";"+targetpositionY+";"+touchpositionX+";"+touchpositionY+";"+hitType;
		WriteTXT();
	}

	private void WriteTXT()
	{
		using (StreamWriter writer = File.AppendText(directorypath + filename))
		{
			writer.WriteLine(currentStringToWrite);
		}
	}

	//Method that submits data to the mysql server. //TODO: test if works
	string phpScript_URL = "http://www.my-site.com/myScript.php";
	IEnumerator SubmitEntry(int targetID, int angleID)
	{
		// Create a form object for sending high score data to the server
		WWWForm form = new WWWForm();
		//Fill in information
		form.AddField("userID", userID);
		form.AddField("sessionID", sessionID);
		form.AddField("targetID", targetID);
		form.AddField("stage", stage);
		form.AddField("hitTime", time);
		form.AddField("reactionTime", reactiontime);
		form.AddField("angleID", angleID);
		form.AddField("angle", angle);
		form.AddField("distance", distance);
		form.AddField("targetPositionX", targetpositionX);
		form.AddField("targetPositionY", targetpositionY);
		form.AddField("touchPositionX", touchpositionX);
		form.AddField("touchPositionY", touchpositionY);
		form.AddField("hitType", hitType);
		//Submit the information
		WWW submit = new WWW(phpScript_URL, form);

		yield return submit;

		if(!string.IsNullOrEmpty(submit.error))
			Debug.Log("Error occured submitting data entry: "+submit.error);
		else
			Debug.Log("Succesfully submitted data entry");
	}

	private void NC_Play()
	{
		CreateDataFolder();
		CreateNewTXTFile();
	}
}
