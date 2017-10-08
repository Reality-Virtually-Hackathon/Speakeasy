using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

/**
 * An extension for the LEAP Hand Controller

    NOTE: this has been superseded by the Orion version of this asset. Included only for reference for
    users on legacy Unity 4.x using the V2 version of Leap Motion.
 * 
 * This extension changes the behaviour of the LEAP hand controller
 * so that it attempts to drive a rigged Unity humanoid avatar's hands
 * rather than instantiating its own hands.
 * 
 * It uses Unity's inverse kinematics system to position the avatar's hands
 * where they should be.
 * 
 * Requires: Unity PRO 4.5+, LEAP 2.0
 * 
 * Author: Ivan Bindoff (@vanmani)
 * Some parts adapted from HandController provided LEAP Unity asset.
 * */
namespace LeapAvatarHands
{
#if UNITY_4_0
    public class IKLeapHandController : HandController {
	    public bool ikActive = true;

	    public Transform spawnPoint;		//the Transform at which the hands should spawn from.
	
	    public GameObject avatarLeftHand;	//keeps track of the left hand gameobject for IK
	    public GameObject avatarRightHand;	//keeps track of the right hand gameobject for IK
	    public bool leftActive = false;		//keep track of whether the LEAP hand is active
	    public bool rightActive = false;	//keep track of whether the LEAP hand is active

	    [HideInInspector] public Vector3 lTarget;				//where we want the left hand to be in worldspace
	    [HideInInspector] public Vector3 rTarget; 				//where we want the right hand to be in worldspace
	    [HideInInspector] public Quaternion lTargetRot;			//the rotation we want for left hand
	    [HideInInspector] public Quaternion rTargetRot;			//the rotation we want for the right hand
	
	    private GameObject rootNode;							//the root, parent, gameobject that this script ultimately lives on.
		
	    protected Animator animator;
	
	    private float lWeight = 0f;			//how much IK to apply to the left hand
	    private float rWeight = 0f;			//how much IK to apply to the right hand

	    private bool show_hands__ = true;
	    //private long prev_graphics_id__ = 0;
	    private long prev_physics_id__ = 0;
	
	    protected float leftInactiveWeight = 0f;
	    protected float rightInactiveWeight = 0f;
	    public float inactiveLerpTime = 0.2f;	//how many seconds it takes to lerp back to default pose after a hand goes inactive
	
	    /**
	     * Set up references
	     * */
	    void Awake()
	    {
		    leap_controller_ = new Controller();

		    animator = GetComponent<Animator>();
		    Transform t = this.transform;
		    while(t != null)
		    {
			    if(t.gameObject.GetComponent<Animator>())
			    {
				    animator = t.gameObject.GetComponent<Animator>();
				    rootNode = t.gameObject;
				    break;
			    }
			    t = t.parent;
		    }
		    if(animator == null)
		    {
			    Debug.LogError ("IKLeapHandController:: no animator found on GameObject or its parents. Are you sure you added IKLeapHandController to a properly defined rigged avatar?");
		    }
		    if(!avatarLeftHand || !avatarRightHand)
		    {
			    Debug.LogError ("IKLeapHandController:: avatar hands not defined. Do this in the inspector first.");
		    }

		    if(leftGraphicsModel != null || rightGraphicsModel != null)
		    {
			    Debug.Log ("IKLeapHandController:: INFO you've set a left/right graphics hand model, but there's no point, the script will override it. You only need to set the Avatar Left Hand and Avatar Right Hand fields.");
		    }
		
		    //get the handmodel for the graphics hands from the avatar hand
		    leftGraphicsModel = avatarLeftHand.GetComponent<HandModel>();
		    rightGraphicsModel = avatarRightHand.GetComponent<HandModel>();

		    if(!leftGraphicsModel || !rightGraphicsModel)
		    {
			    Debug.LogError ("IKLeapHAndController:: the avatar's hands must have a valid HandModel defined on them. Do this in the inspector first.");
		    }
		
		    //destroy hands setting is ignored
		    if(destroyHands == false)
		    {
			    Debug.Log ("IKLeapHandController:: INFO: Destroy Hands setting will be ignored. RigidIKHands will always be destroyed, avatar hands will never be destroyed.");
		    }

		    if(spawnPoint == null)
		    {
			    spawnPoint = transform;
		    }
	    }

