using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	public class PlayerCenteredDisplay : TableDisplay {

		/// <summary>
		/// The offset is given in the player's local space
		/// </summary>
		public Vector3 localOffset, globalOffset;
		
		
		//The override function changes the width and height depending on the distance to the player
		public override GameObject CreateObjectFromInfo(InformationObject infoObjectScript)
		{
			Reference = infoObjectScript.gameObject;
			buttonScale = 0.2f;
			GameObject result = base.CreateObjectFromInfo(infoObjectScript);
			PositionMenu(result);
			return result;
		}

		/// <summary>
		/// If this flag is set, the user's pitch is not considered
		/// </summary>
		public bool onlyConsiderCameraYRotation = true;

		private void PositionMenu(GameObject obj)
		{
			if (obj)
			{
				//Position object ath the reference position to get the right orientation
				obj.transform.position = Reference.transform.position;
				Vector3 target = VRCameraEnable.instance.GetCameraCenter();
				if (onlyConsiderCameraYRotation)
				{
					target = new Vector3(target.x, obj.transform.position.y, target.z);
				}
				obj.transform.LookAtAndRotate180Degrees(target);
	            obj.transform.position = VRCameraEnable.instance.GetCameraCenter();
	            obj.transform.Translate(localOffset);
				obj.transform.position += globalOffset;
				obj.transform.LookAtAndRotate180Degrees(VRCameraEnable.instance.GetCameraCenter());
				if (minimumHeight) EnsureMinimumY(obj);
			}
		}

		void EnsureMinimumY(GameObject obj)
		{
			Vector3 min, max;
			min = max = GameObjectExtensions.initializationVector;
			obj.GetBounds(ref min, ref max);
			//float bottomHeight = obj.transform.TransformPoint(
			if (obj.transform.position.y + min.y < minimumHeight.transform.position.y)
			{
				obj.transform.position = new Vector3(obj.transform.position.x, minimumHeight.transform.position.y - min.y, obj.transform.position.z);
			}
		}

		/// <summary>
		/// The infobox might sometimes overlap with other objects. It is beyond the scope of this bachelor thesis to create
		/// an algorithm who avoids any collisions but at least you should be able to define a bottom border.
		/// </summary>
		public GameObject minimumHeight;
		
		/// <summary>
		/// If set to true, the display always keeps looking at the player. If not set, it turns towards the player on initialization
		/// and then remains static.
		/// </summary>
		public bool alwaysFacePlayer = true;
		
		void Update()
		{
			if (alwaysFacePlayer) PositionMenu(activeObject);
		}
	}
}

