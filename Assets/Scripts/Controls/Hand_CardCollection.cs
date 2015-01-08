using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRUserInterface;


public class Hand_CardCollection : MonoBehaviour {

    public Card card;
    public Card card1;
    public Card card2;
    public Card card3;

    // Tracking attributes
    private GameObject CardBucket;
    private GameObject EmptyHandCollider;

    // Main attributes
    public int maxCardsAllowed = -1;
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

        AddCardToHand(card);
        AddCardToHand(card1);
        AddCardToHand(card2);
        AddCardToHand(card3);
        //card.gameObject.SetActive(false); 
	}

    /// <summary>
    /// Register a Hand when spawned
    /// </summary>
    /// <param name="hand">Hand to be registered as hand that is holding the cards</param>
    public void RegisterHand(GameObject cardBucket, GameObject emptyHandCollider) 
    {
        this.CardBucket = cardBucket;
        this.EmptyHandCollider = emptyHandCollider;
        DisplayCardsOnHand();
        handRegistered = true;
        //print("Registered and show cards");
    }

    public void UnRegisterHand()
    {
        CardBucket = null;
        EmptyHandCollider = null;
        HideCardsOnHand();
        handRegistered = false;
        //print("Unregistered and hide cards");
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
            // Right
            if (card.transform.position.x > cardsOnHand[indexOfNearestCard].transform.position.x)
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

    public Card TakeCardFromHand(int index)
    {
        if (numberOfCardsOnHand > 0 && index < cardsOnHand.Count && index >= 0)
        {
            // Remove card from hand
            Card card = cardsOnHand[index];
            Transform parent = card.transform.parent;
            cardsOnHand.RemoveAt(index);
            numberOfCardsOnHand--;
            Destroy(parent.gameObject);
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

            return card;
        }
        else return null;
    }

    public Card TakeCardFromHand(Card card)
    {

        if (numberOfCardsOnHand > 0)
        {
            // Remove card
            int index = cardsOnHand.IndexOf(card);
            Transform parent = card.transform.parent;
            cardsOnHand.RemoveAt(index);
            numberOfCardsOnHand--;
            Destroy(parent.gameObject);

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

            return card;
        }

        else return null;

    }

    private void DisplayCardsOnHand()
    {
        if (cardsOnHand.Count > 0)
        {
            float angle = 90.0f / cardsOnHand.Count;
            for (int i = 0; i < cardsOnHand.Count; i++)
            {
                GameObject container = new GameObject("CardOffsetHelper");

                cardsOnHand[i].transform.parent = container.transform;
                cardsOnHand[i].transform.localPosition = new Vector3(0, 0.1f, i * 0.001f);
                cardsOnHand[i].transform.localRotation = Quaternion.Euler(new Vector3(0,0,180));

                container.transform.parent = CardBucket.transform;
                container.transform.localPosition = Vector3.zero;
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