	    /**
	     * Deal with the graphical representation of the hands in here
	     * That means the avatar's hands, but not the separate physics hand model
	     * which can be dealt with in FixedUpdate where it belongs.
	     * */
	    void Update() {
		    if (leap_controller_ == null)
			    return;
		
		    UpdateRecorder();
		    Frame frame = GetFrame();
		
		    if (frame != null)
		    {
			    InitializeFlags();
		    }
		
		    if (show_hands__)
		    {
			    /*if (frame.Id != prev_graphics_id__)
			    {*/
				    UpdateHandModels(hand_graphics_, frame.Hands, leftGraphicsModel, rightGraphicsModel);
				    //prev_graphics_id__ = frame.Id;
			    //}
		    }
	    }

	    /**
	     * Deal with the physics hands in here
	     * Physics hands overlay the graphical representation of the hand
	     * but are otherwise invisible in game view.
	     * */
	    void FixedUpdate() {
		    if (leap_controller_ == null)
			    return;
		
		    Frame frame = GetFrame();
		    if (frame != null)
		    {
			    InitializeFlags();
		    }
		
		    if (frame.Id != prev_physics_id__)
		    {
			    UpdatePhysicsHandModels(hand_physics_, frame.Hands, leftPhysicsModel, rightPhysicsModel);
			    UpdateToolModels(tools_, frame.Tools, toolModel);
			    prev_physics_id__ = frame.Id;
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
	    void OnAnimatorIK(){
		    //Debug.Log ("Receiving message");
		    //if we're using the leap controller then do the animation IK override
		    if(animator) 
		    {
			    //if the IK is active, set the position and rotation directly to the goal. 
			    if(ikActive) {
				    //find the LEAP hands and set them as the target
				    setHandTargets();
				
				
				    //set the position and the rotation of the left hand where the leap hand is
				    if(leftActive && leftGraphicsModel.GetLeapHand () != null) {
					    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,lWeight);
					    animator.SetIKPosition(AvatarIKGoal.LeftHand,lTarget);
					    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,lWeight);
					    animator.SetIKRotation (AvatarIKGoal.LeftHand,lTargetRot);
					    leftInactiveWeight = 1f;
				    } 
				    else
				    {
					    //gradually lerp away from the target, returning hand back to normal operation but without a jarring effect
					    leftInactiveWeight -= Time.deltaTime/inactiveLerpTime;
					    if(leftInactiveWeight < 0f)
					    {
						    leftInactiveWeight = 0f;
						    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,leftInactiveWeight);
						    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,leftInactiveWeight);
					    }
					    else
					    {
						    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,leftInactiveWeight);
						    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,0f);
						    animator.SetIKPosition(AvatarIKGoal.LeftHand,lTarget);
						    animator.SetIKRotation (AvatarIKGoal.LeftHand,lTargetRot);
					    }
				    }
				
