using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRUserInterface;

public class CardBucket
{
    public GameObject container;
    public Card card;
    public Vector3 localPosition;
    public Quaternion localRotation;
    public bool interpolate = true;
}
public class Hand_CardCollection : MonoBehaviour {


    // Tracking attributes
    public GameObject HandCardContainer;
    public GameObject EmptyHandCollider;
    public Finger_Permanent[] fingers;
    public Hand_Permanent hand;
    private bool handRegistered = false;

    // Main attributes
    public int maxCardsAllowed = -1;
    public float fanOutDistance = 0.05f;
    private List<CardBucket> cardsOnHand = new List<CardBucket>();
    
    private int numberOfCardsOnHand = 0;
    private int selectedCard;

    public static Hand_CardCollection instance;

    /// <summary>
	/// The y offset of the palm at which the cards are shown on the an	/// </summary>
	public float cardHandOffset = 0.05f;

	/// <summary>
	/// The local position of the card container.
	/// </summary>
	public Vector3 cardContainerLocalPosition = new Vector3(-0.0147f,0,0.0069f);
    void Awake()
    {
        instance = this;        
    }

	// Use this for initialization
	void Start () {

        ChangeCardPositionAndOrientation();
	}

    void Update()
    {
        UpdateCards();
        FanOutCards();
    }

    /// <summary>
    /// Register a Hand when spawned
    /// </summary>
    /// <param name="hand">Hand to be registered as hand that is holding the cards</param>
    public Hand_Permanent RegisterHand() 
    {
        ChangeCardPositionAndOrientation();
        handRegistered = true;

        return hand;
        //print("Registered and show cards");
    }

