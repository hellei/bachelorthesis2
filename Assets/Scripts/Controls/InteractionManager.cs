using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TabletGesture
{
    Tap, Move, Pickup, Release, Lift, PlaceOnStack
}

public enum InteractionLayer
{
    Interactable     = 10,
    HandCollision    = 11,
    Emptyhand        = 12
}

public enum HandOrientationMode
{
    Free, RestrictedOrientation, FixedOrientation
}

public enum FingerUpdateMode
{
    Enabled, Disabled
}

public class InteractionManager : MonoBehaviour {

    // Interaction settings
    public float interactionCD = 2.0f;
    public float grabDistance = 0.05f;
    private float interactionTime;
    public bool touchOnTablet = false;
    private Dictionary<int, TouchInfo> tabletTouches = new Dictionary<int, TouchInfo>();

    // Update Modes
    public HandOrientationMode rightHandOrientationMode = HandOrientationMode.RestrictedOrientation;
    public FingerUpdateMode rightHandFingerUpdateMode = FingerUpdateMode.Disabled;
    public FingerUpdateMode leftHandFingerUpdateMode = FingerUpdateMode.Disabled;

    // Instance
    public static InteractionManager instance;

	// Use this for initialization
	void Awake () {

        instance = this;	
	}

    // Update is called once per frame
    void Update()
    {
        HandleHandTabletInteraction();
        SetHandControlModes();
    }

    void SetHandControlModes()
    {
        // Left Hand
        if (Input.GetKeyDown(KeyCode.Y))
        {
            leftHandFingerUpdateMode = FingerUpdateMode.Enabled;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            leftHandFingerUpdateMode = FingerUpdateMode.Disabled;
        }

        // Right Hand
        if (Input.GetKeyDown(KeyCode.C))
        {
            rightHandFingerUpdateMode = FingerUpdateMode.Enabled;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            rightHandFingerUpdateMode = FingerUpdateMode.Disabled;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            rightHandOrientationMode = HandOrientationMode.Free;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            rightHandOrientationMode = HandOrientationMode.RestrictedOrientation;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            rightHandOrientationMode = HandOrientationMode.FixedOrientation;
        }
    }


    void HandleHandTabletInteraction()
    {
        tabletTouches = Tablet.instance.GetTouches();

        //Touched tablet
        if (tabletTouches.Count > 0)
        {
            touchOnTablet = true;

            // Place card on tablet
            if (Hand_Selecting.instance.GetNumCardsInHand() > 0 && Time.time - interactionTime > interactionCD)
            {
                interactionTime = Time.time;

                Vector2 pos = Vector2.zero;
                foreach (TouchInfo tinfo in tabletTouches.Values)
                {
                    pos += tinfo.currentPosition;
                }
                pos /= tabletTouches.Count;

                Card card = Hand_Selecting.instance.TakeCardFromHand();
                Tablet.instance.AddCardToTable(card, pos);
                Debug.Log("put card on tablet");
            }
        }
        else
        {
            touchOnTablet = false;
        }
    }

    public void HandleCardOnTabletInteraction(Card card, List<TouchInfo> touchlist, TabletGesture gesture)
    {

        // Handle Pickup
        if (gesture == TabletGesture.Pickup)
        {
            Debug.Log("Pickup" + card);
            if (Hand_Selecting.instance.GetNumCardsInHand() < Hand_Selecting.instance.GetNumAllowedCardsInHand() || Hand_Selecting.instance.GetNumAllowedCardsInHand() < 0)
            {
                interactionTime = Time.time;

                //Pickup from Table
                if (card.stack == null)
                {
                    Tablet.instance.TakeCardFromTable(card);
                    Hand_Selecting.instance.AddCardToHand(card);
                }

                //Pickup from Stack
                else
                {
                    Stack stack = card.stack;
                    stack.TakeUpperCardFromStack();
                    Hand_Selecting.instance.AddCardToHand(card);
                }
            }
        }

        if (gesture == TabletGesture.Move)
        {
            // Calculate position
            Vector3 pos = Vector3.zero;
            pos = (Vector3)(touchlist[0].currentPosition - touchlist[0].objectOffsets[card.gameObject]);

            // Update single card's position
            if (card.stack == null)
            {
                card.transform.position = Tablet.instance.TabletToWorldSpace(pos);
            }
            // Update stack position
            else
            {
                card.stack.transform.position = Tablet.instance.TabletToWorldSpace(pos);
            }
        }

        if (gesture == TabletGesture.Tap && Time.time - interactionTime > interactionCD)
        {
            //interactionTime = Time.time;
            //card.transform.Rotate(new Vector3(0, 0, -90));
        }

        if (gesture == TabletGesture.Release)
        {

        }

        if (gesture == TabletGesture.Lift)
        {
            Debug.Log("Pickup" + card);
            if (Hand_Selecting.instance.GetNumCardsInHand() < Hand_Selecting.instance.GetNumAllowedCardsInHand() || Hand_Selecting.instance.GetNumAllowedCardsInHand() < 0)
            {
                interactionTime = Time.time;

                //Pickup from Table
                if (card.stack == null)
                {
                    Tablet.instance.TakeCardFromTable(card);
                    Hand_Selecting.instance.AddCardToHand(card);
                }

                //Pickup from Stack
                else
                {
                    Stack stack = card.stack;
                    stack.TakeUpperCardFromStack();
                    Hand_Selecting.instance.AddCardToHand(card);
                }
            }
        }

        // Place card on stack
        if (gesture == TabletGesture.PlaceOnStack)
        {
            Hand_Selecting.instance.TakeCardFromHand();
            card.stack.AddCard(card);
        }
    }

    // Handles interaction between hands in the air
    public void HandleInteractionBetweenHands(Card card)
    {
        //Checks if hands are tracked
        if (Hand_Selecting.instance.IsHandRegistered() && Hand_CardCollection.instance.IsHandRegistered())
        {
            //Interaction only with nearest card
            //int i = Hand_CardCollection.instance.FindIndexOfNearestCard(Hand_Selecting.instance.GetGrabPosition());
            //if (Hand_CardCollection.instance.GetDistanceToCard(i, Hand_Selecting.instance.GetGrabPosition()) < grabDistance)
            {
                //Check if actions are on cooldown
                if (Time.time - interactionTime > interactionCD)
                {
                    //Take Card from left hand if allowed
                    if (Hand_Selecting.instance.GetNumCardsInHand() < Hand_Selecting.instance.GetNumAllowedCardsInHand() || Hand_Selecting.instance.GetNumAllowedCardsInHand() < 0)
                    {
                        interactionTime = Time.time;
                        Debug.Log("handInteraction: take card from left hand");
                        //Card grabbedCard = Hand_CardCollection.instance.TakeCardFromHand(i);
                        Card grabbedCard = Hand_CardCollection.instance.TakeCardFromHand(card);
                        Debug.Log("handInteraction: add card to right hand");
                        Hand_Selecting.instance.AddCardToHand(grabbedCard);
                    }
                    //Put card in left hand if allowed
                    else if (Hand_Selecting.instance.GetNumCardsInHand() > 0)
                    {
                        interactionTime = Time.time;

                        Card selectedCard = Hand_Selecting.instance.TakeCardFromHand();
                        Hand_CardCollection.instance.AddCardToHand(selectedCard);
                    }
                }
            }
        }
    }
}
