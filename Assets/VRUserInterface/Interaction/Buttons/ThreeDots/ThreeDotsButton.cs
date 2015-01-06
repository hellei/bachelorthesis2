using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VRUserInterface
{
	/// <summary>
	/// Creates a button with a number of dots. You have to look at all dots in the right order to press the button.
	/// Requires at least one instance of a view selection.
	/// </summary>
	public class ThreeDotsButton : Button {

		public TextMesh textMesh;
		public GameObject[] dots;

	    int dotsSelected = 0;

		void OnEnable()
		{
			initialRotation = new Vector3 (0, 180, 0);
			ResetButton ();
			Update ();
		}



		// Use this for initialization
		void Start () {
	        if (dots.Length == 0)
	        {
	            Debug.LogError("You need at least one dot assigned to the dots array");
	        }
			if (textMesh)
			{
				textMesh.text = text;
	        }
			ResetButton();
		}

		float dotSelectedTime = 0f;
		public float dotSelectionDuration = 0.5f;

		/// <summary>
		/// If you look away from the button, the button is resetted. This value 
		/// </summary>
		public float resetAngle = 30;

		// Update is called once per frame
		new void Update () {
			GameObject sel = Selection.instance.WatchedObject;
	        //Test if all dots have been selected
	        if (dotsSelected >= dots.Length)
	        {
	            AllDotsSelected();
	        }
	        else if (sel == dots[dotsSelected] && objectSelectable)
	        {
				dotSelectedTime += Time.deltaTime;
				ShowDot(dots[dotsSelected]);
				if (dotSelectedTime > dotSelectionDuration)
				{
					dotSelectedTime = 0f;
	            	dotsSelected++;
				}
	        }
			else {
				dotSelectedTime -= Time.deltaTime * 2;
				dotSelectedTime = Mathf.Max(dotSelectedTime, 0);
				ShowDot(dots[dotsSelected]);
			}

			//If you look away from the button, reset it
			float angle = Vector3.Angle (VRCameraEnable.instance.GetCameraCenterObject ().transform.forward, (transform.position - VRCameraEnable.instance.GetCameraCenter()).normalized);
			if (Mathf.Abs(angle)>resetAngle) ResetButton();
			base.Update ();
		}


	    void ResetButton()
	    {
	        //Disable the select dots
	        foreach (GameObject dot in dots)
	        {
				dot.GetComponent<CanvasRenderer>().SetAlpha(0.0f);
	        }
	        dotsSelected = 0;
	    }

	    void AllDotsSelected()
	    {
	        ResetButton();
	        CallEvent();
	    }

		public Image iconQuad;

	    /// <summary>
	    /// Shows the dot. You could show some cool animation here as well.
	    /// </summary>
	    /// <param name="dot"></param>
	    void ShowDot(GameObject dot)
	    {
	        dot.GetComponent<CanvasRenderer>().SetAlpha(1.0f);
			float factor = dotSelectedTime / Mathf.Max (dotSelectionDuration, 0.01f);
			factor = Mathf.Min (factor, 1);
			dot.GetComponent<CanvasRenderer>().SetAlpha(factor);
	    }

		public override void SetIcon(Sprite icon)
		{
			iconQuad.sprite = icon;
		}
	}
}
