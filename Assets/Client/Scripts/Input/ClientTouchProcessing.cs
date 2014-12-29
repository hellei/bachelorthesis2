using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClientTouchProcessing : MonoBehaviour {

    private List<MyTouch> Touches = new List<MyTouch>();
    private string touchData;
    public static ClientTouchProcessing instance;

    void Awake()
    {
        instance = this;
        Debug.Log("instance client");
    }

	// Use this for initialization
	void Start () {

        // Rescaling
        //Camera.main.orthographicSize = Screen.height * 0.005f;	
	}
	
	// Update is called once per frame
	void Update () {

        //unifies Touch Information from different Platforms into a "MyTouch" list
        Touches = TouchInput.GetTouchInput();
	}

    public List<MyTouch> GetTouches()
    {
        return Touches;
    } 
}
