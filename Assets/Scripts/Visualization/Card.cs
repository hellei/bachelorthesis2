using UnityEngine;
using System.Collections;
using VRUserInterface;

public enum CardState {
    OnHand, OnTable, OnStack, Selected
}


//[RequireComponent(typeof(InformationObject))]
/// <summary>
/// The main script that deals with the card information.
/// </summary>
public class Card : MonoBehaviour {

    public CardState cardState;

    public Stack stack;

	public float modelHeight = 0.013f;

	public string card;

	InformationObject io;

	void Awake()
	{
		SetUp ();

	}

	/// <summary>
	/// Sets up the card. Can also be used to update the card information if you change it at runtime.
	/// </summary>
	public void SetUp()
	{
		info = CardTypes.instance.Get (card);
		renderer.material.mainTexture = info.sprite.texture;
		io = GetComponent<InformationObject> ();
		if (io)
		{
			io.infoText = info.infoText;
		}
		GenerateShortInfo ();
		ShowShortInfo (false);
	}

	public CardTypes.CardInfo info
	{
		get;
		private set;
	}

	void OnValidate()
	{
		//AssignTexture ();
	}


	/// <summary>
	/// A game object that defines where the cost info should be shown
	/// </summary>
	public GameObject costInfoTarget;
	//public GameObject strengthInfoTargetPosition;

	public float iconDistance = 0.05f;

	public float iconScale = 0.05f;


	/// <summary>
	/// Generates a short info about the card (e.g. cost, name)
	/// </summary>
	/// <returns>The short info.</returns>
	void GenerateShortInfo()
	{
		int i = 0;
		//Generate all sprites defined in the cost list
		foreach (Sprite sprite in info.cost)
		{
			GameObject obj = new GameObject("icon");
			SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
			sr.sprite = sprite;

			float offset = CenteredPosition(i, info.cost.Length);
			obj.transform.parent = costInfoTarget.transform;
			obj.transform.localPosition = Vector3.zero;
			obj.transform.LookAtAndRotate180Degrees(VRCameraEnable.instance.GetCameraCenter());
			obj.transform.Translate (new Vector3(offset * iconDistance,0,0));
			obj.transform.localScale = iconScale * Vector3.one;
		    i++;
		}
	}

	public void ShowShortInfo(bool value)
	{
		costInfoTarget.SetActive (value);
	}

	/// <summary>
	/// Returns the position of an item in a list that is centered around 0
	/// </summary>
	/// <returns>The position.</returns>
	/// <param name="i">The index.</param>
	/// <param name="count">Count.</param>
	public static float CenteredPosition(int i, int count)
	{
		return (-((count * 0.5f) - 0.5f) + i);
	}
}
