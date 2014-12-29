using UnityEngine;
using System.Collections;

public class HandCollectionRegisterScript : MonoBehaviour {

    public GameObject CardBucket;
    public GameObject EmptyHandCollider;

	// Use this for initialization
	void Start () {

        if (CardBucket != null && EmptyHandCollider != null)
        {
            Hand_CardCollection.instance.RegisterHand(CardBucket, EmptyHandCollider);
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
