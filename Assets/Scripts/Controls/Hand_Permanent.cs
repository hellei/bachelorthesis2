using UnityEngine;
using System.Collections;

public struct HandUpdateData
{
    public bool isLeft;
    public Vector3 palmPosition;
    public Quaternion palmRotation;
    public Vector3 palmNormal;
    public Quaternion armRotation;
}

public struct FingerUpdateData
{
    public Quaternion[] boneRotation;
}

public class Hand_Permanent : MonoBehaviour {

    public GameObject meshes;

    public Transform palm;
    public Transform foreArm;

    public Finger_Permanent[] fingers;

    private Vector3 defaultPosition;
    private Quaternion defaultOrientation;

    void Awake()
    {
        defaultPosition = palm.localPosition;
        defaultOrientation = palm.localRotation;

    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (InteractionManager.instance.touchOnTablet)
        {
            //meshes.SetActive(false);
            palm.localPosition = defaultPosition;
            palm.localRotation = defaultOrientation;
        }
        else
        {
            //meshes.SetActive(true);
        }	
	
	}

    /// <summary>
    /// Called by Leap to update permanent hand
    /// </summary>
    /// <param name="data">hand data needed for model update</param>
    public void UpdateHand(HandUpdateData data)
    {
        
        if (palm != null && !InteractionManager.instance.touchOnTablet)
        {
            // Update left hand
            if (data.isLeft)
            {
                palm.position = Vector3.Lerp(palm.position, data.palmPosition, Time.deltaTime * 20);
                palm.rotation = Quaternion.Slerp(palm.rotation, data.palmRotation, Time.deltaTime * 20);
            }
            // Update right hand
            else
            {
                palm.position = Vector3.Lerp(palm.position, data.palmPosition, Time.deltaTime * 20);
                Vector3 up = (new Vector3(1, 1, 0)).normalized;//Vector3.up;
                if (Vector3.Dot(data.palmNormal, up) < 0)
                {
                    palm.rotation = Quaternion.Slerp(palm.rotation, data.palmRotation, Time.deltaTime * 20);
                }
            }
        }

        // Update arm
        if (foreArm != null)
        {
            foreArm.rotation = data.armRotation;
        }
    }
}
