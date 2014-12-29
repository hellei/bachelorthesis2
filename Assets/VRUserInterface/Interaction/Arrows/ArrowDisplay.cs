using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	/// <summary>
	/// Shows the specified arrow (mouse cursor) infront of the player.
	/// </summary>
	public class ArrowDisplay : MonoBehaviour {

	    public GameObject arrowPrefab;

		// Use this for initialization
		void Start () {
	        CreateArrow();
		}

	    float distance = -1;
	    GameObject arrow;

	    Vector3 initialScale;

	    void CreateArrow()
	    {
	        arrow = (GameObject)Instantiate(arrowPrefab);
	        arrow.transform.parent = transform;
	        initialScale = transform.localScale;
	        UpdateArrow();
	    }

	    void UpdateArrow()
	    {
	        distance = -1;
	        RaycastHit hit;
	        if (Physics.Raycast(VRCameraEnable.instance.GetCameraCenter(), VRCameraEnable.instance.GetCameraCenterObject().transform.forward, out hit))
	        {
	            arrow.transform.position = hit.transform.position;
	            arrow.transform.localScale = Vector3.Distance(transform.position, hit.transform.position) * initialScale;
	        }
	        else
	        {
	            arrow.transform.localPosition = Vector3.zero;
	        }
	        //arrow.transform.localPosition = new Vector3(0, 0, distance);
	    }
	}
}
