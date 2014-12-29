/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

// The model for our skeletal hand made out of various polyhedra.
public class SkeletalHand : HandModel {

  protected const float PALM_CENTER_OFFSET = 0.0150f;

  public GameObject palm;
  public GameObject forearm;

  void Start() {
    IgnoreCollisionsWithSelf();
  }

  public override void InitHand() {
    SetPositions();
  }

  public override void UpdateHand() {
    SetPositions();
  }

  protected Vector3 GetPalmCenter() {
    Vector3 offset = PALM_CENTER_OFFSET * Vector3.Scale(GetPalmDirection(), transform.localScale);
    return GetPalmPosition() - offset;
  }

  private void SetPositions() {

    for (int f = 0; f < fingers.Length; ++f) {
      if (fingers[f] != null)
        fingers[f].InitFinger();
    }

    if (palm != null) {
      palm.transform.position = GetPalmCenter();
      palm.transform.rotation = GetPalmRotation();
        Vector3.Lerp(palm.transform.position, GetPalmCenter(), Time.deltaTime);
        Quaternion.Slerp(palm.transform.rotation, GetPalmRotation(), Time.deltaTime);
    }

    if (forearm != null) {
      forearm.transform.position = GetArmCenter();
      forearm.transform.rotation = GetArmRotation();
        Vector3.Lerp(forearm.transform.position, GetArmCenter(), Time.deltaTime);
        Quaternion.Slerp(forearm.transform.rotation, GetArmRotation(), Time.deltaTime);
    }
  }
}


