using UnityEngine;
using System.Collections;

/// <summary>
/// Scales an object with the arrows. Only used for Debug purposes
/// </summary>
public class ScaleObject2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		initialScale = transform.localScale;
	}

	public float speed = 1;

	Vector3 initialScale;
	float factor = 1;
	float lastFactor;
	// Update is called once per frame
	void Update () {
		transform.localScale = factor * initialScale;
		lastFactor = factor;
		factor += Input.GetAxis ("Horizontal") * Time.deltaTime * speed;
		if (factor != lastFactor)
		{
			Debug.Log(factor);
		}
	}
}
