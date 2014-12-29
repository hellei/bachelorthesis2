using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	public enum MenuState { Hidden, Show, ShowSecondState}

	/// <summary>
	/// You can define conditions for a menu to be shown. 
	/// </summary>
	public abstract class MenuCondition : MonoBehaviour {
		public abstract MenuState TestCondition(params string[] info);
	}
}
