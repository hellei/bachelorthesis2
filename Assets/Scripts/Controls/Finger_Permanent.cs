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
    public Quaternion[] defaultBoneOrientation = new Quaternion[4];
    


	// Use this for initialization
	void Start () {

        for (int i = 0; i < bones.Length; ++i)
        {
            if (bones[i] != null)
            {
                defaultBoneOrientation[i] = bones[i].localRotation;
                print(bones[i].transform.localRotation);
            }
        }
	
	}
	
	// Update is called once per frame
	void Update () {

        
	
	}

    public void InitFinger()
    {
        //UpdateFinger();
    }

    public void resetBoneRotation()
    {
        for (int i = 0; i < bones.Length; ++i)
        {
            if (bones[i] != null)
            {
                bones[i].localRotation = defaultBoneOrientation[i];
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
