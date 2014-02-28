using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class UserIDPrompter : MonoBehaviour {

	private string temporaryUserID = "Enter UserID here";
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
		filepath = Application.dataPath + @"/Data/";
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
			GUILayout.BeginArea(new Rect(Screen.width/2-Screen.width*0.1f, Screen.height/2-Screen.height*0.25f, Screen.width*0.2f, Screen.height*0.5f));
			GUILayout.Label("Enter new username: ");

			temporaryUserID = GUILayout.TextField(temporaryUserID);
			if(GUILayout.Button(buttonText))
			{
				Initiate();
			}

			if(fileInfo.Length > 0){
				GUILayout.FlexibleSpace();
				GUILayout.Label("Or choose an existing user");
				foreach (string userID in fileInfo)
				{
					string thisName = Path.GetFileNameWithoutExtension(userID);

					if(GUILayout.Button(thisName))
					{
						temporaryUserID = thisName;
						Initiate();
					}	
				}
			}
			GUILayout.EndArea();
		}
		else if(_displayPlayButton)
		{
			if(GUI.Button(new Rect(Screen.width/2-Screen.width*0.1f, Screen.height/2-Screen.height*0.06f, Screen.width*0.2f, Screen.height*0.1f), "Play"))
			{
				beatsmanager.ToggleBeats();
				_displayPlayButton = false;
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
	
}
