using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	/// <summary>
	/// The abstract class TextGenerator defines a layout for returning a textbox, given an information
	/// </summary>
	public abstract class TextGenerator : MonoBehaviour {
		public abstract GameObject CreateTextObject(InformationObject.InfoBox infoBox);
	}
}
