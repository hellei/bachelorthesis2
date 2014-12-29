using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SharedPrefab : MonoBehaviour {

    private int touchesCount;    
    private List<MyTouch> Touches = new List<MyTouch>();
    private List<MyTouch> touchlist = new List<MyTouch>();

    void Start()
    {
        if (Network.isClient)
        {
            BecomeOwner();
        }

        Debug.Log("Instantiated shared Prefab");
    }

    void Update()
    {
        if (Network.isClient)
        {
            Touches = NormalizeTouchData(ClientTouchProcessing.instance.GetTouches());
            touchesCount = Touches.Count;
        }
    }

    List<MyTouch> NormalizeTouchData(List<MyTouch> touchList)
    {
        foreach (MyTouch t in touchList)
        {
            t.deltaPosition = new Vector2(t.deltaPosition.x / Screen.width, t.deltaPosition.y / Screen.height);
            t.position = new Vector2(t.position.x / Screen.width, t.position.y / Screen.height);
        }

        return touchList;
    }

    void OnNetworkInstantiate(NetworkMessageInfo info)
    {
        Debug.Log(networkView.viewID + " spawned");
        if (Network.isClient)
        {
            Unity_Client.instance.syncPrefab = gameObject;
        }
    }

    void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        int tCount = 0;

        // Touch properties
        Vector3 tDeltaPosition = Vector3.zero;
        float tDeltaTime = 0;
        int tFingerId = 0;
        int tTouchPhase = 0;
        Vector3 tPosition = Vector3.zero;
        int tTapCount = 0;

        if (stream.isWriting && Network.isClient)
        {
            // Serialize number of touches
            tCount = touchesCount;
            stream.Serialize(ref tCount);

            // Initialize touch properties
            for (int i = 0; i < tCount; i++)
            {
                tDeltaPosition = Touches[i].deltaPosition;
                tDeltaTime = Touches[i].deltaTime;
                tFingerId = Touches[i].fingerId;
                switch (Touches[i].phase)
                {
                    case TouchPhase.Began:
                        tTouchPhase = 0;
                        break;
                    case TouchPhase.Canceled:
                        tTouchPhase = 1;
                        break;
                    case TouchPhase.Ended:
                        tTouchPhase = 2;
                        break;
                    case TouchPhase.Moved:
                        tTouchPhase = 3;
                        break;
                    case TouchPhase.Stationary:
                        tTouchPhase = 4;
                        break;
                    default: tTouchPhase = 0;
                        break;
                }
                tPosition = Touches[i].position;
                tTapCount = Touches[i].tapCount;

                // Serialize touch properties
                stream.Serialize(ref tDeltaPosition);
                stream.Serialize(ref tDeltaTime);
                stream.Serialize(ref tFingerId);
                stream.Serialize(ref tTouchPhase);
                stream.Serialize(ref tPosition);
                stream.Serialize(ref tTapCount);
            }
        }
        if (stream.isReading && Network.isServer)
        {
            stream.Serialize(ref tCount);
            touchesCount = tCount;

            touchlist.Clear();

            // Initialize touch properties
            for (int i = 0; i < tCount; i++)
            {
                MyTouch myTouch = new MyTouch();

                // Serialize touch properties
                stream.Serialize(ref tDeltaPosition);
                stream.Serialize(ref tDeltaTime);
                stream.Serialize(ref tFingerId);
                stream.Serialize(ref tTouchPhase);
                stream.Serialize(ref tPosition);
                stream.Serialize(ref tTapCount);

                // Store serialized data
                myTouch.deltaPosition = tDeltaPosition;
                myTouch.deltaTime = tDeltaTime;
                myTouch.fingerId = tFingerId;
                switch (tTouchPhase)
                {
                    case 0:
                        myTouch.phase = TouchPhase.Began;
                        break;
                    case 1:
                        myTouch.phase = TouchPhase.Canceled;
                        break;
                    case 2:
                        myTouch.phase = TouchPhase.Ended;
                        break;
                    case 3:
                        myTouch.phase = TouchPhase.Moved;
                        break;
                    case 4:
                        myTouch.phase = TouchPhase.Stationary;
                        break;
                    default: myTouch.phase = TouchPhase.Began;
                        break;
                }
                myTouch.position = tPosition;
                myTouch.tapCount = tTapCount;

                //Add to touch list
                touchlist.Add(myTouch);
            }

            //Update Touches
            ServerTouchProcessing.instance.UpdateTouchData(touchlist);
        }
    }

    void BecomeOwner()
    {
        NetworkView objView = this.gameObject.GetComponent<NetworkView>();
        var newObjID = Network.AllocateViewID();
        networkView.RPC("ChangeOwner", RPCMode.Others, newObjID);
        objView.viewID = newObjID;
    }

    [RPC]
    void ChangeOwner(NetworkViewID objID)
    {
        var objView = this.gameObject.GetComponent<NetworkView>();
        objView.viewID = objID;
    }

    void OnDestroy()
    {
        Debug.Log("Destroyed shared Prefab");
    }
}
