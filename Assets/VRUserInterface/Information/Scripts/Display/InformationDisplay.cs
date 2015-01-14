using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	public abstract class InformationDisplay : MonoBehaviour {
	    /// <summary>
	    /// The main function. Creates the information depending on the currently selected object.
	    /// </summary>
	    /// <param name="obj"></param>
		public abstract void SetActiveObject(InformationObject obj);
		public abstract void DiscardActiveObject();
	}
}