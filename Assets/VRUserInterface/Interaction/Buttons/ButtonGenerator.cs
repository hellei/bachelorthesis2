using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	public enum ButtonType {Timer, ThreeDots, ProgressBar, Arrows, DragCircle}

	/// <summary>
	/// Generates a button with the specificied parameters
	/// </summary>
	public class ButtonGenerator : MonoBehaviour {

		public GameObject Instantiate()
		{
			GameObject button = null;
			Button buttonPrefab = null;
			switch (buttonType)
			{
			case ButtonType.Timer:
				buttonPrefab = Prefabs.instance.buttons.timer;
				break;
			case ButtonType.ThreeDots:
				buttonPrefab = Prefabs.instance.buttons.threeDots;
				break;
			case ButtonType.ProgressBar:
				buttonPrefab = Prefabs.instance.buttons.progressBar;
				break;
			case ButtonType.Arrows:
				buttonPrefab = Prefabs.instance.buttons.arrows;
				break;
			case ButtonType.DragCircle:
				buttonPrefab = Prefabs.instance.buttons.dragCircle;
				break;
			}
			if (buttonPrefab != null)
			{
				button = (GameObject)Instantiate (buttonPrefab.gameObject);
				button.GetComponent<Button> ().SetIcon (icon);
			}
			else {
				Debug.LogError("No suitable button prefab found. Did you link all buttons in the prefabs component?");
			}

			return button;
		}

		public ButtonType buttonType;

		public Sprite icon;
	}
}
