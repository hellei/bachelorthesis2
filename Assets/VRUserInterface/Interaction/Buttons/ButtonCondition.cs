using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	/// <summary>
	/// Active: The button is shown and can be selected
	/// Inactive: The button is grayed out and cannot be selected.
	/// Invisible: The button is not shown at all
	/// </summary>
	public enum ButtonState {Active, Inactive, Invisible}

	public interface IButtonCondition
	{
		ButtonState Test(params string[] info);
	}
}