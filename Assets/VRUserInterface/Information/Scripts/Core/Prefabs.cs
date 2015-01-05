using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	/// <summary>
	/// Used to set global prefabs
	/// </summary>
	public class Prefabs : MonoBehaviour {

		public static Prefabs instance;

		void Awake()
		{
			instance = this;
		}

		[System.Serializable]
		public class Buttons
		{
			public Button simple;
			public Button timer;
			public Button threeDots;
			public Button progressBar;
			public Button arrows;
			public Button dragCircle;
		}

		public Buttons buttons;
	}
}