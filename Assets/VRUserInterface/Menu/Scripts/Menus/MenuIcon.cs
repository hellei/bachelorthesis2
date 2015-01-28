using UnityEngine;
using System.Collections;


namespace VRUserInterface
{
	/// <summary>
	/// Identifies this object as menu icon
	/// </summary>
	public class MenuIcon : MonoBehaviour {
	    //Constants
	    public const float standardTransparency = 0.4f;
		public const float filledTransparency = 0.58f;
		public const float moveForwardLength = 0.01f;
		public const float selectedFactor = 0.0f;




		public Menu.Icon.Callback callback;


		public Menu.Icon.Condition condition;

	    public string[] tooltip = null;

	    public Menu subMenuPrefab;

		Vector3 standardPosition, selectedPosition;

		/// <summary>
		/// Can be used to position a menu icon behind the others (e.g. used for the back and close button)
		/// </summary>
		public float zOffset = 0f;

		/// <summary>
		/// When this icon is selected, the second state icon is revealed.
		/// </summary>
		public GameObject secondState;

	    void Start()
	    {
			CreateButton ();
	    }

		void CreateButton()
		{
			Button button = GetComponent<Button> ();
			button.OnButtonPressed += TriggerAction;
			button.menuCondition = condition.component;
		}

		public ButtonGenerator buttonGenerator;

	    public bool isBackButton = false;
		public bool isCloseButton = false;


	    const float selectTimer = 1.4f;

		bool isSelected;

		/// <summary>
		/// If this flag is set, the icons show the defined text in a tag above the icon.
		/// </summary>
		protected bool showTag = true;



		[HideInInspector]
		public Menu parentMenu;


		/// <summary>
		/// When the icon is clicked, it triggers an action
		/// </summary>
		void TriggerAction()
		{
			//Call callback
			if (callback != null && callback.component != null)
			{
				callback.component.ReceiveMenuCallback(callback.info);
			}
			if (secondState)
			{
				SwitchToSecondState();
			}
			if (subMenuPrefab)
			{
				UIController.Instance.OpenSubMenu(subMenuPrefab.gameObject);
			}
			else if (isBackButton)
			{
				UIController.Instance.GoBack();
			}
			else if (isCloseButton)
			{
				UIController.Instance.CloseMenu();
			}
		}

		public void SwitchToSecondState()
		{
			secondState.SetActive(true);
			gameObject.SetActive(false);
		}
	}
}