    public void UnRegisterHand()
    {
        //HideCardsOnHand();
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
            if ((cardsOnHand[i].card.transform.position - position).sqrMagnitude < (cardsOnHand[index].card.transform.position - position).sqrMagnitude)
            {
                index = i;
            }
        }
        return index;
    }

    public float GetDistanceToCard(int i, Vector3 position)
    {
        return (cardsOnHand[i].card.transform.position - position).sqrMagnitude;
    }


	float interpolationSpeedP = 3.0f;
	float interpolationSpeedR = 2.0f;

	
    void UpdateCards()
    {
        for (int i = 0; i < cardsOnHand.Count; i++)
        {
            if (cardsOnHand[i].container != null)
            {
				cardsOnHand[i].container.transform.localPosition = Vector3.Lerp(cardsOnHand[i].container.transform.localPosition, cardsOnHand[i].localPosition, Time.deltaTime * interpolationSpeedP);
				cardsOnHand[i].container.transform.localRotation = Quaternion.Slerp(cardsOnHand[i].container.transform.localRotation, cardsOnHand[i].localRotation, Time.deltaTime * interpolationSpeedR);
                cardsOnHand[i].card.transform.localPosition = new Vector3(0, cardHandOffset, i * 0.001f);
            }
        }
    }

    /// <summary>
    /// Adds the card to the left hand
    /// </summary>
    /// <param name="card">the card to add</param>
    /// <param name="cardIndex">the index of the position in hand (starting with 0), -1 for nearest position of right hand</param>
    /// <param name="interpolate">whether a card should be inserted instantly or with interpolation</param>
    public void AddCardToHand(Card card, int cardIndex = -1, bool interpolate = false)
    {
		if (!card)
		{
			Debug.LogWarning("Cannot add null card to hand!");
			return;
		}
        //Add new
        int indexOfNearestCard = FindIndexOfNearestCard(card.transform.position);
        card.transform.parent = null;
        card.CardState = CardState.OnHand;

        // Create card bucket
        CardBucket cardBucket = new CardBucket();
        cardBucket.card = card;
        cardBucket.interpolate = interpolate;

        // Insert at nearest position
        if (cardIndex < 0)
        {
            if (cardsOnHand.Count > 0)
            {

                // Transform card's position in space of nearest card
                Transform nearestCardSpace = cardsOnHand[indexOfNearestCard].card.transform;
                Vector3 relativePosToNearestCard = nearestCardSpace.InverseTransformPoint(card.transform.position);

                // Right
                if (relativePosToNearestCard.x > 0)
                {
                    //At end of list
                    if (indexOfNearestCard + 1 == cardsOnHand.Count)
                    {
                        cardsOnHand.Add(cardBucket);
                    }
                    else
                    {
                        cardsOnHand.Insert(indexOfNearestCard++, cardBucket);
                    }
                }
                // Left
                else
                {
                    cardsOnHand.Insert(indexOfNearestCard, cardBucket);
                }
            }
            else
            //First card on hand
            {
                cardsOnHand.Add(cardBucket);
            }
        }

        // Insert at custom index
        if (cardIndex >= 0 && cardIndex < cardsOnHand.Count)
        {
            cardsOnHand.Insert(cardIndex, cardBucket);
        }
        else if (cardIndex >= cardsOnHand.Count)
        {
            cardsOnHand.Add(cardBucket);
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
        HideCardsOnHand();
        ChangeCardPositionAndOrientation();
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
            // Find index of card
            int index = 0;
            for (int i = 0; i < cardsOnHand.Count; i++)
            {
                if (cardsOnHand[i].card == card)
                {
                    index = i;
                }
            }
            
            Transform parent = card.transform.parent;
            cardsOnHand.RemoveAt(index);
            numberOfCardsOnHand--;
            

            print("Took card from left hand");

            // Reset parameter
            card.transform.parent = null;
            card.transform.localPosition = Vector3.zero;
            card.transform.localRotation = Quaternion.Euler(Vector3.zero);


            //Reset cards
            //if (handRegistered)
            {
                HideCardsOnHand();
                ChangeCardPositionAndOrientation();
            }

            Destroy(parent.gameObject);
            return card;
        }
        else return null;
    }

    private void FanOutCards()
    {
        if (cardsOnHand.Count > 0)
        {
            float angle = 90.0f;
            
            int index = FindIndexOfNearestCard(Hand_Selecting.instance.GetGrabPosition());
            if (GetDistanceToCard(index, Hand_Selecting.instance.GetGrabPosition()) <= fanOutDistance)
            {
                float startAngle = angle * index / cardsOnHand.Count;

                float restAngle = startAngle;
                for (int i = index - 1; i >= 0; i--)
                {
                    // Set new target rotation and position for interpolation
                    //cardsOnHand[i].localPosition = cardContainerLocalPosition;
                    restAngle /= 2.0f;
                    cardsOnHand[i].localRotation = Quaternion.Euler(new Vector3(0, 0, 150 + restAngle));
                }

                restAngle = angle * (cardsOnHand.Count - index) / cardsOnHand.Count;

                for (int i = index + 1; i < cardsOnHand.Count; i++)
                {
                    restAngle /= 2;
                    cardsOnHand[i].localRotation = Quaternion.Euler(new Vector3(0, 0, 150 + angle - restAngle));
                }
            }
        }
    }

	

    private void ChangeCardPositionAndOrientation()
    {
        if (cardsOnHand.Count > 0)
        {
            float angle = 90.0f / cardsOnHand.Count;
            for (int i = 0; i < cardsOnHand.Count; i++)
            {
                // Spawn container at cards position
                GameObject container = new GameObject("CardOffsetHelper");
                container.transform.position = cardsOnHand[i].card.transform.position;

				// Initialize container as child of handcardcontainer
				container.transform.parent = HandCardContainer.transform;
				container.transform.rotation = cardsOnHand[i].card.transform.rotation;
				container.transform.Rotate(new Vector3(0,0,180));

                // Initialize Card in container
                cardsOnHand[i].card.transform.parent = container.transform;
                cardsOnHand[i].card.transform.localPosition = new Vector3(0, cardHandOffset, i * 0.001f);
				cardsOnHand[i].card.transform.localRotation = Quaternion.Euler(new Vector3(0,0,180));

                // Position container with card offset
                Vector3 globalOffset = cardsOnHand[i].card.transform.position - container.transform.position;
                container.transform.position -= globalOffset;

				cardsOnHand[i].card.GetComponent<LookingGlassEffect>().initialLocalPosition = cardsOnHand[i].card.transform.localPosition;

                // Initialize container in Bucket
                cardsOnHand[i].container = container;

                // Set new target rotation and position for interpolation
                if (cardsOnHand[i].interpolate)
                {                    
                    cardsOnHand[i].localPosition = cardContainerLocalPosition;
                    cardsOnHand[i].localRotation = Quaternion.Euler(new Vector3(0, 0, 150 + i * angle));
                }
                // No interpolation
                else
                {
                    container.transform.localPosition = cardContainerLocalPosition;
                    container.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 150 + i * angle));
                    cardsOnHand[i].interpolate = true;                    
                }

                // Activate card
                cardsOnHand[i].card.gameObject.SetActive(true);                
            }
        }
    }
    

    private void HideCardsOnHand()
    {
        for (int i = 0; i < numberOfCardsOnHand; i++)
        {
            Transform container = cardsOnHand[i].card.gameObject.transform.parent;            
            cardsOnHand[i].card.transform.parent = this.transform;
            cardsOnHand[i].card.gameObject.SetActive(false);

            if (container != null && container.gameObject.name == "CardOffsetHelper")
            {
                Destroy(container.gameObject);  
            }
        }
    }
}
