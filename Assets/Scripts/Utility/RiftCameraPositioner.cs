using UnityEngine;
using System.Collections;
using Ovr;

public class RiftCameraPositioner : MonoBehaviour {
    public OVRCameraRig ovrCameraController;

    public TextMesh tm;

	// Use this for initialization
	void Start () {
		Debug.Log ("I am used!!!");
        transform.position = ovrCameraController.centerEyeAnchor.position;
	}

    void Update()
    {
        tm.text = ""+Vector3.Distance(transform.position, GameObject.Find("CameraLeft").transform.position);
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