				    //set the position and the rotation of the right hand where the leap hand is
				    if(rightActive && rightGraphicsModel.GetLeapHand () != null) {
					    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,rWeight);
					    animator.SetIKPosition(AvatarIKGoal.RightHand,rTarget);
					    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,rWeight);
					    animator.SetIKRotation (AvatarIKGoal.RightHand,rTargetRot);
					    rightInactiveWeight = 1f;
				    } 
				    else
				    {
					    //gradually lerp away from the target, returning hand back to normal operation
					    rightInactiveWeight -= Time.deltaTime/inactiveLerpTime;
					    if(rightInactiveWeight < 0f)
					    {
						    rightInactiveWeight = 0f;
						    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,rightInactiveWeight);
						    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,rightInactiveWeight);
					    }
					    else
					    {
						    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,rightInactiveWeight);
						    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,0f);
						    animator.SetIKPosition(AvatarIKGoal.RightHand,rTarget);
						    animator.SetIKRotation (AvatarIKGoal.RightHand,rTargetRot);
					    }	
				    }
			    }
			    else 
			    {
				    //IK not active, resume normal operation
				    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,0);
				    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand,0);
				    animator.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
				    animator.SetIKRotationWeight(AvatarIKGoal.RightHand,0);    	
			    }
		    }
	    }

	    /**
	     * Set the current Leap hands as the IK targets
	     * 
	     * */
	    private void setHandTargets()
	    {
		    if(leftActive && leftGraphicsModel.GetLeapHand () != null)
		    {
			    lTarget = leftGraphicsModel.GetWristPosition();
			    lTargetRot = leftGraphicsModel.GetPalmRotation();
			    lWeight = 1f;
			    //Debug.Log ("left hand target set to " + lTarget);
		    }

		    if(rightActive && rightGraphicsModel.GetLeapHand () != null)
		    {
			    rTarget = rightGraphicsModel.GetWristPosition();
			    rTargetRot = rightGraphicsModel.GetPalmRotation ();
			    rWeight = 1f;
		    }
	    }

	    /**
	     * Updates the graphics hands so that they're drawn properly
	     * */
	    protected new void UpdateHandModels(Dictionary<int, HandModel> all_hands,
	                                    HandList leap_hands,
	                                    HandModel left_model, HandModel right_model) {
		    List<int> ids_to_check = new List<int>(all_hands.Keys);
		
		    // Go through all the active hands in the Leap motion frame and update them.
		    int num_hands = leap_hands.Count;
		    for (int h = 0; h < num_hands; ++h) {
			    Hand leap_hand = leap_hands[h];
			
			    HandModel model = (mirrorZAxis != leap_hand.IsLeft) ? left_model : right_model;

			    // Only create or update if the hand is enabled.
			    if (model != null) {
				    ids_to_check.Remove(leap_hand.Id);	//this prevents the hand from being made inactive, since it is active.
				
				    // Create the hand and initialized it if it doesn't exist yet.
				    if (!all_hands.ContainsKey(leap_hand.Id)) {
					    HandModel new_hand = InitializeHand(model);	//We don't instantiate these ones - they're already on the avatar, so we just init them instead of CreateHand them
					    new_hand.SetLeapHand(leap_hand);
					    new_hand.MirrorZAxis(mirrorZAxis);
					    new_hand.SetController(this);
					
					    // Set scaling based on reference hand.
					    float hand_scale = MM_TO_M * leap_hand.PalmWidth / new_hand.handModelPalmWidth;
					    new_hand.transform.localScale = hand_scale * transform.lossyScale;
					
					    new_hand.InitHand();
					    new_hand.UpdateHand();
					    all_hands[leap_hand.Id] = new_hand;

					    if(new_hand.GetLeapHand ().IsLeft)
						    leftActive = true;
					    else
						    rightActive = true;
					    //Debug.Log ("Initialised graphics hand and added to active hands list, now contains " + all_hands.Count + " hands");
				    }
				    else {
					    // Make sure we update the Leap Hand reference.
					    HandModel hand_model = all_hands[leap_hand.Id];
					    hand_model.SetLeapHand(leap_hand);
					    hand_model.MirrorZAxis(mirrorZAxis);
					
					    // Set scaling based on reference hand.
					    float hand_scale = MM_TO_M * leap_hand.PalmWidth / hand_model.handModelPalmWidth;
					    hand_model.transform.localScale = hand_scale * transform.lossyScale;
					    hand_model.UpdateHand();

					    if(hand_model.GetLeapHand ().IsLeft)
						    leftActive = true;
					    else
						    rightActive = true;
					    //Debug.Log ("Updated graphics hand");
				    }
			    }
		    }

		    // flag hands that are no longer active, so IK knows to turn off
		    for (int i = 0; i < ids_to_check.Count; ++i) {
			    if(all_hands[ids_to_check[i]].GetLeapHand ().IsLeft)
				    leftActive = false;
			    else
				    rightActive = false;
			    all_hands.Remove(ids_to_check[i]);
		    }
	    }

	    /**
	     * Doesn't spawn a hand, just initialises it
	     * */
	    protected HandModel InitializeHand(HandModel model) {
		    Leap.Utils.IgnoreCollisions(model.gameObject, gameObject);
		    return model;
	    }

	    /**
	     * Used to spawn a physics hand
	     * */
	    protected HandModel CreateHand(HandModel model, bool isLeft) {
		    RigidIKHand hand_model = Instantiate(model, spawnPoint.position, spawnPoint.rotation)
			    as RigidIKHand;
		    hand_model.gameObject.SetActive(true);
		    if(isLeft)
			    hand_model.targetHand = leftGraphicsModel;
		    else
			    hand_model.targetHand = rightGraphicsModel;
		    Leap.Utils.IgnoreCollisions(hand_model.gameObject, rootNode);
		    return hand_model;
	    }

	    /**
	     * Makes sure the physics hands are instantiated or updated as required
	     * */
	    protected void UpdatePhysicsHandModels(Dictionary<int, HandModel> all_hands,
	                                    HandList leap_hands,
	                                    HandModel left_model, HandModel right_model) {
		    List<int> ids_to_check = new List<int>(all_hands.Keys);

		    // Go through all the active hands and update them.
		    int num_hands = leap_hands.Count;
		    for (int h = 0; h < num_hands; ++h) {
			    Hand leap_hand = leap_hands[h];
			
			    HandModel model = (mirrorZAxis != leap_hand.IsLeft) ? left_model : right_model;
			
			    // If we've mirrored since this hand was updated, destroy it.
			    if (all_hands.ContainsKey(leap_hand.Id) &&
			        all_hands[leap_hand.Id].IsMirrored() != mirrorZAxis) {
				    DestroyHand(all_hands[leap_hand.Id]);
				    all_hands.Remove(leap_hand.Id);
			    }
			
			    // Only create or update if the hand is enabled.
			    if (model != null) {
				    ids_to_check.Remove(leap_hand.Id);
				    // Create the hand and initialized it if it doesn't exist yet.
				    if (!all_hands.ContainsKey(leap_hand.Id)) {
					    HandModel new_hand = CreateHand(model, leap_hand.IsLeft);
					    new_hand.SetLeapHand(leap_hand);
					    new_hand.MirrorZAxis(mirrorZAxis);
					    new_hand.SetController(this);
					
					    // Set scaling based on reference hand.
					    float hand_scale = MM_TO_M * leap_hand.PalmWidth / new_hand.handModelPalmWidth;
					    new_hand.transform.localScale = hand_scale * transform.lossyScale;
					
					    all_hands[leap_hand.Id] = new_hand;	//bugfix: adding to list before init/update, because if it errors during that process then it'll never get into the list and thus never be removed...
					    new_hand.InitHand();
					    new_hand.UpdateHand();
										
					    //Debug.Log ("Added " + leap_hand.Id + " to physics list");
					    //Debug.Log ("Created a physics hand and added to active physics hands list, now contains " + all_hands.Count + " hands");
				    }
				    else {
					    //Debug.Log ("Updated a physics hand");
					    // Make sure we update the Leap Hand reference.
					    HandModel hand_model = all_hands[leap_hand.Id];
					    hand_model.SetLeapHand(leap_hand);
					    hand_model.MirrorZAxis(mirrorZAxis);
					
					    // Set scaling based on reference hand.
					    float hand_scale = MM_TO_M * leap_hand.PalmWidth / hand_model.handModelPalmWidth;
					    hand_model.transform.localScale = hand_scale * transform.lossyScale;
					    hand_model.UpdateHand();
				    }
			    }
		    }
		
		    // Destroy all hands with defunct IDs.
		    for (int i = 0; i < ids_to_check.Count; ++i) {
			    DestroyHand(all_hands[ids_to_check[i]]);
			    //Debug.Log ("Destroyed " + ids_to_check[i] + " from physics list because it was defunct");
			    all_hands.Remove(ids_to_check[i]);
		    }
	    }

	    /**
	     * Setup the flags
	     * */
	    void InitializeFlags()
	    {
		    // Optimize for top-down tracking if on head mounted display.
		    Controller.PolicyFlag policy_flags = leap_controller_.PolicyFlags;
		    if (isHeadMounted)
			    policy_flags |= Controller.PolicyFlag.POLICY_OPTIMIZE_HMD;
		    else
			    policy_flags &= ~Controller.PolicyFlag.POLICY_OPTIMIZE_HMD;
		
		    leap_controller_.SetPolicyFlags(policy_flags);
		    //flag_initialized_ = true;
	    }

	    protected new ToolModel CreateTool(ToolModel model) {
		    ToolModel tool_model = Instantiate(model, spawnPoint.position, spawnPoint.rotation) as ToolModel;
		    tool_model.gameObject.SetActive(true);
		    Leap.Utils.IgnoreCollisions(tool_model.gameObject, gameObject);
		    return tool_model;
	    }

	    void OnDrawGizmos() {
		    // Draws the little Leap Motion Controller in the Editor view.
		    if(spawnPoint != null)
		    {
			    Gizmos.matrix = Matrix4x4.Scale(GIZMO_SCALE * Vector3.one);
			    Gizmos.DrawIcon(spawnPoint.position, "leap_motion.png");
		    }
	    }
	
	    /**
	    RigidIKHands will always be destroyed (ignores the destroy hands bool because leaving colliders randomly littering the scene is dumb)
	    The avatar's hands should never be destroyed.
	    */
	    protected new void DestroyHand(HandModel hand_model) {
		    Destroy(hand_model.gameObject);
	    }
    }
#endif
}
