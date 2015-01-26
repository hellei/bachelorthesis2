using UnityEngine;
using System.Collections;
using VRUserInterface;

public enum CardState {
    OnHand, OnTable
}


//[RequireComponent(typeof(InformationObject))]
/// <summary>
/// The main script that deals with the card information.
/// </summary>
public class Card : MonoBehaviour {

	private CardState cardState = CardState.OnTable;

    public CardState CardState
	{
		get {
			return cardState;
		}
		set {
			cardState = value;
			GetComponent<LookingGlassEffect>().enabled = (cardState == CardState.OnHand);
			if (mio)
			{
				mio.button = (cardState == CardState.OnHand ? mio.buttonOnHand : mio.buttonOnTable);
			}
			io.RecreateButton();
		}
	}

    public Stack stack;

	public float modelHeight = 0.013f;

	public string card;

	public InformationObject io {
		get;
		private set;
	}

	void Awake()
	{
		SetUp ();

	}

	MagicInformationObject mio;

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
		mio = GetComponent<MagicInformationObject> ();
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


	void Update()
	{
		if (cardState == CardState.OnTable && !stack)
		{
			Vector3 environmentPosition = EnvironmentPositioner.instance.table.transform.position;
			float height = environmentPosition.y;
			float xFactor = environmentPosition.z - transform.position.z + 0.25f;//xFactor, probably between 0 and 0.5
			xFactor *= 0.003f;
			float zOffset = gameObject.GetBoundsSize().y * 0.5f + xFactor;
			transform.position = new Vector3(transform.position.x, height + zOffset, transform.position.z);
		}
	}

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
