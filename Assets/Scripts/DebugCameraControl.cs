using UnityEngine;
using System.Collections;

public class DebugCameraControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	public float speed = 20;
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal") * Time.deltaTime * speed, 0));
		transform.Rotate(new Vector3(Input.GetAxis("Vertical") * Time.deltaTime * speed, 0, 0));
	}
}
