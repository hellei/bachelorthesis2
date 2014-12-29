using UnityEngine;
using System.Collections;

public class Hand_Selecting : MonoBehaviour {
    
    // Tracking attributes
    private GameObject index;
    private GameObject middle;
    private GameObject thumb;
    private GameObject grabbedCardContainer;

    // Main attributes
    public int maxAllowedCardsInHand = 1;
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
            return new Vector3((this.index.transform.position.x + this.middle.transform.position.x + this.thumb.transform.position.x) / 3.0f,
                                                          (this.index.transform.position.y + this.middle.transform.position.y + this.thumb.transform.position.y) / 3.0f,
                                                          (this.index.transform.position.z + this.middle.transform.position.z + this.thumb.transform.position.z) / 3.0f);        
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
    public void RegisterHand(GameObject index, GameObject middle, GameObject thumb, GameObject grabbedCard)
    {         
        this.index = index;
        this.middle = middle;
        this.thumb = thumb;
        grabbedCardContainer = grabbedCard;
        DisplayCardInHand();
        handRegistered = true;
        //print("Registered and show selected card");
    }

    public void UnRegisterHand()
    {
        index = null;
        middle = null;
        thumb = null;
        grabbedCardContainer = null;
        HideCardInHand();
        handRegistered = false;
        //print("Unregistered and hide selected card");
    }

    public void AddCardToHand(Card card)
    {
        cardsInHand++;
        card.transform.parent = this.transform;
        card.gameObject.layer = (int) InteractionLayer.Interactable;
        grabbedCard = card;

        Debug.Log("Added card to right hand: " + card + " " + card.transform.parent);

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
        }
    }

    private void HideCardInHand()
    {
        if (grabbedCard != null)
        {
            grabbedCard.transform.parent = this.transform;
            grabbedCard.gameObject.SetActive(false);
            Debug.Log("Hide right hand card" + " " + grabbedCard.transform.parent);
        }
    }
}
