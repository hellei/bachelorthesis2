using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	public class ViewSelection : Selection {
		// Use this for initialization
		new void Start () {
	        base.Start();
		}





		/// <summary>
		/// Does a raytrace from the camera to get the object the player currently looks at
		/// </summary>
		/// <returns>The watched object.</returns>
		protected override GameObject GetWatchedObject(){
			RaycastHit hit;
	        if (Physics.Raycast(VRCameraEnable.instance.GetCameraCenter(), VRCameraEnable.instance.GetCameraCenterObject().transform.forward, out hit))
			{
				return hit.collider.gameObject;
			}
			return null;
		}
	}
}
