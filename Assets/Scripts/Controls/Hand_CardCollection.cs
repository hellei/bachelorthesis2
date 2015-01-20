using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRUserInterface;


public class Hand_CardCollection : MonoBehaviour {


    // Tracking attributes
    public GameObject CardBucket;
    public GameObject EmptyHandCollider;
    public Finger_Permanent[] fingers;

    // Main attributes
    public int maxCardsAllowed = -1;
    public Hand_Permanent hand;
    private List<Card> cardsOnHand = new List<Card>();
    private bool handRegistered = false;
    private int numberOfCardsOnHand = 0;
    private int selectedCard;

    public static Hand_CardCollection instance;

    void Awake()
    {
        instance = this;        
    }

	// Use this for initialization
	void Start () {
	}

    /// <summary>
    /// Register a Hand when spawned
    /// </summary>
    /// <param name="hand">Hand to be registered as hand that is holding the cards</param>
    public Hand_Permanent RegisterHand() 
    {
        DisplayCardsOnHand();
        handRegistered = true;

        return hand;
        //print("Registered and show cards");
    }

    public void UnRegisterHand()
    {
        HideCardsOnHand();
        handRegistered = false;
        //print("Unregistered and hide cards");
    }

    public Finger_Permanent[] GetFingers()
    {
        return fingers;
    }

    public bool IsHandRegistered()
    {
        return handRegistered;
    }

    public int FindIndexOfNearestCard(Vector3 position)
    {
        int index = 0;
        for (int i = 0; i < cardsOnHand.Count; i++)
        {
            if ((cardsOnHand[i].gameObject.transform.position - position).sqrMagnitude < (cardsOnHand[index].gameObject.transform.position - position).sqrMagnitude)
            {
                index = i;
            }
        }
        return index;
    }

    public float GetDistanceToCard(int i, Vector3 position)
    {
        return (cardsOnHand[i].transform.position - position).sqrMagnitude;
    }

    public void AddCardToHand(Card card)
    {
		if (!card)
		{
			Debug.LogWarning("Cannot add null card to hand!");
			return;
		}
		card.CardState = CardState.OnHand;
        //Add new
        int indexOfNearestCard = FindIndexOfNearestCard(card.transform.position);
        card.transform.parent = null;

        // Insert at nearest position
        if (cardsOnHand.Count > 0)
        {
            // Transform card's position in space of nearest card
            Transform nearestCardSpace = cardsOnHand[indexOfNearestCard].transform;
            Vector3 relativePosToNearestCard = nearestCardSpace.InverseTransformPoint(card.transform.position);

            // Right
            if (relativePosToNearestCard.x > 0)
            {
                //At end of list
                if (indexOfNearestCard + 1 == cardsOnHand.Count)
                {
                    cardsOnHand.Add(card);
                }
                else
                {
                    cardsOnHand.Insert(indexOfNearestCard++, card);
                }
            }
            // Left
            else
            {
                cardsOnHand.Insert(indexOfNearestCard, card);
            }
        }
        else
        // First card on hand
        {
            cardsOnHand.Add(card);
        }
        
        //cardsOnHand.Add(card);
        card.gameObject.layer = (int)InteractionLayer.HandCollision;
        numberOfCardsOnHand++;

        // Disable hand collider if card on hand
        if( numberOfCardsOnHand > 0 && handRegistered)
        {
            EmptyHandCollider.layer = (int)InteractionLayer.Interactable;
            EmptyHandCollider.collider.enabled = false;
        }

        //Reset cards
        if (handRegistered)
        {
            HideCardsOnHand();
            DisplayCardsOnHand();
        }
    }

    /*public Card TakeCardFromHand(int index)
    {
        if (numberOfCardsOnHand > 0 && index < cardsOnHand.Count && index >= 0)
        {
            // Remove card from hand
            Card card = cardsOnHand[index];
            Transform parent = card.transform.parent;
            cardsOnHand.RemoveAt(index);
            numberOfCardsOnHand--;
            
            print("Took card from left hand");

            // Enable hand collider if no card on hand
            if (numberOfCardsOnHand <= 0 && handRegistered)
            {
                EmptyHandCollider.layer = (int)InteractionLayer.HandCollision;
                EmptyHandCollider.collider.enabled = true;
            }

            //Reset parameter
            card.transform.parent = null;
            card.transform.localPosition = Vector3.zero;
            card.transform.localRotation = Quaternion.Euler(Vector3.zero);

            Destroy(parent.gameObject);
            return card;
        }
        else return null;
    }*/

    public Card TakeCardFromHand(Card card)
    {

        if (numberOfCardsOnHand > 0)
        {
            // Remove card
            int index = cardsOnHand.IndexOf(card);
            Transform parent = card.transform.parent;
            cardsOnHand.RemoveAt(index);
            numberOfCardsOnHand--;
            

            print("Took card from left hand");

            // Reset parameter
            card.transform.parent = null;
            card.transform.localPosition = Vector3.zero;
            card.transform.localRotation = Quaternion.Euler(Vector3.zero);


            //Reset cards
            if (handRegistered)
            {
                HideCardsOnHand();
                DisplayCardsOnHand();
            }

            Destroy(parent.gameObject);
            return card;
        }
        else return null;
    }
	/// <summary>
	/// The y offset of the palm at which the cards are shown on the an	/// </summary>
	public float cardHandOffset = 0.05f;

	/// <summary>
	/// The local position of the card container.
	/// </summary>
	public Vector3 cardContainerLocalPosition = new Vector3(-0.0147f,0,0.0069f);

    private void DisplayCardsOnHand()
    {
        if (cardsOnHand.Count > 0)
        {
            float angle = 90.0f / cardsOnHand.Count;
            for (int i = 0; i < cardsOnHand.Count; i++)
            {
                GameObject container = new GameObject("CardOffsetHelper");

                cardsOnHand[i].transform.parent = container.transform;
                cardsOnHand[i].transform.localPosition = new Vector3(0, cardHandOffset, i * 0.001f);
                cardsOnHand[i].transform.localRotation = Quaternion.Euler(new Vector3(0,0,180));

				cardsOnHand[i].GetComponent<LookingGlassEffect>().initialLocalPosition = cardsOnHand[i].transform.localPosition;

                container.transform.parent = CardBucket.transform;
				container.transform.localRotation = Quaternion.identity;
				container.transform.localPosition = cardContainerLocalPosition;
                container.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 150 + i * angle));


                cardsOnHand[i].gameObject.SetActive(true);                
            }
        }
    }
    

    private void HideCardsOnHand()
    {
        for (int i = 0; i < numberOfCardsOnHand; i++)
        {
            Transform container = cardsOnHand[i].gameObject.transform.parent;            
            cardsOnHand[i].transform.parent = this.transform;
            cardsOnHand[i].gameObject.SetActive(false);

            if (container != null && container.gameObject.name == "CardOffsetHelper")
            {
                Destroy(container.gameObject);  
            }
        }
    }
}
