using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractableObject : Interactable
{
    private Card card;

    public void Start()
    {
        Tap gtap = new Tap(this.gameObject, true);
        gtap.OnTriggering += tap;
        gestures.Add(gtap);

        Move gmove = new Move(this.gameObject, 1);
        gmove.OnTriggering += move;
        gestures.Add(gmove);

        Release grelease = new Release(this.gameObject, true);
        grelease.OnTriggering += release;
        gestures.Add(grelease);

        Pickup gpickup = new Pickup(this.gameObject, true);
        gpickup.OnTriggering += pickup;
        gestures.Add(gpickup);

        Lift glift = new Lift(this.gameObject);
        glift.OnTriggering += lift;
        gestures.Add(glift);

        card = GetComponent<Card>();
    }

    public void tap(List<TouchInfo> touchlist)
    {
        InteractionManager.instance.HandleCardOnTabletInteraction(card, touchlist, TabletGesture.Tap);
    }

    public void move(List<TouchInfo> touchlist)
    {
        InteractionManager.instance.HandleCardOnTabletInteraction(card, touchlist, TabletGesture.Move);
    }

    public void release(List<TouchInfo> touchlist)
    {
        InteractionManager.instance.HandleCardOnTabletInteraction(card, touchlist, TabletGesture.Release);
    }

    public void pickup(List<TouchInfo> touchlist)
    {
        InteractionManager.instance.HandleCardOnTabletInteraction(card, touchlist, TabletGesture.Pickup);
    }

    public void lift(List<TouchInfo> touchlist)
    {
        InteractionManager.instance.HandleCardOnTabletInteraction(card, touchlist, TabletGesture.Lift);
    }

    void OnTriggerStay(Collider other)
    {
        if (card.CardState == CardState.OnHand)
        {
            Debug.Log("Collision");
            InteractionManager.instance.HandleInteractionBetweenHands(card);
        }        
    }
}
