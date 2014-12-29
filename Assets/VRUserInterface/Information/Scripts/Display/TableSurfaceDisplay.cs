using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	/// <summary>
	/// A variation of the table display in which the card info is shown on the table surface
	/// </summary>
	public class TableSurfaceDisplay : TableDisplay {
	    //The override function changes the width and height depending on the distance to the player
	    public override GameObject CreateObjectFromInfo(InformationObject infoObjectScript)
	    {
	        GameObject result = base.CreateObjectFromInfo(infoObjectScript);
	        //Position at the active object position and rotate it 90 degrees to align it with the table surface
	        result.transform.position = Reference.transform.position;
	        result.transform.rotation = Quaternion.Euler(90, 0, 0);
	        result.transform.Translate(displayPosition, Space.Self);
	        return result;
	    }
	}
}