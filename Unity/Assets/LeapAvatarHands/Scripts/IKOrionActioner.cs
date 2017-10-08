/**
IKOrionActioner for Leap motion Orion
Forwards the IK pass from the root transform's animator up to the IKOrionLeapHandController

Author: Ivan Bindoff
*/

using UnityEngine;
using System.Collections;

namespace LeapAvatarHands
{

    public class IKOrionActioner : MonoBehaviour
    {
        public IKOrionLeapHandController ikLeapHandController;
        public MatchRotation matchRotation;

        void Awake()
        {
            if (ikLeapHandController == null)
            {
                ikLeapHandController = gameObject.GetComponentInChildren<IKOrionLeapHandController>();
            }
            if (ikLeapHandController == null)
            {
                Debug.LogError("IKOrionActioner:: No IK Leap Hand Controller found. You must set this behaviour up first.");
            }
        }

        /**
         * When an IK pass happens, this happens.
         * 
         * This handles the IK for the hands
         * 
         * IMPORTANT NOTE: You need to have IK pass turned on in the animator or this will never happen,
         * and your hands won't work properly!
         * */
        void OnAnimatorIK()
        {
            //pass the animatorIK message down to the IKLeapHandController
            matchRotation.OnAnimatorIK();
            ikLeapHandController.OnAnimatorIK();
        }
    }
}
