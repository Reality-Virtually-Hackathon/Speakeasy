/**
IKOrionLeapHandController
Extends the Leap Motion orion "LeapHandController" class to enable the use of a game avatar characters
own hands instead of disembodied virtual hands when using the Leap Motion controller.

Author: Ivan Bindoff
*/

using UnityEngine;
using Leap.Unity;
using Leap;
using System.Collections.Generic;

namespace LeapAvatarHands
{

    public class IKOrionLeapHandController : LeapHandController
    {
        public Animator animator;
        private GameObject rootNode;                            //the root, parent, gameobject that this script ultimately lives on.
        public bool ikActive = true;
        public bool leftActive = false;
        public bool rightActive = false;

        public GameObject avatarLeftHand;   //keeps track of the left hand gameobject for IK
        public GameObject avatarRightHand;  //keeps track of the right hand gameobject for IK
        [HideInInspector]
        public HandModel leftHand;
        [HideInInspector]
        public HandModel rightHand;

        [HideInInspector]
        public Vector3 lTarget;             //where we want the left hand to be in worldspace
        [HideInInspector]
        public Vector3 rTarget;                 //where we want the right hand to be in worldspace
        [HideInInspector]
        public Quaternion lTargetRot;           //the rotation we want for left hand
        [HideInInspector]
        public Quaternion rTargetRot;           //the rotation we want for the right hand

        protected float leftInactiveWeight = 0f;
        protected float rightInactiveWeight = 0f;
        public float inactiveLerpTime = 0.2f;   //how many seconds it takes to lerp back to default pose after a hand goes inactive

        private float lWeight = 0f;         //how much IK to apply to the left hand
        private float rWeight = 0f;         //how much IK to apply to the right hand

        public RigidIKHand leftPhysicsHand;
        public RigidIKHand rightPhysicsHand;

        protected virtual void Awake()
        {
            //find the Animator somewhere in this gameobject's hierarchy
            if (animator == null)
            {
                Transform t = transform;
                while (t != null)
                {
                    if (t.gameObject.GetComponent<Animator>())
                    {
                        animator = t.gameObject.GetComponent<Animator>();
                        rootNode = t.gameObject;
                        break;
                    }
                    t = t.parent;
                }
                if (animator == null)
                {
                    Debug.LogError("IKOrionLeapHandController:: no animator found on GameObject or any of its parent transforms. Are you sure you added IKLeapHandController to a properly defined rigged avatar?");
                }
            }

            if (leftHand == null && avatarLeftHand != null)
                leftHand = avatarLeftHand.GetComponent<RiggedHand>();
            if (rightHand == null && avatarRightHand != null)
                rightHand = avatarRightHand.GetComponent<RiggedHand>();

            if (leftHand == null)
                Debug.LogError("IKOrionLeapHandController::Awake::No Rigged Hand set for left hand parameter. You have to set this in the inspector.");
            else
                leftHand.BeginHand();
            if (rightHand == null)
                Debug.LogError("IKOrionLeapHandController::Awake::No Rigged Hand set for right hand parameter. You have to set this in the inspector.");
            else
                rightHand.BeginHand();

            if (leftPhysicsHand != null)
            {
                if (leftHand != null)
                    leftPhysicsHand.targetHand = leftHand;
            }
            if (rightPhysicsHand != null)
            {
                if (rightHand != null)
                    rightPhysicsHand.targetHand = rightHand;
            }
        }

        protected void Start()
        {
            provider = GetComponent<LeapProvider>();
            if (provider == null)
            {
                Debug.LogError("IKOrionLeapHandController::Start::No Leap Provider component was present on " + gameObject.name);
                Debug.Log("Added a Leap Service Provider with default settings.");
                gameObject.AddComponent<LeapServiceProvider>();
            }

        }

        void Update()
        {
            if (graphicsEnabled)
            {
                UpdateHandRepresentations();
            }
        }

        /// <summary>
        /// Calculates the hand targets and rotations AFTER the Leap data has been applied to the hand
        /// otherwise you'll just pick up the hand transform from the animation data rather than the leap
        /// data.
        /// </summary>
        void LateUpdate()
        {
            if (graphicsEnabled)
            {
                //find the LEAP hands and set them as the target
                SetHandTargets();
            }
        }

