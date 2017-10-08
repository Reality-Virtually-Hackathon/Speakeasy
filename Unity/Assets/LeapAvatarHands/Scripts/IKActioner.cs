using UnityEngine;
using System.Collections;

namespace LeapAvatarHands
{
#if UNITY_4_0
    public class IKActioner : MonoBehaviour {
	    public IKLeapHandController ikLeapHandController;

	    void Awake()
	    {
		    if(ikLeapHandController == null)
		    {
			    ikLeapHandController = gameObject.GetComponentInChildren<IKLeapHandController>();
		    }
		    if(ikLeapHandController == null)
		    {
			    Debug.LogError("IKActioner:: No IK Leap Hand Controller found. You must set this behaviour up first.");
		    }
	    }
	
	    /**
	     * When an IK pass happens, this happens.
	     * 
	     * This handles the IK for the hands
	     * 
	     * IMPORTANT NOTE: You need to have IK turned on in the animator or this will never happen,
	     * and your hands won't work properly!
	     * */
	    void OnAnimatorIK(){
		    //pass the animatorIK message down to the IKLeapHandController
		    //Debug.Log ("sending message");
		    ikLeapHandController.SendMessage ("OnAnimatorIK");
	    }
    }
#endif
}