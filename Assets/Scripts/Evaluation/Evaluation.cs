﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRUserInterface;

public class Evaluation : MonoBehaviour {

	public enum State {Waiting, Ready, TimerRunning, Finished}

	EvaluationResult result = new EvaluationResult ();

	public bool onHand = false;

	public List<ButtonType> buttonTests;
	

	State state = State.Waiting;

	public GameObject cardContainer;
	GameObject copy;
	// Use this for initialization
	void Start () {
		buttonTests.Shuffle ();

		result.tests = new EvaluationResult.ButtonTest[buttonTests.Count];
		result.onHand = onHand;
		if (onHand)
		{
			SetUpHandCardCollection();
			//Debug.Log(containerObjects[1]+"  "+containerObjects[1].GetComponent<Card>());
		}
		else
		{
			InteractionManager.instance.Tracking (false);
		}
		InformationController.instance.newInformationObjectSelectedCallback += NewIOSelected;
	}

	void OnDisable()
	{
		InteractionManager.instance.Tracking (true);
	}

	int numberOfSelects = 0;

	public void NewIOSelected()
	{
		if (state == State.TimerRunning)
		{
			numberOfSelects++;
		}
	}

	void SetUpHandCardCollection()
	{
		containerObjects = new List<Transform> ();
		foreach (Transform t in cardContainer.transform)
		{
			containerObjects.Add(t);
		}
		Hand_CardCollection.instance.AddCardToHand(containerObjects[0].GetComponent<Card>(), 0);
		handCardIdx=1;
		toBeSelected = containerObjects[order[handCardIdx]].GetComponent<Card>();
	}


	/// <summary>
	/// The order of the cards to select on hand.
	/// </summary>
	public int[] order;

	int handCardIdx = 0;

	private Card _toBeSelected;

	Card toBeSelected
	{
		set {
			if (toBeSelected) toBeSelected.io.Highlight(false);
			_toBeSelected = value;
			toBeSelected.io.Highlight(true);
		}
		get
		{
			return _toBeSelected;
		}
	}

	List<Transform> containerObjects = new List<Transform>();

	Color colWaiting = Color.blue;
	Color colReady = Color.green;
	Color colTimerRunning = Color.yellow;
	Color colFinished = Color.gray;

	public int numberOfCards = 5;
	public int numberOfTests = 2;

	int i = 0;//The current index of the tests

	int cardsSelected = 0;

	public ButtonGenerator buttonPrefab;

	float timerStart = 0;
	// Update is called once per frame
	void Update () {
		TestAborts ();
		switch (state)
		{
		case State.Waiting:
			renderer.material.color = colWaiting;
			if (Selection.instance.WatchedObject == gameObject && !InformationController.instance.activeObject)
			{
				//Prevent the player from selecting the same card twice
				if (lastSelectedCard)
				{
					/*if (onHand)
					{
						Hand_CardCollection.instance.TakeCardFromHand(lastSelectedCard.GetComponent<Card>());
						lastSelectedCard.transform.position = new Vector3(0,-1000,0);
					}*/
					if (!onHand)
					{
						Destroy (lastSelectedCard);
					}
				}
				state = State.Ready;
				//Add new card to hand
				if (onHand && containerObjects[handCardIdx])
				{
					Hand_CardCollection.instance.AddCardToHand(containerObjects[handCardIdx].GetComponent<Card>(),0);
					toBeSelected = containerObjects[order[handCardIdx]].GetComponent<Card>();
				}
				handCardIdx++;
			}
			break;
		case State.Ready:
			renderer.material.color = colReady;
			if (Selection.instance.WatchedObject != gameObject)
			{
				if (result.tests[i]==null)
				{
					result.tests[i] = new EvaluationResult.ButtonTest();
					result.tests[i].selectionTimes = new float[numberOfCards - numberOfTests];
					result.tests[i].bt = buttonTests[i];
					buttonPrefab.buttonType = buttonTests[i];
					InformationObject.recreateButtons = true;
					Debug.Log("Next button type: "+buttonPrefab.buttonType);
				}
				state = State.TimerRunning;
				timerStart = Time.time;
				if (!copy)
				{
					copy = (GameObject)Instantiate (cardContainer);
					copy.transform.SetParent(cardContainer.transform.parent, false);
					copy.SetActive (false);
				}
			}
			break;
		case State.TimerRunning:
			renderer.material.color = colTimerRunning;
			//Stop timer when the information display has been opened
			bool condition = false;
			if (onHand)
			{
				condition = (InformationController.instance.activeObject == toBeSelected.gameObject);
			}
			else
			{
				condition = InformationController.instance.activeObject;
			}
			if (condition || Input.GetKeyDown(KeyCode.P))
			{
				float selectionTime = (Time.time - timerStart);
				Debug.Log("Selected card in " + selectionTime + " seconds");
				state = State.Waiting;
				if (cardsSelected >= numberOfTests)
				{
					result.tests[i].selectionTimes[cardsSelected-numberOfTests] = selectionTime;
				}
				cardsSelected++;
				if (cardsSelected >= numberOfCards)
				{

					result.Calculate(i);
					result.tests[i].falseSelects = (numberOfSelects - numberOfCards);
					cardContainer.SetActive(false);
					result.tests[i].abortedSelects = numberOfAborts - numberOfSelects;

					numberOfAborts = 0;
					numberOfSelects = 0;
					copy.SetActive(true);
					cardContainer = copy;
					copy = null;
					if (i < buttonTests.Count-1)
					{
						if (onHand)
						{
							//Reset all cards
							foreach (Transform t in containerObjects)
							{
								Hand_CardCollection.instance.TakeCardFromHand(t.GetComponent<Card>());
								t.transform.position = new Vector3(0,-1000,0);
							}
							SetUpHandCardCollection();
						}
						cardsSelected = 0;
						i++;
					}
					else
					{
						result.Save("result_"+Random.Range(0,10000)+".xml");
						state = State.Finished;
						InteractionManager.instance.Tracking (true);
					}
				}
				lastSelectedCard = InformationController.instance.activeObject;
			}
			break;
		case State.Finished:
			renderer.material.color = colFinished;
			break;
		}
	}

	int numberOfAborts = 0;

	GameObject lastSelectedCard;

	bool buttonSelected = false;

	/// <summary>
	/// Tests if a button selection has been aborted
	/// </summary>
	void TestAborts()
	{
		if (state == State.TimerRunning)
		{
			if (Selection.instance.WatchedObject.tag == Tags.buttonComponent)
			{
				buttonSelected = true;
			}
			else if (buttonSelected)
			{
				numberOfAborts++;
				buttonSelected = false;
			}
		}
		else
		{
			buttonSelected = false;
		}
	}
}
