using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	/// <summary>
	/// Triggers a callback if the angle between the look direction of an object and the direction vector towards a second object is higher
	/// than a specified threshold
	/// </summary>
	public class LookAwayCallback : MonoBehaviour {

		public delegate void Callback();

		public Callback callback;

		/// <summary>
		/// How far do you have to look away from the container center to close the view? (x = horizontal angle, y = vertical angle)
		/// For the moment, I only use the x value though
		/// </summary>
		public Vector2 lookAwayThreshold = new Vector2(40,30);


		// Update is called once per frame
		void Update () {
			//If you look away from the button, reset it
			float angle = Vector3.Angle (VRCameraEnable.instance.GetCameraCenterObject ().transform.forward, (transform.position - VRCameraEnable.instance.GetCameraCenter()).normalized);
			if (Mathf.Abs(angle)>lookAwayThreshold.x) callback();
		}
	}
}