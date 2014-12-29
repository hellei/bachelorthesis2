using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stack : MonoBehaviour {

	public class StackCard {
		public Card card;
		public StackCard(Card card, Stack stack){
			this.card = card;
			randomOffset = new Vector3(Random.Range(-stack.stackAccuracy, stack.stackAccuracy), 0, Random.Range(-stack.stackAccuracy, stack.stackAccuracy));
		}
		public Vector3 randomOffset;
	}

	public void Shuffle()  
	{  
		System.Random rng = new System.Random(System.Guid.NewGuid().GetHashCode());  
		int n = cards.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			StackCard value = cards[k];  
			cards[k] = cards[n];  
			cards[n] = value;  
		}
		UpdateStack ();
	}
	
	public List<StackCard> cards = new List<StackCard>();

	public void AddCard(Card card){
		cards.Add(new StackCard(card,  this));
		card.transform.parent = transform;
		UpdateStack ();
	}

	// Use this for initialization
	void Awake () {
		AddAllChildrenToStack();
		Shuffle ();
	}

	/// <summary>
	/// Searches all Card components in the children of the stack transform and adds them to the stack. Useful on startup
	/// </summary>
	void AddAllChildrenToStack(){
		foreach (Card card in transform.GetComponentsInChildren<Card>()){
			AddCard (card);
		}
	}

	public float stackAccuracy = 0.005f;//Determines how accurate the cards are placed on top of each other


	/// <summary>
	/// Remove a card from the stack.
	/// </summary>
	/// <param name="card">Card.</param>
	public void RemoveCardFromStack(Card card)
	{
		foreach (StackCard sc in cards)
		{
			if (sc.card == card)
			{
				sc.card.collider.enabled = true;
				cards.Remove(sc);
				break;
			}
		}
		UpdateStack ();
		DisableColliders ();
	}

	//Updates the world positions of the cards
	void UpdateStack(){
		float height = 0;//The stack height
		foreach (StackCard stackCard in cards){
			height += stackCard.card.modelHeight/2.0f;
			stackCard.card.transform.position = transform.position + new Vector3(stackCard.randomOffset.x, height, stackCard.randomOffset.z);
			stackCard.card.transform.rotation = Quaternion.Euler (new Vector3(90,0,0));
			height += stackCard.card.modelHeight/2.0f;
		}
	}
	/// <summary>
	/// Disable all colliders except the top card
	/// Thus, the player always moves the top card from the stack.
	/// </summary>
	void DisableColliders()
	{
		if (cards.Count == 0) return;
		foreach (StackCard sc in cards)
		{
			sc.card.collider.enabled = false;
		}
		cards [cards.Count - 1].card.collider.enabled = true;
	}

	// Update is called once per frame
	void Update () {
		DisableColliders ();
	}
}
