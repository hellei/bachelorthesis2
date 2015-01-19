using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	public class EarlyUpdate : MonoBehaviour {
		// Update is called once per frame
		void Update () {
			InformationObject.recreateButtons = false;
		}
	}
}
	