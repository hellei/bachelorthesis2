using UnityEngine;
using System.Collections;

public class Horse : MonoBehaviour {

	public float durationTillChange = 20f;
	public string[] animations;


	float timeElapsed = 0;
	// Use this for initialization
	void Start () {
	
	}
	string anim = "";
	// Update is called once per frame
	void Update () {
		timeElapsed += Time.deltaTime;
		if (timeElapsed > durationTillChange || ((anim.Contains("rear_up") || anim.Contains("sniff")) && timeElapsed > 2.0f))
		{
			timeElapsed = 0;
			anim = animations[Random.Range(0, animations.Length)];
			animation.CrossFade (anim);
		}
	}
}
