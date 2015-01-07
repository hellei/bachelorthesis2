﻿/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

// Class to setup a rigged hand based on a model.
public class RiggedHand : HandModel {

  public Transform palm;
  public Transform foreArm;

  public override void InitHand() {
    UpdateHand();
  }

  public override void UpdateHand() {
    if (palm != null) {
      //palm.position = GetPalmPosition();
      //palm.rotation = GetPalmRotation();
        if (hand_.IsLeft)
        {
            palm.position = Vector3.Lerp(palm.position, GetPalmPosition(), Time.deltaTime * 20);
            palm.rotation = Quaternion.Slerp(palm.rotation, GetPalmRotation(), Time.deltaTime * 20);
        }
        else
        {
            palm.position = Vector3.Lerp(palm.position, GetPalmPosition(), Time.deltaTime * 20);
            if (Vector3.Dot(GetPalmNormal(), Vector3.up) < 0)
            {
                palm.rotation = Quaternion.Slerp(palm.rotation, GetPalmRotation(), Time.deltaTime * 20);
            }
        }
    }

    if (foreArm != null)
	{
      foreArm.rotation = GetArmRotation();
	}

    for (int i = 0; i < fingers.Length; ++i) {
      if (fingers[i] != null && hand_.IsLeft && fingers[i].fingerType == Finger.FingerType.TYPE_THUMB)
        fingers[i].UpdateFinger();
      if (fingers[i] != null && hand_.IsRight)
      {
          //fingers[i].UpdateFinger();
      }
    }
  }
}
