using UnityEngine;
using System.Collections;

public class GUIManager: MonoBehaviour {
	
	#region Editor Publics
	[SerializeField] private string[] Users = {"User 1", "User 2", "User 3", "User 4", "User 5", "User 6", "User 7", "User 8", "User 9", "User 10"};
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
	private EndGameLines endGameLines;

	//Rects
	private Rect highscoreRect;
	private Rect windowRect;
	private Rect endWindowRect;
	private Rect countdownRect;
	private Rect userSelectRect;

	//Conditioning
	private GUIBools guiBools = new GUIBools();

	//Variables
	private Vector3 LeftCoverBeginPos;
	private Vector3 RightCoverBeginPos;

	//Strings
	private string currentUser;
	private int CurrentUserID = -1;
	private string currentCountdownNumber;
	private string currentStage;

	private int hits;
	private int misses;
	private string avgReactionTime;

	private bool gameOver = false;
	private bool showingReactionTimes = false;
	#endregion

	void Start()
	{
//		PlayerPrefs.DeleteAll();
		//Connectivity
		scoreManager = GameObject.Find("HighscoreManager").GetComponent<HighscoreManager>();
		endGameLines = GameObject.Find("EndGameLines").GetComponent<EndGameLines>();
		txtLogger = GameObject.Find("WriteToTXT").GetComponent<WriteToTXT>();
		//Rect initialization
		highscoreRect = new Rect(0, 0, 100, 80);
		highscoreRect.center = new Vector2(GetCenterWidth(), 20);
		windowRect = new Rect(0,0, Screen.width * windowMetrics.x, Screen.height * windowMetrics.y);
		windowRect.center = new Vector2(GetCenterWidth(), getCenterHeight());

		userSelectRect = new Rect(0,0, Screen.width * windowMetrics.x, Screen.height * (Users.Length*0.095f));
		userSelectRect.center = new Vector2(GetCenterWidth(), getCenterHeight());

		endWindowRect = new Rect(0,0, Screen.width * 0.52f, Screen.height * 0.88f);
		endWindowRect.center = new Vector2(GetCenterWidth(), getCenterHeight());
		countdownRect = new Rect(0,0,150,150);
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
			userSelectRect = GUILayout.Window(0, userSelectRect, DoUserSelectionWindow, "", guiStyles.WindowFrameStyle);
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
			endWindowRect = GUILayout.Window(0, endWindowRect, DoEndScreenWindow, "", guiStyles.WindowEndFrameStyle);
		}

	}

	#region GUI Windows
	private void DoUserSelectionWindow(int windowID)
	{
		GUILayout.FlexibleSpace();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("V\u00E6lg Bruger", guiStyles.WindowLabelStyle);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();

		int counter = 0;
		int coloumns = 2;

		int rows = Users.Length/2;

		if(Users.Length % 2 == 0)
		{
		}
		else{
			rows++;
		}

		for(int i = 0; i < rows; i++)
		{
			GUILayout.BeginHorizontal();
			for(int j = 0; j < coloumns; j++)
			{
				if(PlaceButton(Users[counter], 0.1f, 0.1f))
				{
					guiBools.displayUserSelection = false;
					guiBools.displayPlayPrompt = true;
					//guiBools.displayStageSelection = true;
					currentUser = Users[counter];
					CurrentUserID = counter;
				}

				counter++;

				if(counter == Users.Length)
				{
					break;
				}


				if(j == 0)
				{
					GUILayout.Space(5);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(5);
		}

		//selectionGrid = GUILayout.SelectionGrid(selectionGrid, Users, 2, guiStyles.WindowStyle);

		if(PlaceButton("Afslut spillet"))
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
		GUILayout.Label("Forts\u00E6t eller afslut?", guiStyles.WindowLabelStyle);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();

		if(!gameOver)
		{
			if(PlaceButton("Forts\u00E6t"))
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

		if(PlaceButton("Tilbage Til Menuen"))
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
		GUILayout.Label("Hej "+currentUser, guiStyles.WindowLabelStyle);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.Space(10);

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("Er du klar?", guiStyles.WindowLabelStyle);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();

		if(PlaceButton("Spil"))
		{
			guiBools.displayPlayPrompt = false;
			txtLogger.UpdateSessionID(CurrentUserID);
//			EnableHighscore(true);
			StartCoroutine(StartCountDown());
		}

		GUILayout.Space(5);

		if(PlaceButton("Fortryd"))
		{
			guiBools.displayPlayPrompt = false;
			guiBools.displayUserSelection = true;
		}

	}

	private void DoEndScreenWindow(int windowID)
	{
		GUILayout.BeginHorizontal();

		if(!showingReactionTimes)
		{
			if(PlaceSmallToggleButton("Tr\u00E6ffere og Missere", guiStyles.SmallButtonOn))
			{
				endGameLines.DisableEndScreen();
				endGameLines.DoHitMissScreen();
				showingReactionTimes = false;
			}
		}
		else if(showingReactionTimes)
		{
			if(PlaceSmallToggleButton("Tr\u00E6ffere og Missere", guiStyles.SmallButton))
			{
				endGameLines.DisableEndScreen();
				endGameLines.DoHitMissScreen();
				showingReactionTimes = false;
			}
		}

		GUILayout.Space(5);

		if(!showingReactionTimes)
		{
			if(PlaceSmallToggleButton("Reaktionstid", guiStyles.SmallButton))
			{
				endGameLines.DisableEndScreen();
				endGameLines.DoReactionScreen();
				showingReactionTimes = true;
			}
		}
		else if(showingReactionTimes)
		{
			if(PlaceSmallToggleButton("Reaktionstid", guiStyles.SmallButtonOn))
			{
				endGameLines.DisableEndScreen();
				endGameLines.DoReactionScreen();
				showingReactionTimes = true;
			}
		}
		GUILayout.EndHorizontal();

		GUILayout.Space(15);

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		if(!showingReactionTimes)
		{
			GUILayout.Label("Tr\u00E6ffere og Missere per Vinkel", guiStyles.WindowLabelStyle);
		}
		else if(showingReactionTimes)
		{
			GUILayout.Label("Reaktionstid per Vinkel", guiStyles.WindowLabelStyle);
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.Space(15);

		if(!showingReactionTimes)
		{
			GUILayout.Label("      Tr\u00E6ffere: "+hits, guiStyles.WindowScoreLabelStyle);
			
			GUILayout.Space(5);
			
			GUILayout.Label("      Missere: "+ misses, guiStyles.WindowScoreLabelStyle);
		}
		else if(showingReactionTimes)
		{
			GUILayout.Label("   Gennemsnit:", guiStyles.WindowScoreLabelStyle);
			GUILayout.Label("   "+ avgReactionTime, guiStyles.WindowScoreLabelStyle);
		}

		GUILayout.FlexibleSpace();

		GUILayout.BeginHorizontal();
		if(PlaceButton("Tilbage Til Menuen"))
		{
			EnableEndScreen(false);
			guiBools.displayUserSelection = true;
			BlockAll(true);
			EnableHighscore(false);
			NotificationCenter.DefaultCenter().PostNotification(this, "NC_Restart");
		}
		GUILayout.Space(5);
		if(PlaceButton("Afslut Spillet"))
		{
			Application.Quit();
		}
		GUILayout.EndHorizontal();
	}

	private IEnumerator StartCountDown()
	{
		guiBools.displayCountDown = true;

		//Slide the countdown to the right side of the screen over 1.5 seconds
		iTween.ValueTo(gameObject, iTween.Hash("from", countdownRect.x, "to", countdownRect.x + 150, "time", 1.5f, 
		                                       "onupdate", "UpdateCountdownRect"));
		currentCountdownNumber = "3";
		yield return new WaitForSeconds(1.0f);
		currentCountdownNumber = "2";
		yield return new WaitForSeconds(1.0f);
		currentCountdownNumber = "1";
		yield return new WaitForSeconds(1.0f);
		currentCountdownNumber = "";
		guiBools.displayCountDown = false;
		BlockRightHalf(false);
		NotificationCenter.DefaultCenter().PostNotification(this, "NC_Play");
	}

	private void UpdateCountdownRect(float itweenChange)
	{
		countdownRect.x = itweenChange;
	}

	#endregion


	#region Public Methods
	public int GetUserID()
	{
		return CurrentUserID;
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
			hits = scoreManager.GetHitCount();
			misses = scoreManager.GetMissCount();
			avgReactionTime = scoreManager.GetReactionMeanFloat()+"s";
			BlockAll(true);
			StartCoroutine(EndGameDelayedScreen(true));		
		}
		else if (state == false)
		{
			BlockAll(false);
			StartCoroutine(EndGameDelayedScreen(false));
		}
	}

	private IEnumerator EndGameDelayedScreen(bool state)
	{
		if(state == true)
		{
			yield return new WaitForSeconds(1.0f);
			guiBools.displayEndScreen = true;
			endGameLines.DoHitMissScreen();
		}
		else if (state == false)
		{
			yield return new WaitForSeconds(0.0f);
			guiBools.displayEndScreen = false;
			endGameLines.DisableEndScreen();
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
			var newPosition = new Vector3(LeftCoverBeginPos.x - 18.0f, 
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
			var newPosition = new Vector3(RightCoverBeginPos.x + 18.0f, 
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
		if(GUILayout.Button(text, guiStyles.WindowStyle, GUILayout.MinHeight(Screen.height*0.15f), GUILayout.MinWidth(Screen.width*0.15f)))
			return (true);
		else
			return (false);
	}
	private bool PlaceButton(string text, float heightPercentage, float widthPercentage)
	{
		if(GUILayout.Button(text, guiStyles.WindowStyle, GUILayout.MinHeight(Screen.height*heightPercentage), GUILayout.MinWidth(Screen.width*widthPercentage)))
			return (true);
		else
			return (false);
	}

	private bool PlaceSmallToggleButton(string text, GUIStyle CurrentStyle)
	{
		if(GUILayout.Button(text, CurrentStyle, GUILayout.MinHeight(Screen.height*0.08f), GUILayout.MinWidth(Screen.width*0.15f)))
			return (true);
		else
			return (false);
	}

	private IEnumerator EndGame()
	{
		NotificationCenter.DefaultCenter().PostNotification(this, "NC_Pause");
		gameOver = true;
		BlockAll(true);
		yield return new WaitForSeconds(curtainSpeed);
		EnableEndScreen(true);
	}

	private void NC_GameOver()
	{
		StartCoroutine_Auto(EndGame());
	}

	private void NC_Restart()
	{
		countdownRect.center = new Vector2(GetCenterWidth(), getCenterHeight());
		gameOver = false;
		showingReactionTimes = false;
	}
	private void NC_Pause()
	{
		//Blocking here gives problems in phase2 since unpausing will remove both walls regardless of which stage.
		//BlockAll(true);
	}
	private void NC_Unpause()
	{
		//BlockAll(false);
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
		public GUIStyle WindowEndFrameStyle;
		public GUIStyle WindowLabelStyle;
		public GUIStyle WindowScoreLabelStyle;
		public GUIStyle HighscoreStyle;
		public GUIStyle CountdownStyle;
		public GUIStyle SmallButton;
		public GUIStyle SmallButtonOn;
	}
	#endregion
}
