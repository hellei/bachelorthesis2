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
	public GameObject leftForetwistModel, rightForetwistModel;

	public GameObject leftShoulderModel, rightShoulderModel;



	public GameObject leftHandModel, rightHandModel;
	/// <summary>
	/// The leap hand.
	/// </summary>
	GameObject leftHand, rightHand;
	GameObject leftElbow, rightElbow;

	//Quaternion initialHeadRotation = Quaternion.identity;

	void SetInitialHeadRotation()
	{
		//initialHeadRotation = neck.transform.localRotation;
	}

	void UpdateHands()
	{
		leftHand = GameObject.Find ("L_Wrist");
		rightHand = GameObject.Find ("R_Wrist");
		leftElbow = GameObject.Find ("ForetwistLeft");
		rightElbow = GameObject.Find ("ForetwistRight");
		UpdateHand (leftForetwistModel, leftShoulderModel, leftHandModel, leftHand, leftElbow);
		UpdateHand (rightForetwistModel, rightShoulderModel, rightHandModel, rightHand, rightElbow);
	}

	bool initialRotationSet = false;

	Vector3 initialTwist1Offset;

	Quaternion initialLeftShoulderRotation, initialRightShoulderRotation;

	void UpdateHand (GameObject model, GameObject shoulder, GameObject hand, GameObject handTarget, GameObject target)
	{
		if (!target) return;

		shoulder.transform.right = -(target.transform.position - shoulder.transform.position).normalized;


		model.transform.right = -target.transform.up;
		//model.transform.rotation = target.transform.rotation;

		model.transform.position = target.transform.position;

		hand.transform.position = handTarget.transform.position;
		hand.transform.rotation = handTarget.transform.rotation;

		hand.transform.Rotate (new Vector3 (0, 90, 0));
		hand.transform.Rotate (new Vector3 (180, 0, 0));
		//hand.transform.right = handTarget.transform.forward;
		//hand.transform.Rotate (new Vector3 (0, 0, 0));
	}

	public Vector3 armLength = new Vector3(0,0.25f,0);

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
		//TODO: Implement in local space of player
		planeX = new Plane(transform.right, upperBody.transform.position);// new Vector3(1, 0, 0)
		planeY = new Plane(transform.up, upperBody.transform.position);//new Vector3(0,1,0)
		planeZ = new Plane(transform.forward, upperBody.transform.position);//new Vector3(0, 0, 1)
        
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

		//Calculate the lean angle sidewards
        float roll = GetDirectionAngle(upperBody.transform.position, cameraPos, planeZ, planeY, planeX);

		//Calculate the lean angle forwards and backwards
        float pitch = GetDirectionAngle(upperBody.transform.position, cameraPos, planeX, planeY, planeZ);
        upperBody.transform.localRotation = Quaternion.Euler(new Vector3(0, -roll, -pitch));
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
        Vector3 planePosABC = c.ProjectPointOnPlane(origin, planePosAB, out sign);

        //Get hypotenuse
        Vector3 direction = (planePosA - origin);
        //get adjacent leg
        Vector3 directionX = (planePosAB - origin);

		return Mathf.Asin(directionX.magnitude / direction.magnitude) * sign * Mathf.Rad2Deg;
    }
}
