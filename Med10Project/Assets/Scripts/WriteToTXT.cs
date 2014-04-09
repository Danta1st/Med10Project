using UnityEngine;
using System.Collections;
using System.IO;

public class WriteToTXT : MonoBehaviour {

	private GUIManager gManager;
	private GameTimerManager gtManager;

	private string directorypath;
	private string filename;

	private string currentStringToWrite;

	private string userID;
	private string stage;
	private string time;
	private string reactiontime;
	private string angle;
	private string distance;
	private string targetpositionX;
	private string targetpositionY;
	private string touchpositionX;
	private string touchpositionY;
	private string hitOrMiss;

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
		userID = gManager.GetUserID();
		filename = userID+" - "+System.DateTime.Now.ToString()+".txt";
		filename = filename.Replace("/","-");
		filename = filename.Replace(":","-");

		using (StreamWriter writer = File.AppendText(directorypath + filename))
		{
			writer.WriteLine("userID;stage;hittime;reactiontime;angle;distance;targetpositionX;targetpositionY;touchpositionX;touchpositionY;hitormiss");
		}
	}

	public void LogData(string _stage, float _reactiontime, float _angle, float _distance, Vector3 _targetposition, Vector2 _touchposition, bool _wasHit)
	{
		userID = gManager.GetUserID();
		stage = _stage;
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

		if(_wasHit)
		{
			hitOrMiss = "hit";
		}
		else if(!_wasHit)
		{
			hitOrMiss = "miss";
		}

		currentStringToWrite = userID+";"+stage+";"+time+";"+reactiontime+";"+angle+";"+distance+";"+targetpositionX+";"+targetpositionY+";"+touchpositionX+";"+touchpositionY+";"+hitOrMiss;
		WriteTXT();
	}

	private void WriteTXT()
	{
		using (StreamWriter writer = File.AppendText(directorypath + filename))
		{
			writer.WriteLine(currentStringToWrite);
		}
	}

	private void NC_Play()
	{
		CreateDataFolder();
		CreateNewTXTFile();
	}
}
