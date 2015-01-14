using UnityEngine;
using System.Collections;

public class HandCollectionRegisterScript : MonoBehaviour {    

    public RiggedHand hand;
    public RiggedFinger[] fingers = new RiggedFinger[5];

	// Use this for initialization
	void Start () {       


            Hand_Permanent permanentHand = Hand_CardCollection.instance.RegisterHand();
            hand.InitPermanentHand(permanentHand);
            for (int i = 0; i < fingers.Length; i++)
            {
                fingers[i].InitPermanentFinger(Hand_CardCollection.instance.GetFingers()[i]);
            }
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDestroy()
    {
        Hand_CardCollection.instance.UnRegisterHand();
    }
}
