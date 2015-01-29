using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Playground : MonoBehaviour {

	// Use this for initialization
	void Start () {
		int idx = 0;
		foreach (Transform t in cardContainer.transform)
		{
			Hand_CardCollection.instance.AddCardToHand(t.GetComponent<Card>());
			idx++;
			if (idx>4) return;
		}
	}

	void OnDisable()
	{
		CardBucket[] copy = new CardBucket[Hand_CardCollection.instance.cardsOnHand.Count];
		Hand_CardCollection.instance.cardsOnHand.CopyTo (copy);
		foreach (CardBucket bucket in copy)
		{
			Hand_CardCollection.instance.TakeCardFromHand(bucket.card);
			Destroy (bucket.card.gameObject);
		}
	}
	public GameObject cardContainer;
}
