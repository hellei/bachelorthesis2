using UnityEngine;
using System.Collections;

public class ScaleObject : MonoBehaviour {

    private Vector3 startScale;

    public float scaleChangeSpeed = 2.0f;


    public KeyCode scaleDown = KeyCode.UpArrow, scaleUp = KeyCode.DownArrow, reset = KeyCode.Backspace;

	// Use this for initialization
	void Start () {
        startScale = transform.localScale;
	}

    private float currentScale = 1.0f;
	
	// Update is called once per frame
	void Update () {
        HandleInput();
	    //Adapt size
        transform.localScale = startScale * currentScale;
	}

    void HandleInput()
    {
#if !UNITY_EDITOR
		float lastScale = currentScale;
#endif
        //Scale down
        if (Input.GetKey(scaleDown))
        {
            currentScale -= scaleChangeSpeed * Time.deltaTime;
        }
        //Scale up
        else if (Input.GetKey(scaleUp))
        {
            currentScale += scaleChangeSpeed * Time.deltaTime;
        }
        //Reset to initial scale
        else if (Input.GetKey(reset))
        {
            currentScale = 1;
        }
#if !UNITY_EDITOR
		if (currentScale != lastScale) Debug.Log(currentScale);
#endif
    }
}
