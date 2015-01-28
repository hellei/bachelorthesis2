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
		public float lookAwayThreshold = 30;

		/// <summary>
		/// Sometimes the initial angle on creation of the object with the look away callback is larger than the threshold itself.
		/// If this flag is set, on startup the threshold will be set to the maximum of the defined threshold and the initial angle.
		/// </summary>
		public bool maxToInitialThreshold = false;

		void Start()
		{
			if (maxToInitialThreshold)
			{
				float initialAngle = Vector3.Angle (VRCameraEnable.instance.GetCameraCenterObject ().transform.forward, (transform.position - VRCameraEnable.instance.GetCameraCenter()).normalized);
				lookAwayThreshold = Mathf.Max (lookAwayThreshold, initialAngle + 5);
			}
		}

		// Update is called once per frame
		void Update () {
			//If you look away from the button, reset it
			float angle = Vector3.Angle (VRCameraEnable.instance.GetCameraCenterObject ().transform.forward, (transform.position - VRCameraEnable.instance.GetCameraCenter()).normalized);
			if (Mathf.Abs(angle)>lookAwayThreshold) callback();
		}
	}

	/// <summary>
	/// This function triggers a callback once you looked at the object and stop looking at it.
	/// </summary>
	public class StopLookingAtObjectCallback : MonoBehaviour
	{
		public delegate void Callback();
		
		public Callback callback;

		bool lookedAtOnce = false;

		public float minTimeAlive = 0.8f;

		float creationTime = 0;

		void Update()
		{
			if (creationTime == 0.0f) creationTime = Time.time;
			if (Selection.instance.WatchedObject == gameObject && (Time.time - creationTime) > minTimeAlive)
			{
				lookedAtOnce = true;
			}
			else if (lookedAtOnce && Selection.instance.WatchedObject.tag != Tags.buttonComponent)
			{
				callback();
				Debug.Log("leaving object! callback!");
				lookedAtOnce = false;
			}
		}
	}
}