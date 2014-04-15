using UnityEngine;
using System.Collections;

public class HighscoreManager : MonoBehaviour {

	#region Editor Publics
	#endregion

	#region Privates
	private int currentScore = 0;
	private int multiplier = 1;
	private int hits = 0;
	private int misses = 0;

	void Start()
	{
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Restart");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Play");
	}

	private void NC_Restart()
	{
		ResetScoreAndMultiplier();
	}

	private void NC_Play()
	{
		ResetScoreAndMultiplier();
	}
	
	#endregion

	#region Public Methods
	public int GetScore()
	{
		return currentScore;
	}

	public int GetMultiplier()
	{
		return multiplier;
	}
	
	public int GetHits()
	{
		return hits;
	}
	
	public int GetMisses()
	{
		return misses;
	}

	public void AddScore(int value, bool useMultiplier)
	{
		if(useMultiplier == true)
			currentScore += value * multiplier;
		else
			currentScore += value;

	}

	public void AddHit()
	{
		hits += 1;
		Debug.Log ("Hits: "+hits);
	}

	public void AddMiss()
	{
		misses += 1;
		Debug.Log ("Misses: "+misses);
	}

	public void SubtractScore(int value)
	{
		if(currentScore - value >= 0)
			currentScore -= value;
		else
			currentScore = 0;
	}

	public void IncreaseMultiplier()
	{
		multiplier++;
	}
	public void IncreaseMultiplier(int value)
	{
		multiplier += value;
	}

	public void DecreaseMultiplier()
	{
		if(multiplier - 1 >= 1)
			multiplier -= 1;

	}
	public void DecreaseMultiplier(int value)
	{
		if(multiplier - value >= 1)
			multiplier -= value;
	}

	public void ResetScore()
	{
		currentScore = 0;
	}

	public void ResetScoreAndMultiplier()
	{
		ResetScore();
		ResetMultiplier();
		ResetHits();
		ResetMisses();
	}

	public void ResetMultiplier()
	{
		multiplier = 1;
	}

	public void ResetHits()
	{
		hits = 0;
	}
	
	public void ResetMisses()
	{
		misses = 0;
	}
	#endregion

	#region Class Methods
	#endregion
}
