using UnityEngine;
using System.Collections;

public class HandSelectingRegisterScript : MonoBehaviour {

    public RiggedHand hand;
    public RiggedFinger[] fingers = new RiggedFinger[5];

	// Use this for initialization
	void Start () {

        // Initialize update of permanent hand
        Hand_Permanent permanentHand = Hand_Selecting.instance.RegisterHand();
        hand.InitPermanentHand(permanentHand);
        for (int i = 0; i < fingers.Length; i++)
        {
            fingers[i].InitPermanentFinger(Hand_Selecting.instance.GetFingers()[i]);
        }
	}
	
	// Update is called once per frame
	void Update () {

        
	}

    void OnDestroy()
    {
        Hand_Selecting.instance.UnRegisterHand();
    }
}
