using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRUserInterface;

public class Tablet : MonoBehaviour, IButtonCondition {

	/// <summary>
	/// If this value is set you cannot select buttons while you're touching the table.
	/// This should prevent the user from accidentally selecting buttons while interacting
	/// on the tablet
	/// </summary>
	public bool blockButtonSelectionWhenTabletTouched = false;

	#region IButtonCondition implementation
	ButtonState IButtonCondition.Test (params string[] info)
	{
		if (!blockButtonSelectionWhenTabletTouched) return ButtonState.Active;
		if (IsTouched ())
		{
			return ButtonState.Inactive;
		}
		else
		{
			return ButtonState.Active;
		}
	}
	#endregion
	
	public bool IsTouched()
	{
		return (GetTouches().Count > 0);
	}

    public static Tablet instance;
    public List<Card> cardsOnTable = new List<Card>();

    // maybe used later
    public List<Card> cardStack = new List<Card>();
    public List<Card> graveYard = new List<Card>();

    void Awake()
    {
        instance = this;
    }

    public void AddCardToTable(Card card, Vector2 touchPosition)
    {
		card.CardState = CardState.OnTable;
        card.transform.parent = null;
        card.transform.position = TabletToWorldSpace(new Vector2(touchPosition.x, touchPosition.y));
        card.transform.rotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        card.transform.parent = EnvironmentPositioner.instance.table.transform;
        card.gameObject.SetActive(true);
        card.gameObject.layer = (int) InteractionLayer.Interactable;
        cardsOnTable.Add(card);
    }

    public Card TakeCardFromTable(Card card)
    {
        //int i = cardsOnTable.IndexOf(card);
        //card = cardsOnTable[i];
        //cardsOnTable.RemoveAt(i);  
        Card tcard = card;
        tcard.transform.parent = null;
		
		card.stack = null;
        Debug.Log("Removed card from tablet" + card.transform.parent);
        return tcard;
    }

    /// <summary>
    /// Returns a Dictionary of all processed touch data including world positions
    /// </summary>
    /// <returns></returns>
	public Dictionary<int,TouchInfo> GetTouches()
	{
        Dictionary<int,TouchInfo> td = ServerTouchProcessing.instance.GetNormalizedTouchData();
        foreach (TouchInfo t in td.Values)
        {
            t.worldPosition = TabletToWorldSpace(t.currentPosition);
			//GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
			//obj.transform.localScale = Vector3.one * 0.01f;
			//obj.transform.position = t.worldPosition;
        }
        return td;
	}

    /// <summary>
    /// Returns the tablet touch position in world space.
    /// </summary>
    /// <param name="input">The normalized two-dimensional input</param>
    /// <returns></returns>
    public Vector3 TabletToWorldSpace(Vector2 input)
    {
        /*Vector2 norm = (input - new Vector2(0.5f, 0.5f));
        norm.x *= transform.localScale.x;
        norm.y *= transform.localScale.z;*/
        return transform.TransformPoint(new Vector3(input.x -0.5f, 0, input.y - 0.5f));
    }

    /// <summary>
    /// Returns the world position of the tablet touch position
    /// </summary>
    /// <param name="input">position in world space</param>
    /// <returns></returns>
    public Vector2 WorldToTabletSpace(Vector3 input)
    {
        Vector2 offset = new Vector2(input.x, input.z) - new Vector2(transform.position.x - transform.localScale.x / 2, transform.position.z - transform.localScale.z / 2);
        return new Vector2(offset.x / transform.localScale.x, offset.y / transform.localScale.z);

    }
}
