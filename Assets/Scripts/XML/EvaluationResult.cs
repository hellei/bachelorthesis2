using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRUserInterface;

public class EvaluationResult : XMLSaveAndLoad<EvaluationResult> {


	[System.Serializable]
	public class ButtonTest
	{
		public ButtonType bt;
		public float[] selectionTimes;
		public float averageTime;
	}

	public ButtonTest[] tests;


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
