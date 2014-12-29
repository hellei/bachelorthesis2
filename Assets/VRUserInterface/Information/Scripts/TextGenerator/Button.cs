using UnityEngine;
using System.Collections;


namespace VRUserInterface
{

	/// <summary>
	/// The abstract class Button defines all relevant properties of a button, e.g. selecting,
	/// setting an info icon that is shown on the button and defining a callback when the button is selected.
	/// </summary>
	public abstract class Button : MonoBehaviour {
		public delegate void Back();
	    public event Back OnButtonPressed;
	    public string text;

	    protected void CallEvent()
	    {
	        var handler = OnButtonPressed;
	        if (handler != null)
	        {
	            handler();
	        }
	    }

		[HideInInspector]
		/// <summary>
		/// The creator is the object that "creates" the button, e.g. the info button on the card is created by the card.
		/// </summary>
		public InformationObject creator;

		/// <summary>
		/// Defines if the button always looks towards the player or has a fixed rotation
		/// </summary>
		private bool _lookAtPlayer = true;

	    /// <summary>
	    /// Defines whether the button always faces towards the player or looks forward.
	    /// Warning: By setting the value to false, the rotation is reset
	    /// </summary>
		public bool LookAtPlayer
		{
			set
			{
				if (!value) transform.localRotation = Quaternion.identity;//Quaternion.Euler (initialRotation);
				_lookAtPlayer = value;
			}
			get
			{
				return _lookAtPlayer;
			}
		}

		public Vector3 initialRotation;

		protected void Update()
		{
			if (LookAtPlayer)
			{
				transform.LookAt(VRCameraEnable.instance.GetCameraCenter());
				transform.Rotate(initialRotation);
			}
			//Test the conditions
			if (condition != null)
			{
				ButtonState lastState = state;
				state = condition.Test (conditionInfo);
				if (state != lastState) UpdateState();
			}
			if (menuCondition != null)
			{
				ButtonState lastState = state;
				MenuState menuState = menuCondition.TestCondition (conditionInfo);
				if (menuState == MenuState.Show) state = ButtonState.Active;
				else if (menuState == MenuState.Hidden) state = ButtonState.Inactive;
				else if (menuState == MenuState.ShowSecondState)
				{
					state = ButtonState.Invisible;
					if (GetComponent<MenuIcon>())
					{
						GetComponent<MenuIcon>().SwitchToSecondState();
					}
				}
				if (state != lastState) UpdateState();
			}
		}


		void Start()
		{
			if (condition != null && menuCondition != null) Debug.LogError("You can only use the IButtonCondition or the MenuCondition but not both at the same time");
		}
		public abstract void SetIcon(Sprite icon);

		protected ButtonState state;
		/// <summary>
		/// You can define a condition that defines whether the button is shown, inactive or completely hidden
		/// </summary>
		public IButtonCondition condition;

		/// <summary>
		/// In addition to the IButtonCondition variable the button component offers a menu condition which is
		/// an abstract class and can therefore be seen in the editor.
		/// </summary>
		public MenuCondition menuCondition;

		public string[] conditionInfo;

		protected bool objectSelectable = true;

		private float initialScale;

		/// <summary>
		/// Updates the state depending on the variable state
		/// </summary>
		protected virtual void UpdateState ()
		{
			if (state == ButtonState.Active)
			{
				objectSelectable = true;
				if (transform.localScale.x == 0) transform.localScale = new Vector3(initialScale, transform.localScale.y, transform.localScale.z);
			}
			else if (state == ButtonState.Inactive)
			{
				objectSelectable = false;
			}
			else if (state == ButtonState.Invisible)
			{
				if (transform.localScale.x != 0)
				{
					initialScale = transform.localScale.x;
					transform.localScale = new Vector3(0, transform.localScale.y, transform.localScale.z);
				}
			}
		}
	}
}
