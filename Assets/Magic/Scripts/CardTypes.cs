using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRUserInterface;

public class CardTypes : MonoBehaviour {

	[System.Serializable]
	public class ShortInfo
	{
		public string name;
		public List<string> info = new List<string> ();

		public ShortInfo(string name, List<string> info)
		{
			this.name = name;
			this.info = info;
		}

		public ShortInfo() {}
	}


	public class AvailableCards : XMLSaveAndLoad<AvailableCards>
	{
		public List<ShortInfo> cards = new List<ShortInfo>();
	}
	public static CardTypes instance;

	void Awake()
	{
		if (instance)
		{
			Debug.LogError("There must not be more than one instance in card types");
		}
		instance = this;
		if (printAvailableCards) WriteXML ();
	}

	void WriteXML()
	{
		Debug.Log("Saving available cards");
		AvailableCards xml = new AvailableCards ();
		foreach (CardInfo c in cards)
		{
			string cname = c.name;
			List<string> info = new List<string>();
			foreach (InformationObject.InfoBox ib in c.infoText)
			{
				info.Add(ib.text.Replace("//",""));
			}
			xml.cards.Add(new ShortInfo(cname, info));
		}
		xml.Save ("Decks/available_cards.xml");
	}

	/// <summary>
	/// If set to true, an xml file with all available cards is printed.
	/// </summary>
	public bool printAvailableCards = false;

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