        /**
         * When an IK pass happens, this happens.
         * 
         * This handles the IK for the hands
         * 
         * IMPORTANT NOTE: You need to have IK pass turned on in the animator or this will never happen,
         * and your hands won't work properly! They will have horrible stretchy wrists at best.
         * */
        public void OnAnimatorIK()
        {
            //Debug.Log ("Receiving message");
            //if we're using the leap controller then do the animation IK override
            if (animator)
            {
                //if the IK is active, set the position and rotation directly to the goal. 
                if (ikActive)
                {
                    //set the position and the rotation of the left hand where the leap hand is
                    if (leftActive && leftHand != null)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, lWeight);
                        animator.SetIKPosition(AvatarIKGoal.LeftHand, lTarget);
                        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, lWeight);
                        animator.SetIKRotation(AvatarIKGoal.LeftHand, lTargetRot);
                        leftInactiveWeight = 1f;
                    }
                    else
                    {
                        //gradually lerp away from the target, returning hand back to normal operation but without a jarring effect
                        leftInactiveWeight -= Time.deltaTime / inactiveLerpTime;
                        if (leftInactiveWeight < 0f)
                        {
                            leftInactiveWeight = 0f;
                            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftInactiveWeight);
                            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftInactiveWeight);
                        }
                        else
                        {
                            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, leftInactiveWeight);
                            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
                            animator.SetIKPosition(AvatarIKGoal.LeftHand, lTarget);
                            animator.SetIKRotation(AvatarIKGoal.LeftHand, lTargetRot);
                        }
                    }

                    //set the position and the rotation of the right hand where the leap hand is
                    if (rightActive && rightHand != null)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rWeight);
                        animator.SetIKPosition(AvatarIKGoal.RightHand, rTarget);
                        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rWeight);
                        animator.SetIKRotation(AvatarIKGoal.RightHand, rTargetRot);
                        rightInactiveWeight = 1f;
                    }
                    else
                    {
                        //gradually lerp away from the target, returning hand back to normal operation
                        rightInactiveWeight -= Time.deltaTime / inactiveLerpTime;
                        if (rightInactiveWeight < 0f)
                        {
                            rightInactiveWeight = 0f;
                            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightInactiveWeight);
                            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightInactiveWeight);
                        }
                        else
                        {
                            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightInactiveWeight);
                            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
                            animator.SetIKPosition(AvatarIKGoal.RightHand, rTarget);
                            animator.SetIKRotation(AvatarIKGoal.RightHand, rTargetRot);
                        }
                    }
                }
                else
                {
                    //IK not active, resume normal operation
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                }
            }
        }

        /**
         * Set the current Leap hands as the IK targets
         * 
         * */
        private void SetHandTargets()
        {
            if (leftActive && leftHand != null)
            {
                lTarget = leftHand.transform.position/*.GetWristPosition()*/;
                lTargetRot = leftHand.transform.rotation;
                lWeight = 1f;
                //Debug.Log ("left hand target set to " + lTarget);
            }

            if (rightActive && rightHand != null)
            {
                rTarget = rightHand.transform.position/*.GetWristPosition()*/;
                rTargetRot = rightHand.transform.rotation;
                rWeight = 1f;
            }
        }

        /// <summary>
        /// Tells the hands to update to match the new Leap Motion hand frame data. Also keeps track of
        /// which hands are currently active.
        /// </summary>
        void UpdateHandRepresentations()
        {
            leftActive = false;
            rightActive = false;
            foreach (Leap.Hand curHand in provider.CurrentFrame.Hands)
            {
                if (curHand.IsLeft)
                {
                    leftHand.SetLeapHand(curHand);
                    leftHand.UpdateHand();
                    leftActive = true;
                }
                if (curHand.IsRight)
                {
                    rightHand.SetLeapHand(curHand);
                    rightHand.UpdateHand();
                    rightActive = true;
                }
            }
        }

        /// <summary>
        /// Tells the physics hands to update to match the new leap motion hand frame data.
        /// </summary>
        void UpdatePhysicsHandRepresentations()
        {
            leftActive = false;
            rightActive = false;

            foreach (Leap.Hand curHand in provider.CurrentFrame.Hands)
            {
                if (curHand.IsLeft)
                {
                    leftPhysicsHand.gameObject.SetActive(true);
                    leftPhysicsHand.SetLeapHand(curHand);
                    leftPhysicsHand.UpdateHand();

                    leftActive = true;
                }
                if (curHand.IsRight)
                {
                    rightPhysicsHand.gameObject.SetActive(true);
                    rightPhysicsHand.SetLeapHand(curHand);
                    rightPhysicsHand.UpdateHand();
                    rightActive = true;
                }
            }
            leftPhysicsHand.gameObject.SetActive(leftActive);
            rightPhysicsHand.gameObject.SetActive(rightActive);
        }

        /** Updates the physics objects */
        protected void FixedUpdate()
        {
            if (leftPhysicsHand != null || rightPhysicsHand != null)
            {
                UpdatePhysicsHandRepresentations();
                //UpdateHandModels(hand_physics_, frame.Hands, leftPhysicsModel, rightPhysicsModel);
            }
        }
    }
}
