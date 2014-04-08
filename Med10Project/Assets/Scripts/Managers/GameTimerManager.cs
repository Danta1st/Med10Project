using UnityEngine;
using System.Collections;

public class GameTimerManager : MonoBehaviour {

	#region Editor Publics
	[SerializeField] private bool EnableTimer = false;
	[SerializeField] private float maxTimeInMinutes = 0.15f;
	#endregion

	#region Privates
	private float maxTime = 0;
	private float StartTime = 0;
	private float currentTimePlayed = 0;

	private float pauseOffset = 0;
	private float pauseStartTime = 0;
	private float pauseEndTime = 0;

	private bool gameOver = false;
	private bool gameRunning = false;
	#endregion

	// Use this for initialization
	void Start () {
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Play");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Pause");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Unpause");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Restart");
		maxTime = 60*maxTimeInMinutes;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(gameRunning && !gameOver)
		{
			currentTimePlayed = Time.time - StartTime - pauseOffset;
		}

		if(currentTimePlayed > maxTime && !gameOver)
		{
			if(EnableTimer == true)
				OutOfTime();
		}
	}

	public float GetCurrentPlayTime()
	{
		return currentTimePlayed;
	}

	private void OutOfTime()
	{
		Debug.Log("GameOver");
		NotificationCenter.DefaultCenter().PostNotification(this, "NC_Pause");
		gameOver = true;
		gameRunning = false;
	}

	private void StartGameTimer()
	{
		StartTime = Time.time;
		gameRunning = true;
	}
	
	private void PauseGameTimer()
	{
		pauseStartTime = Time.time;
		gameRunning = false;
	}
	
	private void UnpauseGameTimer()
	{
		pauseEndTime = Time.time;
		gameRunning = true;
		pauseOffset += pauseEndTime - pauseStartTime;
	}
	
	private void RestartGameTimer()
	{
		pauseOffset = 0;
		currentTimePlayed = 0;
		gameOver = false;
	}

	private float GetPauseOFfset()
	{
		return pauseOffset;
	}
	
	private void NC_Restart()
	{
		RestartGameTimer();
	}
	
	private void NC_Play()
	{
		StartGameTimer();
	}
	
	private void NC_Pause()
	{
		PauseGameTimer();
	}
	
	private void NC_Unpause()
	{
		UnpauseGameTimer();
	}

	void OnGUI()
	{
		if(gameRunning)
			GUI.Label(new Rect(50, 50, 200, 200), ""+currentTimePlayed.ToString("0"));

		if(gameOver)
			GUI.Label(new Rect(200, 200, 200, 200), "Game Ended");
	}
}
