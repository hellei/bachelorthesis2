using UnityEngine;
using System.Collections;
using VRUserInterface;

public class Hand_Selecting : MonoBehaviour {
    
    // Tracking attributes
    public Hand_Permanent hand;
    public GameObject index;
    public GameObject middle;
    public GameObject thumb;
    public GameObject grabbedCardContainer;
    public Finger_Permanent[] fingers;

    // Main attributes
    public int maxAllowedCardsInHand = 1;
    public Vector3 inactiveHandPos = new Vector3(0, -1000,0);
    private Vector3  grabPosition;
    private Card grabbedCard;
    private int cardsInHand;
    private bool handRegistered;


    public static Hand_Selecting instance;

	// Use this for initialization
    void Awake()
    {
        instance = this;
    }
	
	// Update is called once per frame
    void Update()
    {
        if (handRegistered)
        {
            grabPosition = UpdateGrabPosition();            
        }
    }

    private Vector3 UpdateGrabPosition()
    {        
            return new Vector3((index.transform.position.x + middle.transform.position.x + thumb.transform.position.x) / 3.0f,
                                                          (index.transform.position.y + middle.transform.position.y + thumb.transform.position.y) / 3.0f,
                                                          (index.transform.position.z + middle.transform.position.z + thumb.transform.position.z) / 3.0f);        
    }

    public Finger_Permanent[] GetFingers()
    {
        return fingers;
    }

    public void SetHandPosition(Vector3 pos)
    {
        hand.palm.transform.position = pos;
    }

    public Vector3 GetGrabPosition()
    {
        return grabPosition;
    }

    public int GetNumCardsInHand()
    {
        return cardsInHand;
    }

    public int GetNumAllowedCardsInHand()
    {
        return maxAllowedCardsInHand;
    }

    public Card GetCardInHand()
    {
        return grabbedCard;
    }

    public bool IsHandRegistered()
    {
        return handRegistered;
    }

    /// <summary>
    /// Register selecting Hand when spawned
    /// </summary>
    /// <param name="hand">Hand to be registered as hand that is selecting single cards</param>
    public Hand_Permanent RegisterHand()
    {
        DisplayCardInHand();
        handRegistered = true;

        return hand;
        //print("Registered and show selected card");
    }

    public void UnRegisterHand()
    {
        HideCardInHand();
        handRegistered = false;
        SetHandPosition(inactiveHandPos);
        //print("Unregistered and hide selected card");
    }

    public void AddCardToHand(Card card)
    {
        cardsInHand++;
        card.transform.parent = this.transform;
		card.transform.localPosition = Vector3.zero;
        card.CardState = CardState.OnHand;
        card.stack = null;
        card.gameObject.layer = (int) InteractionLayer.Interactable;
        grabbedCard = card;
        Debug.Log("Added card to right hand: " + card + " " + card.transform.parent);

		//grabbedCard.gameObject.SetActive (false);

        HideCardInHand();

        if (handRegistered)
        {            
            DisplayCardInHand();
        }
    }

    public Card TakeCardFromHand()
    {
        Card card = grabbedCard;

        grabbedCard = null;
        cardsInHand--;

        card.transform.parent = null;
        return card;
        //print("Put in hand!");
    }

    private void DisplayCardInHand()
    {
        Debug.Log("Display right hand card but card null");
        if (grabbedCard != null)
        {
            grabbedCard.gameObject.SetActive(true);

            grabbedCard.transform.parent = null;
            grabbedCard.transform.localPosition = Vector3.zero;
            grabbedCard.transform.localRotation = Quaternion.Euler(Vector3.zero);

            GameObject container = new GameObject("CardOffsetHelper");
            grabbedCard.transform.parent = container.transform;


            container.transform.parent = grabbedCardContainer.transform;
            container.transform.localPosition = Vector3.zero;
            container.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

            Debug.Log("Display right hand card");

			grabbedCard.GetComponent<LookingGlassEffect>().initialLocalPosition = grabbedCard.transform.localPosition;
        }
    }

    private void HideCardInHand()
    {
        if (grabbedCard != null)
        {
            if (grabbedCard.transform.parent.gameObject.name == "CardOffsetHelper")
            {
                GameObject container = grabbedCard.transform.parent.gameObject;
                grabbedCard.transform.parent = grabbedCardContainer.transform;
                //grabbedCard.gameObject.SetActive(false);
                Destroy(container);
            }
            
            
            Debug.Log("Hide right hand card" + " " + grabbedCard.transform.parent);
        }
    }
}
