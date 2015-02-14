/******************************************************************************\
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

  private Hand_Permanent permanentHand;  

  public override void InitHand() {
    UpdateHand();
  }

  public void InitPermanentHand(Hand_Permanent hand)
  {
      permanentHand = hand;
      
      UpdateHand();
  }

  public override void UpdateHand()
  {
	
	if ((InteractionManager.instance.touchOnTablet && hand_.IsRight)
		 || (InteractionManager.instance.GetTrackingMode(0) == false && hand_.IsLeft)
	     || (InteractionManager.instance.GetTrackingMode(1) == false && hand_.IsRight))
      {
			if (hand_.IsLeft) Debug.Log("Left Hand tracking disabled");
			if (hand_.IsRight) Debug.Log("Right Hand tracking disabled");
          	return;
      }

		

      if (palm != null)
      {
          //palm.position = GetPalmPosition();
          //palm.rotation = GetPalmRotation();
          if (hand_.IsLeft)
          {
              if (permanentHand != null)
              {
                  //Update permanent Hand
                  HandUpdateData handData = new HandUpdateData();
                  handData.isLeft = true;
                  handData.palmPosition = GetPalmPosition();
                  handData.palmRotation = GetPalmRotation();
                  handData.palmNormal = GetPalmNormal();
                  handData.armRotation = GetArmRotation();
                  permanentHand.UpdateHand(handData);
              }

              // Update Leap Hand
              palm.position = Vector3.Lerp(palm.position, GetPalmPosition(), Time.deltaTime * 20);
              palm.rotation = Quaternion.Slerp(palm.rotation, GetPalmRotation(), Time.deltaTime * 20);
          }
          else
          {
              if (permanentHand != null)
              {
                  //Update permanent Hand
                  HandUpdateData handData = new HandUpdateData();
                  handData.isLeft = false;
                  handData.palmPosition = GetPalmPosition();
                  handData.palmRotation = GetPalmRotation();
                  handData.palmNormal = GetPalmNormal();
                  handData.armRotation = GetArmRotation();
                  permanentHand.UpdateHand(handData);
              }

              palm.position = Vector3.Lerp(palm.position, GetPalmPosition(), Time.deltaTime * 20);
              Vector3 up = (new Vector3(1, 1, 0)).normalized;//Vector3.up;
              if (Vector3.Dot(GetPalmNormal(), up) < 0)
              {
                  palm.rotation = Quaternion.Slerp(palm.rotation, GetPalmRotation(), Time.deltaTime * 20);
              }
          }
      }

      if (foreArm != null)
      {
          foreArm.rotation = GetArmRotation();
      }

      for (int i = 0; i < fingers.Length; ++i)
      {
          if (fingers[i] != null && hand_.IsLeft)
          {
              if (fingers[i].fingerType == Finger.FingerType.TYPE_THUMB || InteractionManager.instance.leftHandFingerUpdateMode == FingerUpdateMode.Enabled)
              {
                  fingers[i].UpdateFinger();
              }

              if (fingers[i].fingerType != Finger.FingerType.TYPE_THUMB && InteractionManager.instance.leftHandFingerUpdateMode == FingerUpdateMode.Disabled)
              {
                  fingers[i].setBonesToDefault();
              }
          }

          if (fingers[i] != null && hand_.IsRight)
          {
              if (InteractionManager.instance.rightHandFingerUpdateMode == FingerUpdateMode.Enabled)
              {
                  fingers[i].UpdateFinger();
              }

              if (InteractionManager.instance.rightHandFingerUpdateMode == FingerUpdateMode.Disabled)
              {
                  fingers[i].setBonesToDefault();
              }
          }
    }
  }
}
