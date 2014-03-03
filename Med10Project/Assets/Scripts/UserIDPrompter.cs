using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class UserIDPrompter : MonoBehaviour {

	private string temporaryUserID = "EnterUserIDhere";
	private string buttonText = "OK"; 
	private bool _displayTextField = true;
	private bool _displayPlayButton = false;

	private GA_Submitter ga_submitter;
	private XmlData xmllogger;
	private BpmManager beatsmanager;

	private string filepath;
	private string[] fileInfo;
	
	// Use this for initialization
	void Start () {
		if(!Directory.Exists(Application.persistentDataPath + @"/Data/")){
			Directory.CreateDirectory(Application.persistentDataPath + @"/Data/");
		}
		filepath = Application.persistentDataPath + @"/Data/";
		fileInfo = Directory.GetFiles(filepath, "*.xml");
		ga_submitter = GameObject.Find("GA_Submitter").GetComponent<GA_Submitter>();
		xmllogger = GameObject.Find ("XMLlogger").GetComponent<XmlData>();
		beatsmanager = GameObject.Find ("BpmManager").GetComponent<BpmManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{

		if(_displayTextField)
		{ 

			GUI.Label(new Rect(0,0,400,40), "Enter new username: ");
			
			temporaryUserID = GUI.TextField(new Rect(0,50,400,40), temporaryUserID);
			if(GUI.Button(new Rect(0,100,400,40), buttonText))
			{
				Initiate();
			}
			
			if(fileInfo.Length > 0){
				GUI.Label(new Rect(0,150,400,40),"Or choose an existing user");


				//foreach (string userID in fileInfo)
				for(int i = 0; i < fileInfo.Length; i++)
				{
					string thisName = Path.GetFileNameWithoutExtension(fileInfo[i]);
					
					if(GUI.Button(new Rect(0,200+(i*50),400,40),thisName))
					{	
						temporaryUserID = thisName;
						Initiate();
					}	
				}
			}
		}
		
		else if(_displayPlayButton)
		{
			//GUI.Label(new Rect(0,0, 200,300), Application.persistentDataPath.ToString());

			if(GUI.Button(new Rect(Screen.width/2-Screen.width*0.1f, Screen.height/2-Screen.height*0.06f, Screen.width*0.2f, Screen.height*0.1f), "Play"))
			{
				StartGame();
			}
		}
	}

	private void Initiate()
	{
		ga_submitter.SetUserID(temporaryUserID);
		ga_submitter.BeginTracing();
		xmllogger.IntiateXMLLogging();
		_displayTextField = false;
		_displayPlayButton = true;
	}

	private void StartGame()
	{
		beatsmanager.ToggleBeats();
		_displayPlayButton = false;
	}
}
