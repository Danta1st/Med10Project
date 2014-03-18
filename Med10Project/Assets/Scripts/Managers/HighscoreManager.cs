using UnityEngine;
using System.Collections;

public class HighscoreManager : MonoBehaviour {

	#region Editor Publics
	#endregion

	#region Privates
	private int currentScore = 0;
	private int multiplier = 1;
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

	public void ResetMultiplier()
	{
		multiplier = 1;
	}
	#endregion

	#region Class Methods
	#endregion
}
