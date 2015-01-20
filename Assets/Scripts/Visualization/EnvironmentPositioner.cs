using UnityEngine;
using System.Collections;
using VRUserInterface;
using System.IO;

public class EnvironmentPositioner : MenuCallback {
	#region implemented abstract members of MenuCallback
	public override void ReceiveMenuCallback (params string[] info)
	{
		if (info == null || info.Length == 0) return;
		switch (info[0])
		{
		case "ResetChair":
			SetChairPosition();
			break;
		case "Calibrate":
			StartCoroutine(DoCalibrate());
			break;
		case "Quit":
			QuitApplication();
			break;
		}
	}
	#endregion

	public GameObject quitApplication;
	

	public void QuitApplication()
	{
		StartCoroutine (DoQuitApplication ());
	}

	IEnumerator DoQuitApplication()
	{
		ShowEnvironment (false);
		Instantiate (quitApplication, VRCameraEnable.instance.GetCameraCenter(), Quaternion.identity);
		yield return new WaitForSeconds (4.0f);
		Application.Quit ();
		Debug.Log ("Quitting Application");
	}

    public static EnvironmentPositioner instance;

    public OVRViewCone riftPositionTracker;


    /// <summary>
    /// The container that contains the objects which are placed in the room and depend on the floor height
    /// </summary>
    public GameObject room;
    /// <summary>
    /// The container that contains the objects which are placed on the table and therefore depend on the table height.
	/// (0,0,0) is the left bottom corner on the surface of the table.
    /// </summary>
    public GameObject table;

    /// <summary>
    /// The chair has to be placed separately because it depends on the rift position tracker position. It currently
    /// has to be placed infront of the riftPositionTracker.
    /// </summary>
    public GameObject chair;

	public MoveModel playerModel;

	// Use this for initialization
	void Start () {
        instance = this;
        PositionEnvironment();
	}

    float zFightingOffset = 0.003f;

	public GameObject calibrationObject;
	public Vector3 calibrationOffset = new Vector3(0,-0.02f,0);

	IEnumerator DoCalibrate()
	{
		yield return new WaitForSeconds (4.0f);
		Calibrate ();
	}

	void Calibrate()
	{
		Vector3 cal = calibrationObject.transform.position + calibrationOffset;
		//Place the table
		Vector3 tabletPosition = cal + new Vector3(Config.instance.tabletSize.x / 2, zFightingOffset, Config.instance.tabletSize.z / 2);
		//Set the offset
		tablet.transform.position = tabletPosition;
		tablet.transform.localScale = Config.instance.tabletSize;
		//The table position is the tablet position minus half the height of the tablet position (It is the table surface)
		Vector3 tablePosition = cal + new Vector3(0, 0, -Config.instance.zOffsetToTableBorder);
		table.transform.position = tablePosition;
		Vector3 relativeTabletOffset = cal - riftPositionTracker.transform.position;
		string content = relativeTabletOffset.x + "," + relativeTabletOffset.y + "," + relativeTabletOffset.z;
		File.WriteAllText("calibration.txt",content);
	}

    void PositionEnvironment()
    {
		/*The following lines might seem a little bit strange. The problem is that the Unity Global Space is defined
		 * by the initial Oculus Rift rotation. However, for this project we need the rift positional tracker to define the
		 * global space. As I found no way, to do this, I have to add a rotation offset to the Oculus Rift HMD rotation */
		Quaternion targetRot = Quaternion.Inverse (riftPositionTracker.transform.rotation);
		riftPositionTracker.transform.parent = VRCameraEnable.instance.vrCam.transform;
		VRCameraEnable.instance.vrCam.transform.rotation = targetRot;
		riftPositionTracker.transform.parent = null;

        //Place the table
		Vector3 tabletPosition = riftPositionTracker.transform.TransformPoint(Config.instance.relativeTabletPosition + new Vector3(Config.instance.tabletSize.x / 2, /*-Config.instance.tabletSize.y / 2 + */zFightingOffset, Config.instance.tabletSize.z / 2));
        //Set the offset
        tablet.transform.position = tabletPosition;
        tablet.transform.localScale = Config.instance.tabletSize;
        //The table position is the tablet position minus half the height of the tablet position (It is the table surface)
		Vector3 tablePosition = riftPositionTracker.transform.TransformPoint(Config.instance.relativeTabletPosition + new Vector3(0, 0, -Config.instance.zOffsetToTableBorder));
        table.transform.position = tablePosition;

        //Place the floor
        Vector3 floorPosition = tablePosition - new Vector3(0, Config.instance.tableSurfaceHeight, 0);
        room.transform.position = floorPosition;

		VRCameraEnable.instance.UpdateEditorCamera ();
		VRCameraEnable.instance.UpdateCenterObject ();
		SetChairPosition ();

		//If there is a saved deck, create a stack on the table
		if (ConfigLoader.savedDeck != null) CreateStack();

		//Mark the terrain static after positioning the environment
		terrain.isStatic = true;
    }

	public GameObject terrain;

	public Card cardPrefab;

	/// <summary>
	/// Deactivates the environment positioner child items
	/// </summary>
	/// <param name="value">If set to <c>true</c> value.</param>
	public void ShowEnvironment(bool value)
	{
		foreach (Transform t in transform)
		{
			t.gameObject.SetActive(value);
		}
	}


	void CreateStack()
	{
		Vector3 position = Tablet.instance.TabletToWorldSpace (stackPosition);
		GameObject stack = new GameObject ("mainDeck");
		stack.transform.parent = table.transform;
		stack.transform.position = position;
		Stack st = stack.AddComponent<Stack> ();
		foreach (string str in ConfigLoader.savedDeck.cards)
		{
			GameObject card = (GameObject)Instantiate(cardPrefab.gameObject);
			Card cardScript = card.GetComponent<Card>();
			cardScript.card = str;
			cardScript.SetUp();
			st.AddCard(cardScript);
		}
		st.Shuffle ();
	}

	/// <summary>
	/// The position of the main stack, given in normalized values
	/// </summary>
	public Vector2 stackPosition = new Vector2 (0.75f, 0);


    public GameObject tablet;
	

	void SetChairPosition()
	{
		if (!playerModel)
		{
			Debug.LogWarning("There is no model with a model script as child of the environment object");
			return;
		}
		//Place the chair underneath the camera
        //The seat position is the neck position of the player minus the height of him sitting
		Vector3 seatPosition = VRCameraEnable.instance.GetCameraCenterObject().transform.TransformPoint(playerModel.neckOffset) - new Vector3(0,Config.instance.personSitHeight,0);
		chair.transform.position = seatPosition;
	}
}
