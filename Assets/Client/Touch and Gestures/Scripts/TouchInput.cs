using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchInfo
{
    public float startTime;
    public Vector2 startPosition;
    public List<GameObject> startObjects; //a list of touched objects when the touch was first recognized
    public Vector2 currentPosition;
    public Vector3 worldPosition;
    public int id;
    public TouchPhase phase;
    public Dictionary<GameObject, Vector2> objectOffsets;
    public bool alive;
    public Dictionary<GameObject, List<Gesture>> touchedObjects; // a dictionary that saves each triggered gesture of each touched object in the lifetime of the touch

    public TouchInfo(MyTouch t)
    {
        startPosition = t.position;
        startTime = Time.time;
        startObjects = new List<GameObject>();
        currentPosition = t.position;
        id = t.fingerId;
        phase = t.phase;
        objectOffsets = new Dictionary<GameObject, Vector2>();
        alive = true;
        touchedObjects = new Dictionary<GameObject, List<Gesture>>();
    }

    public void update(MyTouch t)
    {
        currentPosition = t.position;
        phase = t.phase;
        alive = true;
    }
}

[System.Serializable]
public class MyTouch
{
    public Vector2 deltaPosition { get; set; }
    public float deltaTime { get; set; }
    public int fingerId { get; set; }
    public TouchPhase phase { get; set; }
    public Vector2 position { get; set; }
    public int tapCount { get; set; }    

    public MyTouch() { }
    public MyTouch(Touch t)
    {
        deltaPosition = t.deltaPosition;
        deltaTime = t.deltaTime;
        fingerId = t.fingerId;
        phase = t.phase;
        position = t.position;
        tapCount = t.tapCount;
    }
}

public static class TouchInput
{
    /// <summary>Returns all touches detected on the screen.</summary>
    public static List<MyTouch> GetTouchInput()
    {
        List<MyTouch> TouchInputList = new List<MyTouch>();
#if UNITY_STANDALONE_WIN
        //The WindowsTouches script needs to be placed on the MainCamera and the NameOfApplicationWindow has to be entered in the inspector of the WindowsTouches script
        TouchInputList.AddRange(Camera.main.GetComponent<WindowsTouches>().GetWindowsTouches()); 
#endif

#if UNITY_ANDROID
        foreach (Touch t in Input.touches)
            TouchInputList.Add(new MyTouch(t));
#endif
        return TouchInputList;
    }
}
