using UnityEngine;
using System.Collections;

namespace VRUserInterface
{
	/// <summary>
	/// A helper class used to define the center point of the left and right rift camera.
	/// Provides some additional features like disabling the rift camera and showing a normal camera in Editor Mode.
	/// </summary>
	public class VRCameraEnable : MonoBehaviour {

		public static VRCameraEnable instance;

		public GameObject unityCam, vrCam;
		public GameObject leftVrCam, rightVrCam;
	    public bool testRiftInEditor = false;



		/// <summary>
		/// Returns the position of the camera / player's head (center of two eye cameras)
		/// </summary>
		/// <returns>The camera center.</returns>
		public Vector3 GetCameraCenter()
		{
			if (!cameraCenter) CreateCenterObject();
			return cameraCenter.transform.position;
		}

		public GameObject GetCameraCenterObject()
		{
			if (!cameraCenter) CreateCenterObject();
			return cameraCenter;
		}

		void SetVariables()
		{
			if (!vrCam) vrCam = GameObject.Find ("OVRCameraController");
			if (!leftVrCam) leftVrCam = GameObject.Find ("CameraLeft");
			if (!rightVrCam) rightVrCam = GameObject.Find ("CameraRight");
		}

		void Awake()
		{
			instance = this;
			SetVariables ();
			CreateCenterObject();
		}
		// Use this for initialization
		void Start () {
	        if (testRiftInEditor)
	        {
	            unityCam.SetActive(false);
	            vrCam.SetActive(true);
	        }
	        else
	        {
	            unityCam.SetActive(true);
				unityCam.camera.transparencySortMode = TransparencySortMode.Orthographic;
	            vrCam.SetActive(false);
	        }
	#if !UNITY_EDITOR
			unityCam.SetActive(false);
			vrCam.SetActive(true);
	#endif
		}

		GameObject cameraCenter;

		public GameObject cursorPrefab;




		/// <summary>
		/// Creates a game object that is positioned between the left and the right eye camera.
		/// </summary>
		void CreateCenterObject()
		{
			cameraCenter = new GameObject ("CameraCenter");
			UIController.Instance.reference = cameraCenter;
			UpdateCenterObject ();
			//Add the cursor to the center object
			VRCursor cursor = cameraCenter.AddComponent<VRCursor> ();
			cursor.cursorPrefab = cursorPrefab;
		}

		public void UpdateCenterObject()
		{
			Vector3 pos = Vector3.zero;
			Quaternion rot = Quaternion.identity;
			if (unityCam.activeSelf)
			{
				pos = unityCam.transform.position;
				rot = unityCam.transform.rotation;
			}
			if (vrCam.activeSelf)
			{
				pos = (leftVrCam.transform.position + rightVrCam.transform.position) * 0.5f;
				rot = Quaternion.Lerp(leftVrCam.transform.rotation, rightVrCam.transform.rotation, 0.5f);
			}
			cameraCenter.transform.position = pos;
			cameraCenter.transform.rotation = rot;
		}

		void Update()
		{
			if (!cameraCenter) CreateCenterObject();
			UpdateCenterObject ();
		}
	}
}
