using UnityEngine;
using System.Collections;
using VRUserInterface;
using System.Collections.Generic;

public class DisplayEvaluation : MonoBehaviour {
	GameObject lastActiveObject = null;
	
	float lastSetActiveDisplay = 0;
	
	DisplayType dt = DisplayType.TableDisplay;

	[System.Serializable]
	public class DisplayInfo
	{
		public InformationDisplay script;
		public DisplayType type;
	}

	public List<DisplayInfo> displays;


	EvaluationResult result = new EvaluationResult ();

	void Start()
	{
		displays.Shuffle();
		PrepareCards ();
		result.displayTests = new EvaluationResult.DisplayTest[displays.Count];
		SetDisplayType (0);
	}

	[System.Serializable]
	public class TextObject
	{
		public string name;
		//The number of objects of 1 type
		public List<GameObject> objects;

	}


	public TextObject[] cards;

	void PrepareCards()
	{
		//Prepare x lists of 5 cards
		//Test if all values are correct first
		if (cards.Length != numberOfCards)
		{
			Debug.LogError("The number of card types needs to be equal to the number of cards per display type");
		}
		foreach (TextObject c in cards)
		{
			if (c.objects.Count != displays.Count)
			{
				Debug.LogError("The number of displays tested needs to be equal to the number of different text objects of one type (text length)");
			}
		}
		shuffledCards = new GameObject[displays.Count, numberOfCards];
		for (int i = 0; i < numberOfCards; i++)
		{
			cards[i].objects.Shuffle();
			for (int i2 = 0; i2 < displays.Count; i2++)
			{
				shuffledCards[i2,i] = cards[i].objects[i2];
			}
		}
	}

	/// <summary>
	/// The shuffled cards. First array field defines the different displays, the second one the number of cards tested
	/// </summary>
	public GameObject[,] shuffledCards;

	void Update()
	{
		UpdateDisplayTests ();
	}

	int idx_cards = 0;
	int idx_displays = 0;

	/// <summary>
	/// The first x number of cards are ignored
	/// </summary>
	public int numberOfTests = 1;

	public int numberOfCards = 4;

	EvaluationResult.DisplayTest edt;

	void UpdateDisplayTests()
	{
		if (InformationController.instance.activeObject && !lastActiveObject)
		{
			lastSetActiveDisplay = Time.time;
		}
		else if (!InformationController.instance.activeObject && lastActiveObject)
		{
			if (idx_cards==numberOfTests)
			{
				edt = new EvaluationResult.DisplayTest();
				edt.selectionTimes = new float[numberOfCards - numberOfTests];
				edt.dt = displays[idx_displays].type;
				result.displayTests[idx_displays] = edt;
			}
			if (idx_cards>=numberOfTests)
			{
				edt.selectionTimes[idx_cards - numberOfTests] = Time.time - lastSetActiveDisplay;
				Debug.Log("Closing display: Read text in " + (Time.time - lastSetActiveDisplay));
			}
			//Destroy (lastActiveObject);
			lastActiveObject.SetActive(false);
			idx_cards++;
			if (idx_cards >= numberOfCards)
			{
				//Finish this round
				idx_displays++;
				idx_cards = 0;
				edt.CalculateAverage();
				Debug.Log("Finished this display type evaluation");
				if (idx_displays < displays.Count)
				{
					//Change the display type
					SetDisplayType(idx_displays);
					//CreateCardCopy();
				}
				else {
					//The evaluation is finished.
					gameObject.SetActive(false);
					result.Save("result_display_"+Random.Range(0,10000)+".xml");
					Debug.Log("Saving result!");
				}
			}
		}
		lastActiveObject = InformationController.instance.activeObject;
	}

	public GameObject cardContainer;

	GameObject copy;

	void CreateCardCopy()
	{
		if (copy) Destroy (copy);
		copy = (GameObject)Instantiate (cardContainer);
		copy.transform.SetParent (transform, false);
		cardContainer.SetActive (false);
		copy.SetActive (true);
	}

	void SetDisplayType(int i)
	{
		InformationController.instance.infoDisplay = displays [i].script;
		//Update card visibility
		for (int x = 0; x < displays.Count; x++)
		{
			for (int y = 0; y < numberOfCards; y++)
			{
				shuffledCards[x, y].SetActive(x == i);
				if (x==i) Debug.Log(shuffledCards[x,y]);
			}
		}

		//
		Debug.Log ("Setting info display to " + InformationController.instance.infoDisplay);
	}
}
