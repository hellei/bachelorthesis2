using UnityEngine;
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

		float max = 0;
		foreach (EvaluationResult er in results)
		{
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
					cardIdx++;
				}
				mediumResult.tests[id].abortedSelects += test.abortedSelects;
				mediumResult.tests[id].falseSelects += test.falseSelects;
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
			mediumResult.Calculate(i);
		}
		mediumResult.Save(path+"result.xml");
		ConvertSingleFile (path + "result.xml", path);
	}
}