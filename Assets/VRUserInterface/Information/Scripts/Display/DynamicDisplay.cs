using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	/// <summary>
	/// A variation of the table display which shows the info box relative to the object and looks at the player.
	/// </summary>
	public class DynamicDisplay : TableDisplay {

		//The global display offset is independent of the display orientation
		//The local offset depends on the display orientation. The display always faces the player
		public Vector3 globalDisplayOffset, localDisplayOffset;

	    float scaleFactor;

	    //The override function changes the width and height depending on the distance to the player
	    public override GameObject CreateObjectFromInfo(InformationObject infoObjectScript)
	    {
	        Reference = infoObjectScript.gameObject;
	        scaleFactor = Vector3.Distance(Reference.transform.position + globalDisplayOffset, VRCameraEnable.instance.GetCameraCenter());
	        buttonScale = scaleFactor;
	        maxWidth = maxWidth * scaleFactor;
	        maxHeight = maxHeight * scaleFactor;
	        GameObject result = base.CreateObjectFromInfo(infoObjectScript);
	        if (scaleFactor != 0)
	        {
	            //Reset the max width and height to the normal value afterwards
	            maxWidth = maxWidth / scaleFactor;
	            maxHeight = maxHeight / scaleFactor;
	        }
	        buttonScale = 1;
	        FacePlayer(result);
	        return result;
	    }

	    /// <summary>
	    /// It is not the best option to have the display look directly at the player. Sometimes it looks better if it looks a little bit above
	    /// or to the side. This can be set by this variable. (It is defined in the local space of the camera center)
	    /// </summary>
	    public Vector3 lookAtOffset = new Vector3(0, 0.1f, 0);

	    private void FacePlayer(GameObject obj)
	    {
	        if (obj)
	        {
				obj.transform.position = Reference.transform.position;// + globalDisplayOffset * scaleFactor;
	            obj.transform.LookAtAndRotate180Degrees(VRCameraEnable.instance.GetCameraCenter());
	            obj.transform.Translate(localDisplayOffset * scaleFactor);
	            //obj.transform.LookAtAndRotate180Degrees(VRCameraEnable.instance.GetCameraCenterObject().transform.TransformPoint(lookAtOffset));
	        }
	    }

	    /// <summary>
	    /// If set to true, the display always keeps looking at the player. If not set, it turns towards the player on initialization
	    /// and then remains static.
	    /// </summary>
	    public bool alwaysFacePlayer = true;

		void Update()
		{
	        if (alwaysFacePlayer) FacePlayer(activeObject);
		}
	}
}