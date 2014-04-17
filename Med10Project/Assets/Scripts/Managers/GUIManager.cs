﻿using UnityEngine;
using System.Collections;

public class GUIManager: MonoBehaviour {
	
	#region Editor Publics
	[SerializeField] private float curtainSpeed = 1.0f;
	[SerializeField] private iTween.EaseType curtainEasetype = iTween.EaseType.easeOutCirc;
	[SerializeField] private GUIStyle guiLook;
	[SerializeField] private GUIStyles guiStyles;
	[SerializeField] private GUIElements guiElements;
	[SerializeField] private Vector2 windowMetrics;
	#endregion
	
	#region Privates
	//Connectivity
	private HighscoreManager scoreManager;
	private WriteToTXT txtLogger;

	//Rects
	private Rect highscoreRect;
	private Rect windowRect;
	private Rect countdownRect;
	//Conditioning
	private GUIBools guiBools = new GUIBools();
	//Variables
	private Vector3 LeftCoverBeginPos;
	private Vector3 RightCoverBeginPos;
	//Strings
	private string currentUser;
	private string currentCountdownNumber;
	private string currentStage;

	private int hits;
	private int misses;
	private string avgReactionTime;

	private bool gameOver = false;
	#endregion

	void Start()
	{
		//Connectivity
		scoreManager = GameObject.Find("HighscoreManager").GetComponent<HighscoreManager>();
		txtLogger = GameObject.Find("WriteToTXT").GetComponent<WriteToTXT>();
		//Rect initialization
		highscoreRect = new Rect(0, 0, 100, 80);
		highscoreRect.center = new Vector2(GetCenterWidth(), 20);
		windowRect = new Rect(0,0, Screen.width * windowMetrics.x, Screen.height * windowMetrics.y);
		windowRect.center = new Vector2(GetCenterWidth(), getCenterHeight());
		countdownRect = new Rect(0,0,400,200);
		countdownRect.center = new Vector2(GetCenterWidth(), getCenterHeight());

		LeftCoverBeginPos = guiElements.LeftCover.transform.position;
		RightCoverBeginPos = guiElements.RightCover.transform.position;

		NotificationCenter.DefaultCenter().AddObserver(this, "NC_GameOver");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Restart");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Pause");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Unpause");
	}

	void OnGUI()
	{
		if(guiBools.displayUserSelection == true)
		{
			windowRect = GUILayout.Window(0, windowRect, DoUserSelectionWindow, "", guiStyles.WindowFrameStyle);
		}

		if(guiBools.displayStageSelection == true)
		{
			windowRect = GUILayout.Window(0, windowRect, DoStageSelectionWindow, "", guiStyles.WindowFrameStyle);
		}

		if(guiBools.displayHighscore == true)
		{			
			GUI.Label(highscoreRect, ""+scoreManager.GetScore(), guiLook);
		}

		if(guiBools.displayExitConfirmation == true)
		{
			windowRect = GUILayout.Window(0, windowRect, DoExitConfirmationWindow, "", guiStyles.WindowFrameStyle);
		}

		if(guiBools.displayPlayPrompt)
		{
			windowRect = GUILayout.Window(0, windowRect, DoPlayPromptWindow, "", guiStyles.WindowFrameStyle);
		}

		if(guiBools.displayCountDown)
		{
			GUI.Label(countdownRect, ""+currentCountdownNumber, guiStyles.CountdownStyle);
		}

		if(guiBools.displayEndScreen)
		{
			windowRect = GUILayout.Window(0, windowRect, DoEndScreenWindow, "", guiStyles.WindowFrameStyle);
			//GUI.Label(countdownRect, "Hits: "+hits, guiStyles.CountdownStyle);
		}
	}

	#region GUI Windows
	private void DoUserSelectionWindow(int windowID)
	{
		if(PlaceButton("User 1"))
		{
			guiBools.displayUserSelection = false;
			guiBools.displayPlayPrompt = true;
			//guiBools.displayStageSelection = true;
			currentUser = "User 1";
		}

		GUILayout.Space(5);

		if(PlaceButton("User 2"))
		{
			guiBools.displayUserSelection = false;
			guiBools.displayPlayPrompt = true;
			//guiBools.displayStageSelection = true;
			currentUser = "User 2";
		}


		GUILayout.FlexibleSpace();

		if(PlaceButton("Exit Game"))
		{
			Application.Quit();
		}

	}

	private void DoStageSelectionWindow(int windowID)
	{
		if(PlaceButton("Single Target"))
		{
			guiBools.displayStageSelection = false;
			guiBools.displayPlayPrompt = true;
			currentStage = "Single Target";
		}
		
		if(PlaceButton("Sequential Target"))
		{
			guiBools.displayStageSelection = false;
			guiBools.displayPlayPrompt = true;
			currentStage = "Sequential Target";
		}
		
		if(PlaceButton("Multi Target"))
		{
			guiBools.displayStageSelection = false;
			guiBools.displayPlayPrompt = true;
			currentStage = "Multi Target";
		}
		
		if(PlaceButton("Identify Right"))
		{
			guiBools.displayStageSelection = false;
			guiBools.displayPlayPrompt = true;
			currentStage = "Identify Right";
		}
		
		if(PlaceButton("Identify Left"))
		{
			guiBools.displayStageSelection = false;
			guiBools.displayPlayPrompt = true;
			currentStage = "Identify Left";
		}
		
		if(PlaceButton("Identify Both"))
		{
			guiBools.displayStageSelection = false;
			guiBools.displayPlayPrompt = true;
			currentStage = "Identify Both";
		}
	}

	private void DoExitConfirmationWindow(int windowID)
	{
		GUILayout.FlexibleSpace();

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("Continue or Exit?", guiStyles.WindowLabelStyle);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		if(!gameOver)
		{
			if(PlaceButton("Continue"))
			{
				guiBools.displayExitConfirmation = false;
				NotificationCenter.DefaultCenter().PostNotification(this, "NC_Unpause");
			}
		}

		/*if(PlaceButton("Exit Game))
		{
			//TODO: Pause Game
			//TODO: Save, send, log Data
			Application.Quit();
		}*/

		GUILayout.Space(5);

		if(PlaceButton("Exit To Menu"))
		{
			guiBools.displayExitConfirmation = false;
			guiBools.displayUserSelection = true;
			BlockAll(true);
			EnableHighscore(false);
			NotificationCenter.DefaultCenter().PostNotification(this, "NC_Restart");
		}
	}

	private void DoPlayPromptWindow(int windowID)
	{
		GUILayout.FlexibleSpace();

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(currentUser + " selected.", guiStyles.WindowLabelStyle);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		if(PlaceButton("Play"))
		{
			guiBools.displayPlayPrompt = false;
			txtLogger.UpdateSessionID(currentUser);
//			EnableHighscore(true);
			StartCoroutine(StartCountDown());
		}

		GUILayout.Space(5);

		if(PlaceButton("Cancel"))
		{
			guiBools.displayPlayPrompt = false;
			guiBools.displayUserSelection = true;
		}

	}

	private void DoEndScreenWindow(int windowID)
	{
		GUILayout.FlexibleSpace();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("Times Up!", guiStyles.WindowLabelStyle);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("Hits: "+hits, guiStyles.WindowScoreLabelStyle);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("Misses: "+ misses, guiStyles.WindowScoreLabelStyle);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("Average Reaction Time: "+ avgReactionTime, guiStyles.WindowScoreLabelStyle);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(PlaceButton("Exit To Menu"))
		{
			guiBools.displayEndScreen = false;
			guiBools.displayUserSelection = true;
			BlockAll(true);
			EnableHighscore(false);
			NotificationCenter.DefaultCenter().PostNotification(this, "NC_Restart");
		}
		GUILayout.Space(5);
		if(PlaceButton("Exit Game"))
		{
			Application.Quit();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	private IEnumerator StartCountDown()
	{
		guiBools.displayCountDown = true;
		currentCountdownNumber = "3";
		yield return new WaitForSeconds(1.0f);
		currentCountdownNumber = "2";
		yield return new WaitForSeconds(1.0f);
		currentCountdownNumber = "1";
		yield return new WaitForSeconds(1.0f);
		currentCountdownNumber = "";
		guiBools.displayCountDown = false;
		BlockAll(false);
		NotificationCenter.DefaultCenter().PostNotification(this, "NC_Play");
	}
	
	#endregion


	#region Public Methods
	public string GetUserID()
	{
		return currentUser;
	}

	//Highscore Logic-----------------------
	public void EnableHighscore(bool state)
	{
		if(state == true && guiBools.displayHighscore == false)
		{
			guiElements.Highscore.SetActive(true);
			guiBools.displayHighscore = true;
		}
		else if(state == false && guiBools.displayHighscore == true)
		{
			guiElements.Highscore.SetActive(false);
			guiBools.displayHighscore = false;
		}
	}

	public void EnableEndScreen(bool state)
	{
		//BlockAll(true);
		if(state == true)
		{
			hits = scoreManager.GetHits();
			misses = scoreManager.GetMisses();
			avgReactionTime = scoreManager.GetAverageStringReactiontime()+" seconds";
			StartCoroutine(EndGameDelayedScreen(true));		}
		else if (state == false)
		{
			StartCoroutine(EndGameDelayedScreen(false));
		}

	}

	private IEnumerator EndGameDelayedScreen(bool state)
	{
		if(state == true)
		{
			yield return new WaitForSeconds(1.0f);
			guiBools.displayEndScreen = true;
		}
		else if (state == false)
		{
			yield return new WaitForSeconds(0.0f);
			guiBools.displayEndScreen = false;
		}
	}

	//Curtain Logic-------------------------
	public void BlockLeftHalf(bool state)
	{
		if(state == true && guiBools.displayLeftHalf == true)
		{
			guiBools.displayLeftHalf = false;
			var newPosition = new Vector3(LeftCoverBeginPos.x, 
			                              LeftCoverBeginPos.y, 
			                              LeftCoverBeginPos.z);
			iTween.MoveTo(guiElements.LeftCover, iTween.Hash("position", newPosition, 
		                                                 	 "time", curtainSpeed, 
			                                                 "easetype", curtainEasetype));
		}
		else if(state == false && guiBools.displayLeftHalf == false)
		{
			guiBools.displayLeftHalf = true;
			var newPosition = new Vector3(LeftCoverBeginPos.x - 17.0f, 
			                              LeftCoverBeginPos.y, 
			                              LeftCoverBeginPos.z);
			iTween.MoveTo(guiElements.LeftCover, iTween.Hash("position", newPosition, 
			                                                 "time", curtainSpeed, 
			                                                 "easetype", curtainEasetype));
		}
	}

	public void BlockRightHalf(bool state)
	{
		if(state == true && guiBools.displayRightHalf == true)
		{
			guiBools.displayRightHalf = false;
			var newPosition = new Vector3(RightCoverBeginPos.x, 
			                              RightCoverBeginPos.y, 
			                              RightCoverBeginPos.z);
			iTween.MoveTo(guiElements.RightCover, iTween.Hash("position", newPosition, 
			                                                  "time", curtainSpeed, 
			                                                  "easetype", curtainEasetype));
		}
		else if(state == false && guiBools.displayRightHalf == false)
		{
			guiBools.displayRightHalf = true;
			var newPosition = new Vector3(RightCoverBeginPos.x + 17, 
			                              RightCoverBeginPos.y, 
			                              RightCoverBeginPos.z);
			iTween.MoveTo(guiElements.RightCover, iTween.Hash("position", newPosition, 
			                                                  "time", curtainSpeed, 
			                                                  "easetype", curtainEasetype));
		}
	}

	public void BlockAll(bool state){
		BlockLeftHalf(state);
		BlockRightHalf(state);
	}
	//-----------------------------------------------


	public void ExitConfirmation()
	{
		if(!gameOver){
			NotificationCenter.DefaultCenter().PostNotification(this, "NC_Pause");

			if(guiBools.displayExitConfirmation == false)
				guiBools.displayExitConfirmation = true;
		}
	}

	public string GetStage()
	{
		return currentStage;
	}

	#endregion

	
	#region Class Methods
	private int GetCenterWidth()
	{
		return Screen.width/2;
	}
	
	private int getCenterHeight()
	{
		return Screen.height/2;
	}
	
	private bool PlaceButton(string text)
	{
		bool state = true;
		//TODO: Implement proper guistyle through guiStyles.WindowStyle or new style
		//if(GUILayout.Button(text, GUILayout.MinHeight(40)))
		if(GUILayout.Button(text, guiStyles.WindowStyle, GUILayout.MinHeight(Screen.height*0.15f), GUILayout.MinWidth(Screen.width*0.2f)))
			return (true);
		else
			return (false);
	}

	private void NC_GameOver()
	{
		gameOver = true;
	}

	private void NC_Restart()
	{
		gameOver = false;
	}
	private void NC_Pause()
	{
		BlockAll(true);
	}
	private void NC_Unpause()
	{
		BlockAll(false);
	}

	#endregion

	#region Subclasses
	[System.Serializable]
	public class GUIElements
	{
		public GameObject Highscore;
		public GameObject LeftCover;
		public GameObject RightCover;
	}

	[System.Serializable]
	public class GUIBools
	{
		public bool displayUserSelection = true;
		public bool displayHighscore = false;
		public bool displayLeftHalf = false;
		public bool displayRightHalf = false;
		public bool displayStatistics = false;
		public bool displayPlayPrompt = false;
		public bool displayExitConfirmation = false;
		public bool displayCountDown = false;
		public bool displayStageSelection = false;
		public bool displayEndScreen = false;
	}

	[System.Serializable]
	public class GUIStyles
	{
		public GUIStyle WindowStyle;
		public GUIStyle WindowFrameStyle;
		public GUIStyle WindowLabelStyle;
		public GUIStyle WindowScoreLabelStyle;
		public GUIStyle HighscoreStyle;
		public GUIStyle CountdownStyle;
	}
	#endregion
}
