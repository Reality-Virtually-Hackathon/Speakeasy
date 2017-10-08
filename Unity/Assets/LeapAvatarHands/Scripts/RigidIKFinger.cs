using UnityEngine;
using System.Collections;
using Leap.Unity;

/**
A basic RigidFinger for use with the IK solution. Makes physics work.

    Author: Ivan Bindoff
    */

namespace LeapAvatarHands
{

    public class RigidIKFinger : RigidFinger
    {
        /** An added offset vector. */
        public Vector3 offset_ = Vector3.zero;

        public override void UpdateFinger()
        {
            for (int i = 0; i < bones.Length; ++i)
            {
                if (bones[i] != null)
                {
                    // Set bone dimensions.
                    CapsuleCollider capsule = bones[i].GetComponent<CapsuleCollider>();
                    if (capsule != null)
                    {
                        // Initialization
                        capsule.direction = 2;
                        bones[i].localScale = new Vector3(1f, 1f, 1f);

                        // Update
                        capsule.radius = GetBoneWidth(i) / 2f;
                        capsule.height = GetBoneLength(i) + GetBoneWidth(i);
                    }

                    Rigidbody boneBody = bones[i].GetComponent<Rigidbody>();
                    if (boneBody)
                    {
                        boneBody.MovePosition(GetBoneCenter(i) + offset_);
                        boneBody.MoveRotation(GetBoneRotation(i));
                    }
                    else
                    {
                        bones[i].position = GetBoneCenter(i) + offset_;
                        bones[i].rotation = GetBoneRotation(i);
                    }
                }
            }
        }
    }
}
