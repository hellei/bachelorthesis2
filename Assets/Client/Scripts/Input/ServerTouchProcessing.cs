using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerTouchProcessing : MonoBehaviour {

    // used to get the touched objects
    private Ray touchray;
    //private Camera tableCamera;

    // saves information for all active touches
    private Dictionary<int, TouchInfo> activeTouches = new Dictionary<int, TouchInfo>();

    // saves information for all active objects
    private Dictionary<GameObject, List<int>> activeObjects = new Dictionary<GameObject, List<int>>();

    private Vector2 touchObjectOffset;

    private bool optionTouchThrough = false;

    private List<MyTouch> Touches = new List<MyTouch>();

    List<GameObject> tmpRemoveList = new List<GameObject>();
    List<int> tmpTouchRemoveList = new List<int>();
    private List<GameObject> hitObjectList = new List<GameObject>();

    public static ServerTouchProcessing instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //tableCamera = camera;
    }

    void Update()
    {
        // Touch detected on Display or old touches are still processed
        if (Touches.Count > 0 || activeTouches.Count > 0) ProcessTouches();
    }

    public Camera GetTableCamera()
    {
        return camera;
    }

    public Dictionary<int,TouchInfo> GetNormalizedTouchData()
    {
        /*List<MyTouch> touchlist = new List<MyTouch>();
        foreach (MyTouch t in Touches)
        {
            MyTouch myTouch = new MyTouch();
            myTouch.deltaPosition = new Vector2(t.deltaPosition.x / Screen.width, t.deltaPosition.y / Screen.width);
            myTouch.position = new Vector2(t.position.x / Screen.width, t.position.y / Screen.height);
            touchlist.Add(myTouch);
        }
        return touchlist;*/

        return activeTouches;
    }    

   /* List<MyTouch> DeNormalizeTouchData(List<MyTouch> touchList)
    {
        foreach (MyTouch t in touchList)
        {
            t.deltaPosition = new Vector2(t.deltaPosition.x * Screen.width, t.deltaPosition.y * Screen.height);
            t.position = new Vector2(t.position.x * Screen.width, t.position.y * Screen.height);
        }

        return touchList;
    }*/

    // called by Update when Touches are detected
    void ProcessTouches()
    {
        UpdateActiveTouches();
        updateHitObjectInformations();
        CallHandleTouches();
        CleanUp();
    }

    public void UpdateTouchData(List<MyTouch> touchList)
    {
        Touches = touchList;//DeNormalizeTouchData(touchList);
    }

    /// <summary>Saves starting information of each new touch and updates old ones.</summary>
    void UpdateActiveTouches()
    {
        // reset all alive values in activeTouches
        foreach (TouchInfo ti in activeTouches.Values)
            ti.alive = false;

        // update touches and add new ones
        foreach (MyTouch t in Touches)
        {
            if (t.phase == TouchPhase.Began || !activeTouches.ContainsKey(t.fingerId)) // add new touches
            {
                t.phase = TouchPhase.Began;
                if (!activeTouches.ContainsKey(t.fingerId))
                {
                    activeTouches.Add(t.fingerId, new TouchInfo(t));
                }
                
            }
            else // update existing touches
                activeTouches[t.fingerId].update(t);
        }

        // end each touch that has not been updated (touch was not detected anymore and EndPhase was skipped)
        foreach (TouchInfo ti in activeTouches.Values)
        {
            if (!ti.alive)
            {
                ti.phase = TouchPhase.Ended;
                ti.alive = true;
            }
        }
    }

	bool TouchRay(Vector3 origin, Vector3 direction, out RaycastHit hit)
	{
		int layerMask = 1 << 10;
		bool result = Physics.Raycast (origin, direction, out hit, 1000f, layerMask);
		return result;
	}

	RaycastHit[] TouchRayAll(Vector3 origin, Vector3 direction)
	{
		int layerMask = 1 << 10;
		RaycastHit[] hits = Physics.RaycastAll (origin, direction, 1000f, layerMask);
		return hits;
	}

    /// <summary>Updates activeObjects list and hitObjects per touch in activeTouches.</summary>
    void updateHitObjectInformations()
    {
        //if there are no new touches, skip this function
        if (Touches.Count == 0) return;

        //If there is exactly 1 touch and 1 active object, no update of active objects is done to keep the reference to the 1 active object (e.g. to avoid losing the object when dragging it fast)
        if (!(activeTouches.Count == 1 && activeTouches[Touches[0].fingerId].phase != TouchPhase.Began && activeObjects.Count == 1 && !optionTouchThrough))
        {
            // clear active objects list
            activeObjects.Clear();

            // save object offsets of each touch and add each touch to the corresponding touched objects
            foreach (TouchInfo ti in activeTouches.Values)
            {
                // clear list of all hit objects
                hitObjectList.Clear();

                Vector3 origin = Tablet.instance.TabletToWorldSpace(ti.currentPosition);
                origin.y += 0.3f;
                touchray = new Ray(origin,Vector3.down);// camera.ScreenPointToRay(ti.currentPosition);

                //If Touch Through option is off, only the first hit object of exactly 1 touch is returned. If it is on or there are more touches detected, all hit objects are returned
                RaycastHit[] hitObjects;
                if (!optionTouchThrough && Touches.Count == 1)
                {
                    // saves the first hit object in hitObjects[0]
                    RaycastHit tmphit;
                    if (TouchRay(touchray.origin, touchray.direction, out tmphit))
                    {
                        hitObjects = new RaycastHit[1];
                        hitObjects[0] = tmphit;
                    }
                    else
                        hitObjects = TouchRayAll(touchray.origin, touchray.direction); //TODO when is this reached??
                }
                else
				{
                    hitObjects = TouchRayAll(touchray.origin, touchray.direction); // returns all colliders hit by the touchray
				}

                // registers information about hitObjects (and the corresponding touch) in "activeObjects" and "activeTouches"
                foreach (RaycastHit RaycastHitObject in hitObjects)
                {
                    GameObject hitObject = RaycastHitObject.collider.gameObject;

                    // add the hitObject to the "hitObjectList"
                    hitObjectList.Add(hitObject);

                    // calculates the offset (in screenSpace) of the object center towards the touch point
                    Vector2 tmpPos = Tablet.instance.WorldToTabletSpace(hitObject.transform.position);//camera.WorldToScreenPoint(hitObject.transform.position);
                    touchObjectOffset = new Vector2(ti.currentPosition.x - tmpPos.x, ti.currentPosition.y - tmpPos.y);
                    //touchObjectOffset = new Vector2(ti.currentPosition.x - hitObject.transform.position.x, ti.currentPosition.y - hitObject.transform.position.y);

                    // add the hit object to the startObjects of the touch if the touch is new
                    if (ti.phase == TouchPhase.Began) ti.startObjects.Add(hitObject);

                    // adds objectOffset of the hitObject to the TouchInfo of the current touch if not already available
                    if (!ti.objectOffsets.ContainsKey(hitObject))
                        ti.objectOffsets.Add(hitObject, touchObjectOffset);

                    // adds hitObject to the touchedObjects dictionary for the current touch if not already available
                    if (!ti.touchedObjects.ContainsKey(hitObject))
                        ti.touchedObjects.Add(hitObject, new List<Gesture>());

                    // adds Object to the activeObjects list if not listed yet
                    if (!activeObjects.ContainsKey(hitObject)) activeObjects.Add(hitObject, new List<int>());

                    // adds current fingerId to the hitObject entry in "activeObjects"
                    if (!activeObjects[hitObject].Contains(ti.id)) activeObjects[hitObject].Add(ti.id);
                }

                // remove Offsets of Objects (in activeTouches) that are not hit by the touch (but were hit in previous frame)
                List<GameObject> tmp = new List<GameObject>();
                foreach (GameObject obj in ti.objectOffsets.Keys)
                {
                    if (!hitObjectList.Contains(obj))
                        tmp.Add(obj);
                }
                foreach (GameObject obj in tmp)
                {
                    ti.objectOffsets.Remove(obj);
                }
            }
        }
    }

    /// <summary>Calls "handleTouches" of each active object with reference to all touches on the corresponding object.</summary>
    void CallHandleTouches()
    {
        foreach (GameObject obj in activeObjects.Keys)
        {
            List<TouchInfo> tmp = new List<TouchInfo>();
            foreach (int id in activeObjects[obj])
			{
                tmp.Add(activeTouches[id]);
			}
			if (obj.GetComponent<Interactable>())
			{
            	obj.GetComponent<Interactable>().handleTouches(tmp);//obj.SendMessage("handleTouches", tmp);
			}
		}
    }

    /// <summary>Remove ending touches and objects that are not touched anymore or should not be existent anymore.</summary>
    void CleanUp()
    {
        // mark ending touches in activeTouches. Mark objects that won't have a corresponding touch anymore.
        foreach (TouchInfo ti in activeTouches.Values)
        {
            if (ti.phase == TouchPhase.Ended)
            {
                tmpTouchRemoveList.Add(ti.id);
                foreach (GameObject obj in activeObjects.Keys)
                {
                    activeObjects[obj].Remove(ti.id);
                    if (activeObjects[obj].Count == 0)
                        tmpRemoveList.Add(obj);
                }
            }
        }

        // remove touches that have ended
        foreach (int i in tmpTouchRemoveList)
            activeTouches.Remove(i);

        // clear the list of ended touches
        tmpTouchRemoveList.Clear();

        // remove objects that are not touched anymore
        foreach (GameObject obj in tmpRemoveList)
            activeObjects.Remove(obj);

        // clear the list of objects that should be destroyed at the end of each Update 
        tmpRemoveList.Clear();
    }
}
