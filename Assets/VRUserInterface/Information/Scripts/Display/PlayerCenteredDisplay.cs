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
		

		private void PositionMenu(GameObject obj)
		{
			if (obj)
			{
				//Position object ath the reference position to get the right orientation
				obj.transform.position = Reference.transform.position;
				obj.transform.LookAtAndRotate180Degrees(VRCameraEnable.instance.GetCameraCenter());
	            obj.transform.position = VRCameraEnable.instance.GetCameraCenter();
	            obj.transform.Translate(localOffset);
				obj.transform.position += globalOffset;
				obj.transform.LookAtAndRotate180Degrees(VRCameraEnable.instance.GetCameraCenter());
			}
		}
		
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

