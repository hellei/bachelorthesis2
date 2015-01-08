using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	public class MagicInformationObject : InformationObject {

		Card card;

		// Use this for initialization
		protected override void Start () {
			base.Start ();
			card = GetComponent<Card>();
		}
		
		// Update is called once per frame
		protected override void Update () {
			ShowShortInfo(watchedObj == this);
			base.Update ();
		}

	    /// <summary>
	    /// An empty game object that defines the position of the info button if the card is on the hand
	    /// </summary>
	    public GameObject offsetOnHand, offsetOnTable;

	    /// <summary>
	    /// You can override the standard button size. The scale is multiplied with this value.
	    /// </summary>
	    public float buttonSize = 0.3f;

	    /// <summary>
	    /// The local position and scale of the button
	    /// </summary>
	    protected override void PositionInfoButton(GameObject obj)
	    {
	        GameObject reference = null;
	        switch (card.CardState)
	        {
	            case CardState.OnHand:
	                reference = offsetOnHand;
	                break;
	            case CardState.OnTable:
	                reference = offsetOnTable;
	                break;
	        }
	        obj.transform.SetParent(transform, false);
	        obj.transform.position = reference.transform.position;
	        obj.transform.localScale = obj.transform.localScale.ComponentProduct(reference.transform.localScale) * buttonSize; ;

	        selectionButton = obj.GetComponent<Button>();
	        selectionButton.OnButtonPressed += OnButtonSelection;
	        selectionButton.text = "Info";
	        selectionButton.condition = Tablet.instance;
	        if (card.CardState == CardState.OnHand)
	        {
	            selectionButton.LookAtPlayer = false;
	            obj.transform.Rotate(new Vector3(180, 0, 0));
	        }
	    }

		/// <summary>
		/// If you look at a card, you want a short information of the most important things.
		/// </summary>
		void ShowShortInfo(bool value)
		{
			card.ShowShortInfo (value);
		}

		/*public override void OnDisplayPrefabInstantiated(GameObject display)
		{
			base.OnDisplayPrefabInstantiated (display);
			//Delete all buttons in the children of the object
			foreach (Transform t in display.transform)
			{
				if (t.GetComponent<Button>())
				{
					Destroy (t.gameObject);
				}
			}
		}*/
	}
}
