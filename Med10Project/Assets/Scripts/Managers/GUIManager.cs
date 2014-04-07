using UnityEngine;
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
	#endregion

	void Start()
	{
		//Connectivity
		scoreManager = GameObject.Find("HighscoreManager").GetComponent<HighscoreManager>();

		//Rect initialization
		highscoreRect = new Rect(0, 0, 100, 80);
		highscoreRect.center = new Vector2(GetCenterWidth(), 20);
		windowRect = new Rect(0,0, Screen.width * windowMetrics.x, Screen.height * windowMetrics.y);
		windowRect.center = new Vector2(GetCenterWidth(), getCenterHeight());
		countdownRect = new Rect(0,0,400,200);
		countdownRect.center = new Vector2(GetCenterWidth(), getCenterHeight());

		LeftCoverBeginPos = guiElements.LeftCover.transform.position;
		RightCoverBeginPos = guiElements.RightCover.transform.position;
	}
	
	//TODO: Implement proper guistyles
	void OnGUI()
	{
		if(guiBools.displayUserSelection == true)
		{
			windowRect = GUILayout.Window(0, windowRect, DoUserSelectionWindow, "User Selection");
		}

		if(guiBools.displayHighscore == true)
		{			
			GUI.Label(highscoreRect, ""+scoreManager.GetScore(), guiLook);
		}

		if(guiBools.displayExitConfirmation == true)
		{
			windowRect = GUILayout.Window(0, windowRect, DoExitConfirmationWindow, "Want to quit?");
		}

		if(guiBools.displayPlayPrompt)
		{
			windowRect = GUILayout.Window(0, windowRect, DoPlayPromptWindow, "Confirm User Selection");
		}

		if(guiBools.displayCountDown)
		{
			GUI.Label(countdownRect, ""+currentCountdownNumber, guiStyles.CountdownStyle);
		}
	}

	#region GUI Windows
	private void DoUserSelectionWindow(int windowID)
	{
		//TODO: Implement proper user selection with the logging system
		if(PlaceButton("User 1"))
		{
			guiBools.displayUserSelection = false;
			guiBools.displayPlayPrompt = true;
			currentUser = "User 1";
		}

		if(PlaceButton("User 2"))
		{
			guiBools.displayUserSelection = false;
			guiBools.displayPlayPrompt = true;
			currentUser = "User 2";
		}
	}

	private void DoExitConfirmationWindow(int windowID)
	{
		if(PlaceButton("Yes"))
		{
			//TODO: Pause Game
			//TODO: Save, send, log Data
			Application.Quit();
		}

		if(PlaceButton("No"))
		{
			//TODO: Unpause game (probably counter to begin)
			guiBools.displayExitConfirmation = false;
			NotificationCenter.DefaultCenter().PostNotification(this, "NC_Unpause");
		}

		GUILayout.FlexibleSpace();

		if(PlaceButton("Back To User Selection"))
		{
			guiBools.displayExitConfirmation = false;
			guiBools.displayUserSelection = true;
			BlockAll(true);
			//TODO: Needs to reset highscore
			EnableHighscore(false);
			NotificationCenter.DefaultCenter().PostNotification(this, "NC_Restart");
		}
	}

	private void DoPlayPromptWindow(int windowID)
	{
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label(currentUser + " selected.");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		if(PlaceButton("Play"))
		{
			guiBools.displayPlayPrompt = false;
//			EnableHighscore(true);
			StartCoroutine(StartCountDown());
		}

		GUILayout.FlexibleSpace();

		if(PlaceButton("Cancel"))
		{
			guiBools.displayPlayPrompt = false;
			guiBools.displayUserSelection = true;
		}

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
		NotificationCenter.DefaultCenter().PostNotification(this, "NC_Pause");
		if(guiBools.displayExitConfirmation == false)
			guiBools.displayExitConfirmation = true;
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
		if(GUILayout.Button(text, GUILayout.MinHeight(40)))
			return (true);
		else
			return (false);
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
	}

	[System.Serializable]
	public class GUIStyles
	{
		public GUIStyle WindowStyle;
		public GUIStyle HighscoreStyle;
		public GUIStyle CountdownStyle;
	}
	#endregion
}
