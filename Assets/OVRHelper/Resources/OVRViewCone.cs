using UnityEngine;
using System.Collections;
using Ovr;

public class OVRViewCone : MonoBehaviour {

	private GameObject viewConeMesh;
	public OVRCameraRig camController;

	// Use this for initialization
	void Start () 
	{        
		UpdateCameraTracker ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		/*if (Input.GetKeyDown (KeyCode.JoystickButton0) || Input.GetKeyDown (KeyCode.Return))
		{
			OVRDevice.ResetOrientation ();
			updateCameraTracker ();
		}*/
		/*Transform camCenter = null;
		if (camController.centerEyeAnchor)
		{
			camCenter = camController.centerEyeAnchor;
		}
		else
		{
			camCenter = Camera.main.transform;
		}
		Vector3 cameraPos = camCenter.position;*/
	}

    /// <summary>
    /// Position the camera tracker at the correct position
    /// </summary>
	void UpdateCameraTracker()
	{
		Vector3 IRCameraPos = Vector3.zero;
		Quaternion IRCameraRot = Quaternion.identity;
		
		float cameraHFov = 0;
		float cameraVFov = 0;
		float cameraNearZ = 0;
		float cameraFarZ = 0;
		
		GetIRCamera (ref IRCameraPos, ref IRCameraRot, ref cameraHFov, ref cameraVFov, ref cameraNearZ, ref cameraFarZ);
		
		IRCameraPos.z *= -1;

        transform.localPosition = IRCameraPos + camController.transform.position;
		transform.rotation = IRCameraRot;
		
		//float horizontalScale = Mathf.Tan (cameraHFov / 2f);
		//float verticalScale = Mathf.Tan (cameraVFov / 2f);

		//transform.localScale = new Vector3 (horizontalScale * cameraFarZ, verticalScale * cameraFarZ, cameraFarZ);
	}
	
	bool GetIRCamera(	ref Vector3 position, 
						ref Quaternion rotation, 
						ref float cameraHFov, 
						ref float cameraVFov, 
						ref float cameraNearZ, 
						ref float cameraFarZ)
	{
		if (OVRManager.capiHmd == null)
			return false;
		
		TrackingState ss =  OVRManager.capiHmd.GetTrackingState();
		
		rotation = new Quaternion(	ss.CameraPose.Orientation.x,
		                          ss.CameraPose.Orientation.y,
		                          ss.CameraPose.Orientation.z,
		                          ss.CameraPose.Orientation.w);
		
		position = new Vector3(	ss.CameraPose.Position.x,
		                       ss.CameraPose.Position.y,
		                       ss.CameraPose.Position.z);
		
		HmdDesc desc = OVRManager.capiHmd.GetDesc();
		
		cameraHFov = desc.CameraFrustumHFovInRadians;
		cameraVFov = desc.CameraFrustumVFovInRadians;
		cameraNearZ = desc.CameraFrustumNearZInMeters;
		cameraFarZ = desc.CameraFrustumFarZInMeters;
		
		//OVRDevice.OrientSensor (ref rotation);
		
		return true;
	}
}
