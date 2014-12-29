using UnityEngine;
using System.Collections;
using VRUserInterface;

/// <summary>
/// Moves the player model depending on the position of the player in the virtual space
/// </summary>
public class MoveModel : MenuCallback {

	private bool _isVisible = true;

	public bool IsVisible
	{
		get
		{
			return _isVisible;
		}
	}

	#region implemented abstract members of MenuCallback
	public override void ReceiveMenuCallback (params string[] info)
	{
		//Hides or shows the character
		if (info[0] == "Hide")
		{
			_isVisible = false;
		}
		else if (info[0] == "Show")
		{
			_isVisible = true;
		}
		foreach (Transform t in transform)
		{
			t.gameObject.SetActive(_isVisible);
		}
	}
	#endregion

    public GameObject upperBody;

	// Use this for initialization
	void Start () {
        CreatePlayerPlanes();
		SetInitialHeadRotation ();
	}
	
	// Update is called once per frame
	void Update () {
        OrientBodyToCamera();
		RotateHead ();
		UpdateHands ();
	}

	/// <summary>
	/// The neck joint, used to rotate the head. This object does not have to be set, as you usually do not see your head.
	/// </summary>
	//public GameObject neck;

	/// <summary>
	/// The left hand model, this is the actual model hand.
	/// </summary>
	public GameObject leftHandModel, rightHandModel;

	/// <summary>
	/// The leap hand.
	/// </summary>
	public GameObject leftHand, rightHand;

	//Quaternion initialHeadRotation = Quaternion.identity;

	void SetInitialHeadRotation()
	{
		//initialHeadRotation = neck.transform.localRotation;
	}

	void UpdateHands()
	{
		UpdateHand (leftHandModel, leftHand);
		UpdateHand (rightHandModel, rightHand);
	}

	void UpdateHand (GameObject model, GameObject target)
	{
		if (!target) return;
		model.transform.position = target.transform.position;
	}

	void RotateHead()
	{
		//TODO: Implement correctly or remove code (this is mainly interesting for debug purposes)
		/*if (neck)
		{
			neck.transform.rotation = initialHeadRotation;//Quaternion.Inverse(initialHeadRotation) * VRCameraEnable.instance.GetCameraCenterObject().transform.rotation;
			neck.transform.Rotate(new Vector3(90,0,0));
			neck.transform.Rotate(new Vector3(0,-90,0));
		}*/
	}

    Plane planeX, planeY, planeZ;

    void CreatePlayerPlanes()
    {
        planeX = new Plane(new Vector3(0, 0, 1), upperBody.transform.position);
        planeY = new Plane(new Vector3(0,1,0), upperBody.transform.position);
        planeZ = new Plane(new Vector3(-1, 0, 0), upperBody.transform.position);
    }
    GameObject test;

	/// <summary>
	/// The offset of the neck compared to the camera position
	/// </summary>
	public Vector3 neckOffset;

    void OrientBodyToCamera()
    {
		GameObject cameraCenterObject = null;
		if (!VRCameraEnable.instance)
		{
			cameraCenterObject = Camera.main.gameObject;
		}
		else
		{
			cameraCenterObject = VRCameraEnable.instance.GetCameraCenterObject();
		}
        Vector3 cameraPos = cameraCenterObject.transform.TransformPoint(neckOffset);

        float pitch = GetDirectionAngle(upperBody.transform.position, cameraPos, planeX, planeY, planeZ);
        float roll = GetDirectionAngle(upperBody.transform.position, cameraPos, planeZ, planeY, planeX);
        //Debug.Log(dY + " / " + distance +": "+pitch);
        upperBody.transform.localRotation = Quaternion.Euler(new Vector3(0, pitch, -roll));
    }
	

    /// <summary>
    /// Projects a point on a plane and gets the angle.
    /// </summary>
    /// <param name="p">The targeted points</param>
    /// <param name="a">The first plane</param>
    /// <param name="b">The second plane</param>
    /// <param name="b">The third plane, only used for the sign</param>
    /// <returns></returns>
    float GetDirectionAngle(Vector3 origin, Vector3 p, Plane a, Plane b, Plane c)
    {
        float sign;
        //Project point onto a plane
        Vector3 planePosA = a.ProjectPointOnPlane(origin, p, out sign);

        //Project point onto a and b plane
        Vector3 planePosAB = b.ProjectPointOnPlane(origin, planePosA, out sign);

        //Project point onto a and b plane
        //Vector3 planePosABC = c.ProjectPointOnPlane(origin, planePosAB, out sign);

        //Get hypotenuse
        Vector3 direction = (planePosA - origin);
        //get adjacent leg
        Vector3 directionX = (planePosAB - origin);
        float distance = direction.magnitude;
        return Mathf.Asin(directionX.magnitude / distance) * sign * Mathf.Rad2Deg;
    }
}
