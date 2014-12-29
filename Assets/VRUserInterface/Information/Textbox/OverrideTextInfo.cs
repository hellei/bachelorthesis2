using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VRUserInterface
{
	/// <summary>
	/// This can be used to provide information where in the game object structure a text item is defined.
	/// For example, in an info box you may have the text object as a child object of the main panel.
	/// </summary>
	public class OverrideTextInfo : MonoBehaviour {
		public void SetText(string str)
		{
			text.text = str;
		}

		public Text text;

		public Image background;

		public void SetTextColor(Color color)
		{
			text.color = color;
		}

		public void SetBackgroundColor(Color color)
		{
			background.color = color;
		}

	}
}