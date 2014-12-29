using UnityEngine;
using System.Collections;
using VRUserInterface;

public class CardTypes : MonoBehaviour {

	public static CardTypes instance;

	void Awake()
	{
		if (instance)
		{
			Debug.LogError("There must not be more than one instance in card types");
		}
		instance = this;
	}

	/// <summary>
	/// Returns the card with the given string name.
	/// </summary>
	public CardInfo Get(string name)
	{
		foreach (CardInfo ci in cards)
		{
			if (ci.name == name) return ci;
		}
		Debug.LogError ("Card " + name + " not found in card types collection!");
		return null;
	}

	public CardInfo[] cards;

	[System.Serializable]
	public class CardInfo
	{
		/// <summary>
		/// The name is used to identify the card and needs to be unique
		/// </summary>
		public string name;
		public Sprite sprite;

		/// <summary>
		/// A list of icons that show what the card costs.
		/// </summary>
		public Sprite[] cost;

		public InformationObject.InfoBox[] infoText;

	}
}
