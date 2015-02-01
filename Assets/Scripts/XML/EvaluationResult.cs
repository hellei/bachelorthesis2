using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRUserInterface;

public enum DisplayType {TableDisplay, PlayerCenteredDisplay, PlayerCenteredDisplayFixedAngle, PCDText}

public class EvaluationResult : XMLSaveAndLoad<EvaluationResult> {


	[System.Serializable]
	public class ButtonTest
	{
		public ButtonType bt;
		public float[] selectionTimes;
		public float averageTime;
		public float averageTimeOwnCards, averageTimeOpponentCards;

		/// <summary>
		/// Every time you select the wrong card, this counter increases by 1.
		/// </summary>
		public int falseSelects;
		
		/// <summary>
		/// Everytime the player looks at a button and then looks away again.
		/// </summary>
		public int abortedSelects;

	}

	/// <summary>
	/// Defines whether the selected cards were on the hand or not.
	/// </summary>
	public bool onHand;

	public ButtonTest[] tests;

	public DisplayTest[] displayTests;

	[System.Serializable]
	public class DisplayTest
	{
		public DisplayType dt;
		public float[] selectionTimes;
		public float averageTime = 0;

		public void CalculateAverage()
		{
			foreach (float t in selectionTimes)
			{
				averageTime += t;
			}
			averageTime /= (float)selectionTimes.Length;
		}
	}
	
	public void Calculate(int i)
	{
		tests[i].averageTime = 0;
		tests[i].averageTimeOwnCards = 0;
		tests[i].averageTimeOpponentCards = 0;
		int numberOfOwnCards = 0;
		int numberOfOpponentCards = 0;
		int idx = 0;
		foreach (float val in tests[i].selectionTimes)
		{
			tests[i].averageTime += val;
			if (idx % 2 == 0){
				tests[i].averageTimeOwnCards += val;
				numberOfOwnCards++;
			}
			else
			{
				tests[i].averageTimeOpponentCards += val;
				numberOfOpponentCards++;
			}
			idx++;
		}
		tests[i].averageTime /= (float)tests[i].selectionTimes.Length;
		tests[i].averageTimeOwnCards /= (float)numberOfOwnCards;
		tests[i].averageTimeOpponentCards /= (float)numberOfOpponentCards;
		Debug.Log ("Average time is " + tests[i].averageTime);
	}
}

