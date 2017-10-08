/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap.Unity;
using LeapAvatarHands;

namespace LeapAvatarHands
{

    // The model for our rigid hand made out of various polyhedra.
    public class RigidIKHand : RigidHand
    {
        public float offset = 0.1f;
        [HideInInspector]
        public HandModel targetHand;



        public override void UpdateHand()
        {
            //skip the update if there's any unset parameters (i.e. we're not initialised properly yet)
            if (targetHand == null || hand_ == null || hand_.Direction == null)
            {
                return;
            }


            Vector3 offs = targetHand.GetPalmDirection() * offset;

            for (int f = 0; f < fingers.Length; ++f)
            {
                if (fingers[f] != null)
                {
                    ((fingers[f]) as RigidIKFinger).offset_ = offs;
                    fingers[f].UpdateFinger();
                }
            }

            if (palm != null)
            {
                Rigidbody palmBody = palm.GetComponent<Rigidbody>();
                if (palmBody)
                {
                    palmBody.MovePosition(GetPalmCenter() + offs);
                    palmBody.MoveRotation(GetPalmRotation());
                }
                else
                {
                    palm.position = GetPalmCenter() + offs;
                    palm.rotation = GetPalmRotation();
                }
            }

            if (forearm != null)
            {
                // Set arm dimensions.
                CapsuleCollider capsule = forearm.GetComponent<CapsuleCollider>();
                if (capsule != null)
                {
                    // Initialization
                    capsule.direction = 2;
                    forearm.localScale = new Vector3(1f, 1f, 1f);

                    // Update
                    capsule.radius = GetArmWidth() / 2f;
                    capsule.height = GetArmLength() + GetArmWidth();
                }

                Rigidbody forearmBody = forearm.GetComponent<Rigidbody>();
                if (forearmBody)
                {
                    forearmBody.MovePosition(GetArmCenter());
                    forearmBody.MoveRotation(GetArmRotation());
                }
                else
                {
                    forearm.position = GetArmCenter();
                    forearm.rotation = GetArmRotation();
                }
            }
        }
    }
}
