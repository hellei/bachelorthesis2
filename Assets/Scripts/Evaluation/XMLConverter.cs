using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

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
			ConvertSingleFile();
		}
	}

	void ConvertSingleFile()
	{
		List<string> lines = new List<string> ();
		lines.Add("Button Type; Card Id;Cards;");
		EvaluationResult er = EvaluationResult.Load(path);
		foreach (EvaluationResult.ButtonTest t in er.tests)
		{
			int id = 1;
			foreach (float val in t.selectionTimes)
			{
				lines.Add (t.bt+";"+id+";"+val+";");
				id++;
			}
		}
	}
}
