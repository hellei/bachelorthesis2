using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRUserInterface;

public class Evaluation : MonoBehaviour {

	public enum State {Waiting, Ready, TimerRunning, Finished}

	EvaluationResult result = new EvaluationResult ();

	public ButtonType[] buttonTests;

	State state = State.Waiting;

	public GameObject cardContainer;
	GameObject copy;
	// Use this for initialization
	void Start () {
		result.tests = new EvaluationResult.ButtonTest[buttonTests.Length];
	}

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
		switch (state)
		{
		case State.Waiting:
			renderer.material.color = colWaiting;
			if (Selection.instance.WatchedObject == gameObject && !InformationController.instance.activeObject)
			{
				//Prevent the player from selecting the same card twice
				if (lastSelectedCard)
				{
					Destroy (lastSelectedCard);
				}
				state = State.Ready;
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
			if (InformationController.instance.activeObject)
			{
				float selectionTime = (Time.time - timerStart);
				Debug.Log("Selected card in " + selectionTime + " seconds");
				state = State.Waiting;
				cardsSelected++;
				if (cardsSelected >= numberOfTests)
				{
					result.tests[i].selectionTimes[cardsSelected-1-numberOfTests] = selectionTime;
				}
				if (cardsSelected >= numberOfCards)
				{

					result.Calculate(i);
					cardContainer.SetActive(false);
					copy.SetActive(true);
					cardContainer = copy;
					copy = null;
					if (i < buttonTests.Length-1)
					{
						cardsSelected = 0;
						i++;
					}
					else
					{
						result.Save("result_"+Random.Range(0,10000)+".xml");
						state = State.Finished;
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

	GameObject lastSelectedCard;

}
