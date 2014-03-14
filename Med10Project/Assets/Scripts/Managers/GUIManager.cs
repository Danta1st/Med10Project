using UnityEngine;
using System.Collections;

public class GUIManager: MonoBehaviour {
	
	#region Editor Publics
	public GUIStyle guiStyle;
	#endregion
	
	#region Privates
	private bool displayHighscore = true;
	private bool displayLeftHalf = true;
	private bool displayRightHalf = true;
	private bool displayStatistics = false;
	private Rect highscoreRect;

	private HighscoreManager scoreManager;
	#endregion
	
	#region Public Methods

	#endregion

	void Start()
	{
		//Connectivity
		scoreManager = GameObject.Find("HighscoreManager").GetComponent<HighscoreManager>();

		//Class general
		highscoreRect = new Rect(0, 0, 100, 80);
		highscoreRect.center = new Vector2(GetCenterWidth(), 20);
	}

	void OnGUI()
	{
		if(displayHighscore == true)
			GUI.Label(highscoreRect, ""+scoreManager.GetScore(), guiStyle);
	}

	private int GetCenterWidth()
	{
		return Screen.width/2;
	}

	private int getHeightWidth()
	{
		return Screen.height/2;
	}
}
