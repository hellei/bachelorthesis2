using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	public class ViewSelection : Selection {
		// Use this for initialization
		new void Start () {
	        base.Start();
		}


		// Update is called once per frame
		void Update () {
			GetWatchedSelectableObject();
		}

		public override GameObject GetSelectedObject(){
	        if (InformationObject.selectedObj)
	        {
	            return InformationObject.selectedObj.gameObject;
	        }
	        return null;
		}

		/// <summary>
		/// Does a raytrace from the camera to get the object the player currently looks at
		/// </summary>
		/// <returns>The watched object.</returns>
		public override GameObject GetWatchedSelectableObject(){
			RaycastHit hit;
	        if (Physics.Raycast(VRCameraEnable.instance.GetCameraCenter(), VRCameraEnable.instance.GetCameraCenterObject().transform.forward, out hit))
			{
	            //Tell the information object that it is watched
	            SetWatchedObject(hit.collider.gameObject);
				return hit.collider.gameObject;
			}
			SetWatchedObject(null);
			return null;
		}
	}
}
