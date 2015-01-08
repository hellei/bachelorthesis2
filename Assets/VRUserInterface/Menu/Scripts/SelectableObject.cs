using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	/// <summary>
	/// This class defines an object that can be selected by the cursor and opens a menu.
	/// </summary>
	public class SelectableObject : MonoBehaviour {
	    /// <summary>
	    /// Determines which menu prefab is opened.
	    /// </summary>
	    public Menu menuPrefab;

		/// <summary>
		/// Sets this menu active on start.
		/// </summary>
		public bool menuActiveOnStart;

		public ButtonGenerator selectionButton;

		public GameObject buttonPosition;

		GameObject button;

		void Start()
		{
			if (selectionButton)
			{
				button = selectionButton.Instantiate();
				button.transform.parent = transform;
				button.transform.localPosition = buttonPosition.transform.localPosition;
				button.GetComponent<Button>().OnButtonPressed += OpenMenu;
			}
			if (menuActiveOnStart)
			{
				OpenMenu();
			}
		}

		public void OpenMenu()
		{
			//Inform the ui controller about the start menu
			UIController.Instance.SetMenu(menuPrefab.gameObject);
		}

		bool isWatched = false;

		void Update()
		{
			if (Selection.instance.WatchedObject.tag != "ButtonComponent") isWatched = (Selection.instance.WatchedObject == gameObject);
			button.SetActive(isWatched);
		}
	}
}