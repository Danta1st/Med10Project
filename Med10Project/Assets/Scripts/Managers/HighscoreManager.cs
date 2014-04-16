using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HighscoreManager : MonoBehaviour {

	#region Editor Publics
	#endregion

	#region Privates
	private int currentScore = 0;
	private int multiplier = 1;
	private int hits = 0;
	private int misses = 0;
	private List<float> reactionTimes = new List<float>();

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

	public string GetAverageStringReactiontime()
	{
		float avgReactiontime = 0;
		float totalReactionTimes = 0;

		for(int i = 0; i < reactionTimes.Count; i++)
		{
			totalReactionTimes += reactionTimes[i];
		}

		avgReactiontime = totalReactionTimes/reactionTimes.Count;

		string tempAvgReactString;

		if(avgReactiontime < 1)
		{
			tempAvgReactString = "0"+avgReactiontime.ToString("#.0");
		}
		else{
			tempAvgReactString = avgReactiontime.ToString("#.0");
		}

		return tempAvgReactString;
	}

	public float GetAverageFloatReactiontime()
	{
		float avgReactiontime = 0;
		float totalReactionTimes = 0;
		
		for(int i = 0; i < reactionTimes.Count; i++)
		{
			totalReactionTimes += reactionTimes[i];
		}
		
		avgReactiontime = totalReactionTimes/reactionTimes.Count;
		
		return avgReactiontime;
	}

	public void AddScore(int value, bool useMultiplier)
	{
		if(useMultiplier == true)
			currentScore += value * multiplier;
		else
			currentScore += value;

	}

	public void AddHit(float _reactionTime)
	{
		reactionTimes.Add(_reactionTime);
		hits += 1;
	}

	public void AddMiss()
	{
		misses += 1;
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
		ResetReactionTimeList();
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

	public void ResetReactionTimeList()
	{
		reactionTimes.Clear();
	}
	#endregion

	#region Class Methods
	#endregion
}
