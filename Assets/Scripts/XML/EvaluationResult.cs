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
		foreach (float val in tests[i].selectionTimes)
		{
			tests[i].averageTime += val;
		}
		tests[i].averageTime /= (float)tests[i].selectionTimes.Length;
		Debug.Log ("Average time is " + tests[i].averageTime);
	}
}

