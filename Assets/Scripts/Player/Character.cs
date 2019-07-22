using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rigid;
    private CapsuleCollider col;

    // state inputs
    private bool jumpInput;
    private bool moveInputActive;
    private bool isGrounded;
    private bool isRunning;
    private bool isJumping;
    private bool isWalking;
    private bool isPushing;
    private bool isCovered;

    private float turnAroundInput;
    private float moveInputForward;
    private float moveInputSide;
    private float crouchInput;

    [Header("Movement and Interactions")]
    [Space(10f)]
    [SerializeField]
    private bool rootMotion = false;
    [Range(0.1f, 30f)]
    [SerializeField]
    private float walkSpeed = 1.0f;
    [Range(0.1f, 30f)]
    [SerializeField]
    private float runSpeed = 1.0f;
    [Range(0.1f, 30f)]
    [SerializeField]
    private float pushSpeed = 1.0f;
    [Range(0.1f, 2f)]
    [SerializeField]
    private float pushDistance = 1.0f;
    [Range(0.1f, 30f)]
    [SerializeField]
    private float crouchSpeed = 0.8f;
    [Range(0f, 30f)]
    [SerializeField]
    private float crouchColliderShrink = 0.3f;
    [Range(0f, 30f)]
    [SerializeField]
    private float crouchColliderCenterY = 0.7f;
    [Range(0.1f, 200f)]
    [SerializeField]
    private float turnSpeed = 100.0f;
    [Range(0.1f, 50f)]
    [SerializeField]
    private float gravity = 10.0f;
    [Range(0.1f, 50f)]
    [SerializeField]
    private float maxVelocityChange = 10.0f;
    [Range(0.1f, 5f)]
    [SerializeField]
    private float jumpHeight = 2.0f;

    // grounded attributes
    private Vector3 charBottomPoint;
    private Vector3 charBottomPoint2;
    private Vector3 charBottomPoint3;
    private Vector3 charBottomPoint4;
    private Vector3 charBottomPoint5;
    private int groundLayerMask = 1 << 8;
    private int pushLayerMask = 1 << 9;

    // character collider attributes
    private float colliderExtendX;
    private float colliderExtendZ;
    private float colliderExtendY;
    private float collHeight;
    private Vector3 collCenter;

    // pushing IK attributes
    private bool ikActive = false;
    private RaycastHit lastPushHit;

    [Header("Abilities")]
    [Space(20f)]    
    [SerializeField]
    private bool canJump = true;
    [SerializeField]
    private bool canCrouch = true;
    [SerializeField]
    private bool canRun = true;

    private void Awake()
    {
        groundLayerMask = ~groundLayerMask; // invert layermask
        animator = GetComponentInChildren<Animator>();
        rigid = GetComponentInChildren<Rigidbody>();
        col = GetComponentInChildren<CapsuleCollider>();
        rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
        rigid.useGravity = false;

        WalkSpeed = walkSpeed;
        CrouchSpeed = crouchSpeed;
        RunSpeed = runSpeed;
        PushSpeed = pushSpeed;

        colliderExtendX = col.bounds.extents.x - 0.2f;
        colliderExtendZ = col.bounds.extents.z - 0.2f;
        colliderExtendY = col.bounds.extents.y;
        collHeight = col.height;
        collCenter = col.center;
    }

    public bool IsGrounded
    {
        get
        {
            return isGrounded;
        }
        private set
        {
            isGrounded = value;
            if (animator != null) animator.SetBool("Grounded", value);
        }
    }

    public void SetMoveInput(float _forward, float _side, bool _inputIsActive)
    {
        moveInputForward = _forward;
        moveInputSide = _side;
        animator.SetFloat("InputMove", moveInputForward + moveInputSide);
        moveInputActive = _inputIsActive;
    }

    public bool IsRunning
    {
        get { return isRunning; }
        set
        {
            if (canRun && CrouchInput >= 0)
            {
                isRunning = value;
                if (animator != null) animator.SetBool("Running", value);
            }
        }
    }

    public bool IsJumping
    {
        get { return isJumping; }
        set
        {
            if (canJump)
            {
                isJumping = value;
                if (animator != null) animator.SetBool("Jump", value);
            }
        }
    }

    public bool IsWalking
    {
        get { return isWalking; }
        private set
        {
            isWalking = value;
            if (animator != null) animator.SetBool("OnWalk", value);
        }
    }

    public bool IsCovered
    {
        get { return isCovered; }
        private set
        {
            isCovered = value;
        }
    }

    public bool IsPushing
    {
        get
        {
            return isPushing && animator.GetCurrentAnimatorStateInfo(0).IsName("Pushing");
        }
        private set
        {
            isPushing = value;
            if (animator != null) animator.SetBool("IsPushing", value);
        }
    }

    public float CrouchInput {
        get { return crouchInput;  }
        set
        {
            if (canCrouch)
            {
                if (!IsCovered)
                {
                    crouchInput = value;
                    animator.SetFloat("InputCrouch", value);
                }

                if (crouchInput < 0)
                {
                    col.height = collHeight - crouchColliderShrink;
                    col.center = new Vector3(col.center.x, crouchColliderCenterY, col.center.z);
                } else
                {
                    col.height = collHeight;
                    col.center = collCenter;
                }
            }            
        }
    }

    public float WalkSpeed
    {
        get { return walkSpeed; }
        set
        {
            walkSpeed = value;
            if (rootMotion) animator.SetFloat("WalkSpeed", value);
            else animator.SetFloat("WalkSpeed", 1f);
        }
    }

    public float RunSpeed
    {
        get { return walkSpeed; }
        set
        {
            runSpeed = value;
            if (rootMotion) animator.SetFloat("RunSpeed", value);
            else animator.SetFloat("RunSpeed", 1f);
        }
    }

    public float PushSpeed
    {
        get { return pushSpeed; }
        set
        {
            pushSpeed = value;
            if (rootMotion) animator.SetFloat("PushSpeed", value);
            else animator.SetFloat("PushSpeed", 1f);
        }
    }

    public float CrouchSpeed
    {
        get { return crouchSpeed; }
        set
        {
            crouchSpeed = value;
            if (rootMotion) animator.SetFloat("CrouchSpeed", value);
            else animator.SetFloat("CrouchSpeed", 1f);
        }
    }

    public void OnAnimatorMove()
    {
        if (animator != null) transform.position += animator.deltaPosition;
    }

    public void TriggerJump()
    {
        jumpInput = true;
    }

    void FixedUpdate()
    {
        // set grounded raycasts start points
        Vector3 colliderCenter = col.bounds.center;
        charBottomPoint = new Vector3(colliderCenter.x, colliderCenter.y, transform.position.z);
        charBottomPoint2 = new Vector3(colliderCenter.x + colliderExtendX, colliderCenter.y, colliderCenter.z + colliderExtendZ);
        charBottomPoint3 = new Vector3(colliderCenter.x - colliderExtendX, colliderCenter.y, colliderCenter.z - colliderExtendZ);
        charBottomPoint4 = new Vector3(colliderCenter.x + colliderExtendX, colliderCenter.y, colliderCenter.z - colliderExtendZ);
        charBottomPoint5 = new Vector3(colliderCenter.x - colliderExtendX, colliderCenter.y, colliderCenter.z + colliderExtendZ);

        if (animator == null) return;
        if (IsGrounded)
        {
            animator.applyRootMotion = true;

            CheckPushingRange();
            if (CrouchInput < 0) CheckHeadCover();

            Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
            Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up);
            camForward = camForward.normalized;            
            camRight = camRight.normalized;

            // Calculate how fast we should be moving
            Vector3 targetVelocity = new Vector3(moveInputSide, 0, moveInputForward);
            targetVelocity = targetVelocity.z * camForward + targetVelocity.x * camRight;

            // rotate character
            if (moveInputActive)
            {
                float turnSpeed = Time.deltaTime * this.turnSpeed;
                Quaternion newRotation = Quaternion.LookRotation(targetVelocity);
                newRotation.x = 0f;
                newRotation.z = 0f;
                transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, turnSpeed);
            }

            Vector3 velocity = rigid.velocity;
            if (crouchInput < 0) targetVelocity *= CrouchSpeed;
            else if (IsRunning) targetVelocity *= RunSpeed;
            else if (IsPushing) targetVelocity *= PushSpeed;
            else targetVelocity *= WalkSpeed;

            // Apply a force that attempts to reach our target velocity
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);

            // add force to move character
            if (!rootMotion) rigid.AddForce(velocityChange, ForceMode.VelocityChange);

            // Jump
            if (canJump && jumpInput && !IsJumping)
            {
                animator.applyRootMotion = false;

                // add force to move character
                if (rootMotion) rigid.AddForce(velocityChange, ForceMode.Acceleration);

                IsJumping = true;
                jumpInput = false;
                rigid.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
                IsGrounded = false;
            }

            // update walk input state
            IsWalking = moveInputActive;
        }

        // apply gravity manually for more tuning control
        rigid.AddForce(new Vector3(0, -gravity * rigid.mass, 0));
    }

    private void OnCollisionEnter(Collision _collision)
    {
        // detect if grounded
        RaycastHit hit;
        bool hitted = Physics.Raycast(charBottomPoint, Vector3.down, out hit, (colliderExtendY + 0.1f));
        if (!hitted) hitted = Physics.Raycast(charBottomPoint2, Vector3.down, out hit, (colliderExtendY + 0.1f));
        if (!hitted) hitted = Physics.Raycast(charBottomPoint3, Vector3.down, out hit, (colliderExtendY + 0.1f));
        if (!hitted) hitted = Physics.Raycast(charBottomPoint4, Vector3.down, out hit, (colliderExtendY + 0.1f));
        if (!hitted) hitted = Physics.Raycast(charBottomPoint5, Vector3.down, out hit, (colliderExtendY + 0.1f));
        IsGrounded = hitted;
        if (IsGrounded) IsJumping = false;
    }

    private void OnCollisionExit(Collision _collision)
    {
        // detect if still grounded after collider exit, while not jumping
        if (!IsJumping)
        {
            RaycastHit hit;
            bool hitted = Physics.Raycast(charBottomPoint, Vector3.down, out hit, (colliderExtendY + 0.1f));
            if (!hitted) hitted = Physics.Raycast(charBottomPoint2, Vector3.down, out hit, (colliderExtendY + 0.1f));
            if (!hitted) hitted = Physics.Raycast(charBottomPoint3, Vector3.down, out hit, (colliderExtendY + 0.1f));
            if (!hitted) hitted = Physics.Raycast(charBottomPoint4, Vector3.down, out hit, (colliderExtendY + 0.1f));
            if (!hitted) hitted = Physics.Raycast(charBottomPoint5, Vector3.down, out hit, (colliderExtendY + 0.1f));
            IsGrounded = hitted;
            if (IsGrounded) IsJumping = false;
        }
    }

    private void CheckPushingRange()
    {
        RaycastHit hit;
        if (Physics.Raycast(col.bounds.center, transform.forward, out hit, pushDistance, pushLayerMask))
        {
            if (hit.collider.isTrigger && hit.rigidbody != null && IsWalking)
            {
                IsPushing = true;
                lastPushHit = hit;

                float objColliderHigh = hit.collider.bounds.size.y;
                float charColliderHigh = col.bounds.size.y;
                float charColliderHipHigh = col.center.y;

                float animationState = 0f;                
                if (objColliderHigh <= charColliderHipHigh) animationState = 0f;
                else if (objColliderHigh >= charColliderHigh) animationState = 1f;
                else animationState = objColliderHigh / charColliderHigh;

                // enable IK
                ikActive = true;

                // set push animation
                if (animator != null) animator.SetFloat("MovingObjHigh", animationState);
                if(IsPushing) lastPushHit.rigidbody.isKinematic = false;
            } else
            {
                ikActive = false;
            }
        } else
        {
            if (lastPushHit.rigidbody != null) lastPushHit.rigidbody.isKinematic = true;
            IsPushing = false;
            ikActive = false;
        }
    }

    private void CheckHeadCover()
    {
        RaycastHit hit;
        bool hitted = Physics.Raycast(charBottomPoint, Vector3.up, out hit, (colliderExtendY + 0.1f));
        if (!hitted) hitted = Physics.Raycast(charBottomPoint2, Vector3.up, out hit, (colliderExtendY + 0.1f));
        if (!hitted) hitted = Physics.Raycast(charBottomPoint3, Vector3.up, out hit, (colliderExtendY + 0.1f));
        if (!hitted) hitted = Physics.Raycast(charBottomPoint4, Vector3.up, out hit, (colliderExtendY + 0.1f));
        if (!hitted) hitted = Physics.Raycast(charBottomPoint5, Vector3.up, out hit, (colliderExtendY + 0.1f));
        IsCovered = hitted;
    }

    float CalculateJumpVerticalSpeed()
    {
        // From the jump height and gravity deduce the upwards speed 
        // for the character to reach at the apex.
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    // callback for calculating IK
    private void OnAnimatorIK()
    {
        if (animator)
        {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {
                // get objects collider top position
                Bounds colBounds = lastPushHit.collider.bounds;
                float anchorHigh = colBounds.center.y + colBounds.extents.y;

                // get characters bone transforms
                Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
                Transform leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
                Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
                
                if (anchorHigh > head.position.y) anchorHigh = head.position.y;

                Vector3 topPosRight = new Vector3(rightHand.position.x, anchorHigh, rightHand.position.z);
                Vector3 topPosLeft = new Vector3(leftHand.position.x, anchorHigh, leftHand.position.z);

                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                animator.SetIKPosition(AvatarIKGoal.RightHand, topPosRight);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, topPosLeft);
            }

            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            }
        }
    }
}
