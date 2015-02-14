﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using VRUserInterface;

public class XMLConverter : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	string path = "path";

	void OnGUI()
	{
		path = GUI.TextField (new Rect (0, 0, 300, 100), path);
		if (GUI.Button(new Rect(0,100,300,100),"Convert"))
		{
			ConvertSingleFile(path);
		}
		if (GUI.Button(new Rect(0,200,300,100),"Convert all"))
		{
			ConvertListOfFiles();
		}
	}

	void ConvertSingleFile(string file, string directory ="")
	{
		List<string> lines = new List<string> ();
		lines.Add("Button Type;Card Id;Cards;");
		EvaluationResult er = EvaluationResult.Load(file);
		foreach (EvaluationResult.ButtonTest t in er.tests)
		{
			int id = 1;
			foreach (float val in t.selectionTimes)
			{
				lines.Add (t.bt+";"+id+";"+val+";");
				id++;
			}
		}

		foreach (EvaluationResult.DisplayTest t in er.displayTests)
		{
			int id = 1;
			foreach (float val in t.selectionTimes)
			{
				lines.Add (t.dt+";"+id+";"+val+";");
				id++;
			}
		}

		if (directory == ""){
			File.WriteAllLines ("new.txt", lines.ToArray());
		}
		else {
			File.WriteAllLines(directory+"results.txt",lines.ToArray());
		}
	}

	public float threshold = 10f;

	void ConvertListOfFiles()
	{
		string[] groups = new string[4];
		List<EvaluationResult> results = new List<EvaluationResult> ();
		foreach (string file in Directory.GetFiles(path))
		{
			results.Add (EvaluationResult.Load(file));
			Debug.Log(file);
		}
		EvaluationResult mediumResult = new EvaluationResult ();
		mediumResult.tests = new EvaluationResult.ButtonTest[4];
		mediumResult.tests [0] = new EvaluationResult.ButtonTest ();
		mediumResult.tests [0].bt = ButtonType.Timer;
		mediumResult.tests [0].selectionTimes = new float[10];
		mediumResult.tests [1] = new EvaluationResult.ButtonTest ();
		mediumResult.tests [1].bt = ButtonType.ThreeDots;
		mediumResult.tests [1].selectionTimes = new float[10];
		mediumResult.tests [2] = new EvaluationResult.ButtonTest ();
		mediumResult.tests [2].bt = ButtonType.Arrows;
		mediumResult.tests [2].selectionTimes = new float[10];
		mediumResult.tests [3] = new EvaluationResult.ButtonTest ();
		mediumResult.tests [3].bt = ButtonType.DragCircle;
		mediumResult.tests [3].selectionTimes = new float[10];

		mediumResult.displayTests = new EvaluationResult.DisplayTest[4];
		mediumResult.displayTests [0] = new EvaluationResult.DisplayTest ();
		mediumResult.displayTests [0].dt = DisplayType.TableDisplay;
		mediumResult.displayTests [0].selectionTimes = new float[5];
		mediumResult.displayTests [1] = new EvaluationResult.DisplayTest ();
		mediumResult.displayTests [1].dt = DisplayType.PlayerCenteredDisplay;
		mediumResult.displayTests [1].selectionTimes = new float[5];
		mediumResult.displayTests [2] = new EvaluationResult.DisplayTest ();
		mediumResult.displayTests [2].dt = DisplayType.PlayerCenteredDisplayFixedAngle;
		mediumResult.displayTests [2].selectionTimes = new float[5];
		mediumResult.displayTests [3] = new EvaluationResult.DisplayTest ();
		mediumResult.displayTests [3].dt = DisplayType.PCDText;
		mediumResult.displayTests [3].selectionTimes = new float[5];

		float max = 0;
		foreach (EvaluationResult er in results)
		{
			if (er.tests != null){
				foreach (EvaluationResult.ButtonTest test in er.tests)
				{

					int id = 0;
					switch (test.bt)
					{
						case ButtonType.Timer:
							id = 0;
							break;
						case ButtonType.ThreeDots:
							id = 1;
							break;
						case ButtonType.Arrows:
							id = 2;
							break;
						case ButtonType.DragCircle:
							id = 3;
							break;
					}
					int cardIdx = 0;
					foreach (float val in test.selectionTimes)
					{
						if (val > max) max = val;
						mediumResult.tests[id].selectionTimes[cardIdx] += Mathf.Min (val, threshold);
						groups[id]+=Mathf.Min (val, threshold)+System.Environment.NewLine;
						cardIdx++;
					}
					mediumResult.tests[id].abortedSelects += test.abortedSelects;
					mediumResult.tests[id].falseSelects += test.falseSelects;
				}
			}


			if (er.displayTests != null){
				foreach (EvaluationResult.DisplayTest test in er.displayTests)
				{
					
					int id = 0;
					switch (test.dt)
					{
					case DisplayType.TableDisplay:
						id = 0;
						break;
					case DisplayType.PlayerCenteredDisplay:
						id = 1;
						break;
					case DisplayType.PlayerCenteredDisplayFixedAngle:
						id = 2;
						break;
					case DisplayType.PCDText:
						id = 3;
						break;
					}
					int cardIdx = 0;
					foreach (float val in test.selectionTimes)
					{
						mediumResult.displayTests[id].selectionTimes[cardIdx] += val;
						cardIdx++;
					}
				}
			}
		}
		Debug.Log (max);
		for (int i = 0; i < 4; i++)
		{
			for (int i2 = 0; i2 < 10; i2++)
			{
				mediumResult.tests[i].selectionTimes[i2]/=results.Count;
			}
			mediumResult.tests[i].abortedSelects /= results.Count;
			mediumResult.tests[i].falseSelects /= results.Count;

			for (int i2 = 0; i2 < 5; i2++)
			{
				mediumResult.displayTests[i].selectionTimes[i2]/=results.Count;
			}
			mediumResult.displayTests[i].CalculateAverage();
			mediumResult.Calculate(i);
		}
		File.WriteAllText (path + "group1.txt", groups [0]);
		File.WriteAllText (path + "group2.txt", groups [1]);
		File.WriteAllText (path + "group3.txt", groups [2]);
		File.WriteAllText (path + "group4.txt", groups [3]);
		mediumResult.Save(path+"result.xml");
		ConvertSingleFile (path + "result.xml", path);
	}
}