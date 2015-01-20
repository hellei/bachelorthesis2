using UnityEngine;
using System.Collections;

public enum FingerType
{
    Thumb, Index, Middle, Ring, Pinky
}

public class Finger_Permanent : MonoBehaviour {

    public static readonly string[] FINGER_NAMES = { "Thumb", "Index", "Middle", "Ring", "Pinky" };

    public FingerType fingertype = FingerType.Index;

    public Transform[] bones = new Transform[4];
    


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        
	
	}

    public void InitFinger()
    {
        //UpdateFinger();
    }

    public void resetBoneRotation(FingerUpdateData data)
    {
        for (int i = 0; i < bones.Length; ++i)
        {
            if (bones[i] != null)
            {
                bones[i].localRotation = data.boneRotation[i];
            }
        }
    }

    public void UpdateFinger(FingerUpdateData data)
    {
        for (int i = 0; i < bones.Length; ++i)
        {
            if (bones[i] != null)
            {
                bones[i].rotation = Quaternion.Slerp(bones[i].rotation, data.boneRotation[i], Time.deltaTime * 20);
            }
        }
    }
}
