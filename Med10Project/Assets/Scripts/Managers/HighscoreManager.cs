using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HighscoreManager : MonoBehaviour {

	#region Editor Publics
	[SerializeField] private int calibrationSkipAmount = 3;
	#endregion

	#region Privates
	private int currentScore = 0;
	private int multiplier = 1;
	private int skipCounter = 0;

	//Reactiontime Lists
	private int angleCount = 10;
	private List<List<float>> rtList = new List<List<float>>();
	private List<List<float>> hitDistances = new List<List<float>>();
	private List<List<float>> missDistances = new List<List<float>>();
	private List<int> angleHits = new List<int>();
	private List<int> angleMisses = new List<int>();

	#endregion

	void Start()
	{
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Restart");
		NotificationCenter.DefaultCenter().AddObserver(this, "NC_Play");

		InitialiseLists(angleCount);
	}

	#region Public Methods	
	//Statistics------------------------------------------------------------------------------
	
	public void AddHit(int angleID, float _reactionTime, float _distance)
	{
		if(skipCounter >= calibrationSkipAmount)
		{
			//Increase count for hits
			angleHits[angleID - 1]++; // = angleHits[angleID - 1] + 1;
			//Add reaction time to angle list
			AddReactionTime(angleID, _reactionTime);
			//Add distance to hitDistance list
			AddHitDistance(angleID, _distance);
		}
		else
			skipCounter++;
	}
	
	public void AddMiss(int angleID, float _distance)
	{
		//Increase count for misses
		angleMisses[angleID - 1]++;
		//Add distance to missDistance list
		AddMissDistance(angleID, _distance);
	}

	//Method returning the total hit count
	public int GetHitCount()
	{
		int count = 0;

		foreach(int value in angleHits)
			count += value;

		return count;
	}

	//Method returning the hit count for a specific angleID
	public int GetHitCount(int angleID)
	{
		return angleHits[angleID - 1];
	}

	//Method returning the total miss count
	public int GetMissCount()
	{
		int count = 0;
		
		foreach(int value in angleMisses)
			count += value;
		
		return count;
	}

	//Method returning the miss count for a specific angleID
	public int GetMissCount(int angleID)
	{
		return angleMisses[angleID - 1];
	}

	public List<float> GetHitDistances(int angleID)
	{
		return hitDistances[angleID - 1];
	}

	public List<float> GetMissDistances(int angleID)
	{
		return missDistances[angleID - 1];
	}
		
	//Method returning a list containing all mean reaction times
	public List<float> GetAllReactionTimes()
	{
		List<float> reactionMeans = new List<float>();
		
		for(int i = 1; i <= angleCount; i++)
		{
			if(GetReactionMean(i) != null)
			{
				reactionMeans.Add(GetReactionMean(i));
			}
			else
			{
				reactionMeans.Add(0);
			}
		}
		
		return reactionMeans;
	}

	//Method returning the mean reaction of a specific angle
	public float GetReactionMean(int angleID)
	{
		int index = angleID - 1;
		
		float sum = 0;
		int count = 0;
		
		foreach(var value in rtList[index])
		{
			if(value != 0)
			{
				sum += value;
				count++;
			}
		}
		
		float mean = sum/(float) count;
		
		return mean;
	}
	
	//Method returning the mean reaction time based on all hits & latehits
	public float GetReactionMean()
	{
		float sum = 0;
		int count = 0;
		//Crawl through lists
		foreach (var sublist in rtList)
		{
			foreach (var value in sublist)
			{
				if(value != 0)
				{
					sum += value;
					count++;
				}
			}
		}
		//Calculate Mean Value
		float mean = sum/(float) count;
		Debug.Log("sum: "+sum+" count: "+count+" HS RT MEAN: "+mean);
		return mean;
	}

	//Method returning the mean reaction time as a float
	public string GetReactionMeanFloat()
	{
		float avgReactiontime = GetReactionMean();
		
		string tempAvgReactString;
		
		if(avgReactiontime < 1)
		{
			tempAvgReactString = "0"+avgReactiontime.ToString("#.000");
		}
		else{
			tempAvgReactString = avgReactiontime.ToString("#.000");
		}
		
		return tempAvgReactString;
	}

	//Score Stuff-------------------------------------------------------------------------------
	public int GetScore()
	{
		return currentScore;
	}
	
	public int GetMultiplier()
	{
		return multiplier;
	}

	public void AddScore(int value, bool useMultiplier)
	{
		if(useMultiplier == true)
			currentScore += value * multiplier;
		else
			currentScore += value;
		
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
	public void ResetMultiplier()
	{
		multiplier = 1;
	}
	public void ResetScoreAndMultiplier()
	{
		skipCounter = 0;
		ResetScore();
		ResetMultiplier();
		ResetLists();
	}
	#endregion


	#region Class Methods
	private void InitialiseLists(int amountOfLists)
	{
		for(int i = 1; i <= amountOfLists; i++)
		{
			//Add amountOfLists to Reaction Time Lists, hitDistances, & missDistances
			List<float> sublist = new List<float>();
			List<float> sublist2 = new List<float>();
			List<float> sublist3 = new List<float>();
			rtList.Add(sublist);
			hitDistances.Add(sublist2);
			missDistances.Add(sublist3);
			//Add amountOfLists entries to hits
			angleHits.Add(0);
			//Add amountOfLists entries to misses
			angleMisses.Add(0);
		}

	}

	private void AddReactionTime(int angleID, float reactionTime)
	{
		int index = angleID - 1;

		rtList[index].Add(reactionTime);
	}
	private void AddHitDistance(int angleID, float distance)
	{
		int index = angleID - 1;
		
		hitDistances[index].Add(distance);
	}
	private void AddMissDistance(int angleID, float distance)
	{
		int index = angleID - 1;
		
		missDistances[index].Add(distance);
	}

	private void ResetLists()
	{
		rtList.Clear();
		hitDistances.Clear();
		missDistances.Clear();
		angleHits.Clear();
		angleMisses.Clear();
		InitialiseLists(angleCount);
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
}
