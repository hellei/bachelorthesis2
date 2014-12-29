using UnityEngine;
using System.Collections;

public class HandSelectingRegisterScript : MonoBehaviour {

    public GameObject index;
    public GameObject middle;
    public GameObject thumb;
    public GameObject grabbedCardContainer;

	// Use this for initialization
	void Start () {

        Hand_Selecting.instance.RegisterHand(index, middle, thumb, grabbedCardContainer);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDestroy()
    {
        Hand_Selecting.instance.UnRegisterHand();
    }
}
