using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Hand_Table : MonoBehaviour {

    //public float handOffsetY;
	
	// Update is called once per frame
	void Update () {      
        //Calculate position
        Dictionary<int, TouchInfo> tTouches = Tablet.instance.GetTouches();
        if (tTouches != null && tTouches.Count > 0)
        {
			SetHandEnabled(true);
           

            Vector3 pos = Vector3.zero;
            foreach (TouchInfo ttouch in tTouches.Values)
            {
                pos += (Vector3)(ttouch.worldPosition);
            }
            pos /= tTouches.Count;

            //Apply position with hand y offset
            transform.position = new Vector3(pos.x, pos.y, pos.z);
        }
        else
        {
			SetHandEnabled(false);
        }
	}

	void SetHandEnabled(bool value)
	{
		if (!value) transform.position = new Vector3(0,-100,0);
	}
}
