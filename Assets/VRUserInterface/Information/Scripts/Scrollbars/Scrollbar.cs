using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VRUserInterface
{
	public abstract class Scrollbar : MonoBehaviour {
		/// <summary>
		/// Scrolls the specified amount.
		/// </summary>
		/// <param name="val">The specified scroll amount. -1 = scroll up, 1 = scroll down, 2 = scroll down two items...</param>
		public abstract void Scroll(int val);

		private int _numberOfShownElements = -1;

		/// <summary>
		/// Returns the number of elements that can be shown at a time
		/// </summary>
		/// <value>The number of shown elements.</value>
		protected int NumberOfShownElements
		{
			get
			{
				if (_numberOfShownElements != -1) return _numberOfShownElements;
				_numberOfShownElements = 0;
				foreach (GameObject obj in Elements)
				{
					if (obj.activeSelf) _numberOfShownElements++;
				}
				return _numberOfShownElements;
			}
		}


		/// <summary>
		/// The current position in the list of elements
		/// </summary>
		private int _currentPos = 0;

		protected int CurrentPos
		{
			get {return _currentPos;}
			set
			{
				_currentPos = value;
				//Clamp the value to be in the scroll range
				_currentPos = Mathf.Clamp(_currentPos, 0, MaxIndex);
			}
		}

		/// <summary>
		/// The maximum scroll index possible.
		/// </summary>
		/// <value>The index of the max.</value>
		protected int MaxIndex
		{
			get
			{
				return Elements.Count - NumberOfShownElements;
			}
		}

		/// <summary>
		/// Updates the info boxes
		/// </summary>
		protected void UpdateBoxes()
		{
			for (int i = 0; i < Elements.Count; i++)
			{
				Elements[i].SetActive(i >= CurrentPos && i < CurrentPos + NumberOfShownElements);
			}
		}

		private List<GameObject> _elements = null;

		//Returns a list with all scroll elements.
		protected List<GameObject> Elements
		{
			get
			{
				if (_elements == null)
				{
					_elements = new List<GameObject>();
					foreach (Transform t in gameObject.transform)
					{
						_elements.Add (t.gameObject);
					}
				}
				return _elements;
			}
		}
	}
}
