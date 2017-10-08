/**
Matches the rotation and position of the transform this script is on with an external transform.

Intended for use with Neck transforms on avatars that are driven by a VR headset.

Author: Ivan Bindoff
*/

using UnityEngine;
using System.Collections;

namespace LeapAvatarHands
{

    public class MatchRotation : MonoBehaviour
    {
        public Transform rootTransform;     
        public Transform targetTransform;   //i.e. the camera being driven by a VR headset
        public float minYrotation = -30;          //the minimum y rotation the neck can have before it'll start turning the body to compensate
        public float maxYrotation = 30;          //the maximum y rotation the neck can have before it'll start turning the body to compensate
        public float compensationSpeed = 150f;   //the speed with which it will try to compensate
        protected float targetYrot = 0f;        //the y rotation we're aiming for
        public Transform idealMountingPosition; //the ideal mounting position for the camera to sit on the avatar.
        public float maxYdifference = 0.1f;     //the maximum difference between ideal mount position and camera on the Y axis before we decide to bend the avatar
        public Vector3 maxIncrement = new Vector3(1f, 1f, 1f);  //the maximum rotation increment you can try per IK pass for each rotation axis
        public int maxIterations = 5;   //the maximum number of IK iterations you can go through before giving up
        protected Vector3 solvedRotation = Vector3.zero;    //the amount of rotation the IK solver has figured to apply to each spine transform to get us where we want to be
        public bool canMove = true;             //can we move this frame? External scripts can toggle this if they need to take over positioning for a moment.
        public bool spaceBarRecentersCamera = true;

        public void StopMovementForOneFrame()
        {
            canMove = false;
        }

        protected void Awake()
        {
            if(rootTransform == null)
            {
                rootTransform = transform.root;
            }
            if(targetTransform == null)
            {
                Debug.Log(gameObject.name + "::MatchRotation::Awake:: No targetTransform set. Defaulting to Camera.main");
                targetTransform = Camera.main.transform;
            }
            if (idealMountingPosition == null)
                Debug.Log(gameObject.name + "::MatchRotation::Awake:: No idealMountingPosition set. You need to create a transform as a child of the avatar's head that is positioned where the VR camera should ideally be mounted, and set this in the inspector.");
            else
            {
                RecenterCamera();
            }
        }

        /// <summary>
        ///
        /// </summary>
        void FixedUpdate()
        {
            // If the neck is too far turned, turn the body instead
            float normalizedYangle = transform.localRotation.eulerAngles.y;
            if (normalizedYangle > 180f)
                normalizedYangle -= 360f;
            if (normalizedYangle < -180f)
                normalizedYangle += 360f;
            //Debug.Log(transform.localRotation.eulerAngles + " vs " + normalizedYangle);

            if (normalizedYangle < minYrotation)
            {
                //Debug.Log("too small");
                targetYrot = targetTransform.rotation.eulerAngles.y;
                rootTransform.rotation = Quaternion.RotateTowards(rootTransform.rotation, Quaternion.Euler(0, targetYrot, 0), Time.deltaTime * compensationSpeed);
            }

            if (normalizedYangle > maxYrotation)
            {
                //Debug.Log("too big");
                targetYrot = targetTransform.rotation.eulerAngles.y;
                rootTransform.rotation = Quaternion.RotateTowards(rootTransform.rotation, Quaternion.Euler(0, targetYrot, 0), Time.deltaTime * compensationSpeed);
            }

            //figure out where the avatar needs to be positioned to make the camera in the right place
            Vector3 difference = idealMountingPosition.transform.position - targetTransform.position;

            //if the Y difference is too big then the player is presumably leaning over
            //so let's do a bit of fudgey inverse kinematics on the spine to get the difference shrunk
            //if there's still a difference, keep adding bend factor
            if(difference.y > maxYdifference)
                solvedRotation = new Vector3(solvedRotation.x + (Time.deltaTime*maxIncrement.x), 0, 0);
            else if(difference.y < 0 && solvedRotation.x > 0)   //if it's gone the other way, unbend
                solvedRotation = new Vector3(solvedRotation.x - (Time.deltaTime * maxIncrement.x), 0, 0);

            if (solvedRotation != Vector3.zero)
                ApplyRotationToSpine(solvedRotation);

            difference = idealMountingPosition.transform.position - targetTransform.position;


            //physically move the avatar to adjust for the difference
            if (canMove)
                rootTransform.position = new Vector3(rootTransform.position.x - difference.x, rootTransform.position.y, rootTransform.position.z - difference.z);
            else
                canMove = true;
        }

        //happens after animation, so will override any neck animation data
        void LateUpdate()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                RecenterCamera();   //particularly useful for if you launch editor while sitting down or headset on table, then stand up...
            }

            if(solvedRotation != Vector3.zero)
                ApplyRotationToSpine(solvedRotation);
            
            //turn this neck transform to look in the same direction as the VR headset
            if (targetTransform != null)
            {
                transform.rotation = Quaternion.LookRotation(targetTransform.forward, Vector3.up);
            }
        }

        //TODO my working theory is that the spine bending rotations aren't applied as far as the IK solver is concerned.
        //either because it gets overridden by the animation just before the IK happens
        //or because the IK solver makes some goofy assumptions.
        //One plan is to make a bending animation and blend through that. Another is to just fix this shit sdmehow
        public void Update()
        {
            if (solvedRotation != Vector3.zero)
                ApplyRotationToSpine(solvedRotation);
        }


        public void OnAnimatorIK()
        {
            if (solvedRotation != Vector3.zero)
                ApplyRotationToSpine(solvedRotation);
        }
        

        /// <summary>
        /// Resets the camera to be in the ideal mounting position.
        /// </summary>
        void RecenterCamera()
        {
            targetTransform.position = idealMountingPosition.position;
            targetTransform.rotation = idealMountingPosition.rotation;
            UnityEngine.VR.InputTracking.Recenter();
        }
        
        protected void ApplyRotationToSpine(Vector3 chosenRotation)
        {
            //traverse all the way back down from this neck transform to hips transform
            Transform current = transform.parent;
            while (current != transform.root && !current.name.Contains("Hips"))
            {
                current.transform.localRotation = Quaternion.Euler(chosenRotation);
                current = current.parent;
            }
        }
    }
}
