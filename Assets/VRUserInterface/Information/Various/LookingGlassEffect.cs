using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	/// <summary>
	/// Attach this component to a text box or info item. If the player looks at it the object will increase in size
	/// and move forwards to help the player read or see it.
	/// </summary>
	public class LookingGlassEffect : MonoBehaviour {
		float timeSelected = 0;

		public float magnifyingFactor = 1.5f;
		/// <summary>
		/// Defines how far the object moves forward on select
		/// </summary>
		public float moveForwardValue = 0.1f;

		public float moveUpwardValue = 0;
		/// <summary>
		/// How long does the popup effect take
		/// </summary>
		public float popupDuration = 0.7f;
		/// <summary>
		/// The object should often decrease to its original size faster than it increases to the magnified size.
		/// This can be achieved by setting this multiplication factor.
		/// </summary>
		public float minimizeSpeedMultiplication = 2;

		public Vector3 scalePivot = Vector3.zero;


		// Update is called once per frame
		void Update () {
			GameObject sel = Selection.instance.WatchedObject;
			//If the currently watched object is a button, test if the creator was this object. If yes, it is deemed selected.

			GameObject buttonParentSelection = null;
			if (sel && sel.IsButton())
			{
				Button selButton = sel.GetButtonComponent();
				if (selButton && selButton.creator)
				{
					buttonParentSelection = selButton.creator.gameObject;
				}
			}
			if (sel == gameObject || buttonParentSelection == gameObject)
			{
				//If the object was not selected in the last frame set up all position values
				if (timeSelected == 0) SetValues();
				timeSelected += Time.deltaTime;

				timeSelected = Mathf.Min(timeSelected, popupDuration);
				float progress = 1;
				if (popupDuration != 0) progress = timeSelected / popupDuration;
				SetEffect(progress);
			}
			//If the other object is a button, do not scale down as the button is probably positioned on the enlarged object
			else if (!sel || !sel.IsButton() || (buttonParentSelection && buttonParentSelection != gameObject))
			{
				//It has to be tested if the time selected is zero, before a change is applied.
				//The first frame the selection time switches back to zero, you still have to set the position of the label.
				bool timeSelectedEqualsZero = (timeSelected == 0);
				timeSelected -= Time.deltaTime * minimizeSpeedMultiplication;
				timeSelected = Mathf.Max(0,timeSelected);
				float progress = 0;
				if (popupDuration != 0) progress = timeSelected / popupDuration;
				if (!timeSelectedEqualsZero && initialLocalScale != Vector3.zero) SetEffect(progress);
			}
		}

		public AnimationCurve sizeCurve;

		void SetEffect(float progress)
		{
			float factor = sizeCurve.Evaluate (progress);
			float sizeIncrease = Mathf.Lerp (1.0f, magnifyingFactor, factor);
			if (ob)
			{
				ob.min = initialObMin * sizeIncrease;
				ob.max = initialObMax * sizeIncrease;
			}

			transform.localScale = initialLocalScale * sizeIncrease;
			float moveForwardFactor = Mathf.Lerp (0, moveForwardValue, factor);
			float moveUpwardFactor = Mathf.Lerp (0, moveUpwardValue, factor);
			transform.localPosition = initialLocalPosition + new Vector3 (0, moveUpwardFactor, -moveForwardFactor);
		}

		void Start()
		{
			ob = GetComponent<OverrideBounds>();
		}



		Vector3 initialLocalScale;
		[HideInInspector]
		public Vector3 initialLocalPosition;

		OverrideBounds ob;

		Vector3 initialObMin, initialObMax;

		/// <summary>
		/// Sets up all values needed for the effect.
		/// </summary>
		void SetValues()
		{
			initialLocalScale = transform.localScale;
			initialLocalPosition = transform.localPosition;
			if (ob){
				initialObMin = ob.min;
				initialObMax = ob.max;
			}
		}
	}
}
