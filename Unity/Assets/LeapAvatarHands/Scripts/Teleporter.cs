/**
Teleportation functions for use with Leap Motion gestures
Set the rayTransform to be your "pointing at" transform (usually the camera, so you teleport to wherever
you're looking). Then set up the mask to only work on your Floor objects (however you've tagged those).

Set up a Leap gesture or other trigger to fire the TeleportTo() method, in the manner of your choosing.

Author: Ivan Bindoff
*/

using UnityEngine;
using System.Collections;
using Leap.Unity;

namespace LeapAvatarHands
{
    public class Teleporter : MonoBehaviour
    {
        public Transform rayTransform;      //the transform from which the ray is cast forwards (typically camera)
        public LayerMask rayMask;           //the layer that the ray can collide with (typically set to floor layer)
        public bool canTeleport = false;    //can we teleport right now? i.e. is the rayTransform currently looking at a surface that matches the rayMask.
        RaycastHit hit;                     //what the ray hit
        protected bool teleporting = false;

        public bool doRotation = true;      //for room scale you will probably want to turn this off, because rotation will be driven by the rotation of the VR headset

        public GameObject teleportIndicatorPrefab;  //the prefab that is drawn where you are about to teleport
        protected GameObject teleportIndicator;     //the instance of that prefab. Keeps track of it to prevent having excessive Instantiate calls.

        void Awake()
        {
            if (rayTransform == null)
            {
                Debug.Log("Teleporter::Awake::No rayTransform set in the inspector. You SHOULD set this manually. Defaulting to camera!");
                rayTransform = Camera.main.transform;
            }
            if (teleportIndicatorPrefab == null)
                Debug.Log("Teleporter::Awake::No teleport indicator prefab assigned in inspector. Player won't be able to see where they're going to teleport to.");
        }
        
        void Update()
        {
            if(rayTransform != null)
            {
                //cast a ray from the ray transform (typically camera)

                if (Physics.Raycast(rayTransform.transform.position, rayTransform.forward, out hit, 100f, rayMask))
                {
                    //if we hit something we can teleport
                    canTeleport = true;
                    if (teleportIndicator == null && teleportIndicatorPrefab != null)
                    {
                        teleportIndicator = GameObject.Instantiate<GameObject>(teleportIndicatorPrefab);
                    }
                    if(teleportIndicator != null)
                        teleportIndicator.transform.position = hit.point;
                }
                else
                {
                    canTeleport = false;
                    if(teleportIndicator != null)
                        Destroy(teleportIndicator);
                }
            }
        }

        /// <summary>
        /// Starts the teleportation process
        /// </summary>
        public void TeleportTo()
        {
            if(canTeleport)
            {
                teleporting = true;
                
                Debug.Log("teleported");
            }
        }


        /// <summary>
        /// Teleportation actually happens in LateUpdate() to help ensure nothing else can interfere 
        /// with the position during the frame processing procedure.
        /// </summary>
        public void LateUpdate()
        {
            if(teleporting)
            {
                //rotate
                if(doRotation)
                    transform.rotation = Quaternion.LookRotation(hit.point - transform.position, Vector3.up);
                
                //teleport
                transform.position = hit.point;
                teleporting = false;
            }
        }
    }
}
