/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

public class RiggedFinger : FingerModel {

  public static readonly string[] FINGER_NAMES = {"Thumb", "Index", "Middle", "Ring", "Pinky"};

  public Transform[] bones = new Transform[NUM_BONES];

  private Finger_Permanent permanentFinger;
  
  public override void InitFinger() {
    UpdateFinger();
  }

  public void InitPermanentFinger(Finger_Permanent finger)
  {
      permanentFinger = finger;
      UpdateFinger();
  }

  public override void UpdateFinger()
  {
      FingerUpdateData data = new FingerUpdateData();
      data.boneRotation = new Quaternion[4];

      for (int i = 0; i < bones.Length; ++i)
      {
          if (bones[i] != null)
          {
              //bones[i].rotation = GetBoneRotation(i);
              data.boneRotation[i] = GetBoneRotation(i);
              if (permanentFinger != null)
              {
                  permanentFinger.UpdateFinger(data);
              }
              bones[i].rotation = Quaternion.Slerp(bones[i].rotation, GetBoneRotation(i), Time.deltaTime * 20);
          }
      }
  }
}
