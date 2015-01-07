using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VRUserInterface
{
	public class DragCircleButton : Button {

		void OnEnable()
		{
			initialRotation = new Vector3 (0, 180, 0);
			ResetButton ();
			Update ();
		}

		public Image iconQuad;

		public override void SetIcon(Sprite icon)
		{
			iconQuad.sprite = icon;
		}

		private Vector3 initialCircleLocalPosition;

		// Use this for initialization
		void Awake () {
			initialCircleLocalPosition = dragCircle.transform.localPosition;
		}
		/// <summary>
		/// If you look away from the button, the button is resetted. This value 
		/// </summary>
		public float resetAngle = 30;


		public GameObject dragCircle, dragExitCollider;

		bool dragCircleSelected = false;

		// Update is called once per frame
		new void Update () {
			GameObject sel = Selection.instance.WatchedObject;
			//The player has to directly look at the circle to start dragging it
			if (sel == dragCircle)
			{
				dragCircleSelected = true;
				DragCircle();
			}
			//Once selected, the circle can be dragged within the exit collider range
			else if (sel == dragExitCollider && dragCircleSelected)
			{
				DragCircle();
			}
			else 
			{
				dragCircleSelected = false;
			}
			
			//If you look away from the button, reset it
			float angle = Vector3.Angle (VRCameraEnable.instance.GetCameraCenterObject ().transform.forward, (transform.position - VRCameraEnable.instance.GetCameraCenter()).normalized);
			if (Mathf.Abs(angle)>resetAngle)
			{
				ResetButton();
			}
			base.Update ();
		}

		void Select()
		{
			ResetButton();
			//Disable the button once the event is triggered
			CallEvent();
		}

		void ResetButton()
		{
			dragCircle.transform.localPosition = initialCircleLocalPosition;
			dragCircleSelected = false;
		}

		public float requiredXOffset = 1.0f;

		/// <summary>
		/// Drag towards cursor position
		/// </summary>
		void DragCircle()
		{
			float lastLocalX = dragCircle.transform.localPosition.x;
			dragCircle.transform.position = VRCursor.instance.CursorPosition;
			dragCircle.transform.localPosition = new Vector3 (Mathf.Max (dragCircle.transform.localPosition.x, lastLocalX), initialCircleLocalPosition.y, 0);
			if (dragCircle.transform.localPosition.x > requiredXOffset)
			{
				Select ();
			}
		}
	}
}