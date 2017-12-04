using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SoccerGame
{
    public enum LocomotionType
    {
        normal,
        soccer,
        strafe,
        air
    }
    [System.Serializable]
    public class ControllerLocomotion
    {


        public LocomotionType motionType;

        [SerializeField]
        private float m_MovingTurnSpeed = 360;
        [SerializeField]
        private float m_StationaryTurnSpeed = 180;
        [SerializeField]
        private float speedDampTime = 0.1f;
        [SerializeField]
        private float anguarSpeedDampTime = 0.25f;
        [SerializeField]
        private float directionResponseTime = 0.2f;
        [SerializeField]
        private float animatorSpeedDamp = 5.5f;
        [SerializeField]
        private float animSpeedMultiplier = 1.0f;

        [SerializeField]
        private bool useExtraRotation = false;

        public bool inIdle { get { return m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"); } }
        public bool inWalkRun { get { return m_Animator.GetCurrentAnimatorStateInfo(0).IsName("WalkRun"); } }
        public bool inEntry { get { return m_Animator.GetCurrentAnimatorStateInfo(1).IsName("Entry"); } }
        public bool inStumble { get { return m_Animator.GetCurrentAnimatorStateInfo(2).IsName("Stumble"); } }
        public bool inKick { get { return m_Animator.GetCurrentAnimatorStateInfo(0).IsName("LongKick"); } }
        public bool inPass { get { return m_Animator.GetCurrentAnimatorStateInfo(0).IsName("ShortPass"); } }
        public bool inTrip
        {
            get
            {
                AnimatorStateInfo state = m_Animator.GetCurrentAnimatorStateInfo(0);

                bool result =
                    state.IsName("Trip.Trip") ||
                    state.IsName("Trip.Lay") ||
                    state.IsName("Trip.Stand");
                return result;
            }
        }
        public bool inTrack { get { return m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Track"); } }
        public bool inNormal { get { return inStrafe == false && inSoccer == false; } }
        public bool inStrafe { get { return motionType == LocomotionType.strafe; } }
        public bool inSoccer { get { return motionType == LocomotionType.soccer; } }
        public bool inTurn
        {
            get
            {
                AnimatorStateInfo state = m_Animator.GetCurrentAnimatorStateInfo(0);

                bool result =
                    state.IsName("Locomotion.TurnOnSpot") ||
                    state.IsName("Locomotion.PlantNTurnLeft") ||
                    state.IsName("Locomotion.PlantNTurnRight");
                return result;
            }
        }
       

        private Animator m_Animator = null;
        private NavMeshAgent m_Agent = null;
        private Rigidbody m_Rigidbody = null;
        private ControllerGravity m_Gravity = null;
        private PlayerController m_controller = null;
        private bool m_ai = false;

        private int m_SpeedId = 0;
        private int m_AgularSpeedId = 0;
        private int m_DirectionId = 0;
        private int m_GroundedId = 0;
        private int m_JumpId = 0;
        private int m_JumpLegCycleId = 0;
        private int m_KickId = 0;
        private int m_PassId = 0;
        private int m_EntryId = 0;
        private int m_StumbleId = 0;
        private int m_OwnerBall = 0;
        private int m_Trip = 0;

        public void Start(PlayerController controller, ControllerGravity gravity)
        {
            m_controller = controller;
            m_Animator = m_controller.transform.GetComponent<Animator>();
            m_Rigidbody = m_controller.transform.GetComponent<Rigidbody>();
            m_Agent = m_controller.transform.GetComponent<NavMeshAgent>();
            m_Gravity = gravity;

            m_SpeedId = Animator.StringToHash("Speed");
            m_AgularSpeedId = Animator.StringToHash("AngularSpeed");
            m_DirectionId = Animator.StringToHash("Direction");
            m_GroundedId = Animator.StringToHash("OnGround");
            m_JumpId = Animator.StringToHash("Jump");
            m_JumpLegCycleId = Animator.StringToHash("JumpLeg");
            m_KickId = Animator.StringToHash("LongKick");
            m_PassId = Animator.StringToHash("ShortPass");
            m_EntryId = Animator.StringToHash("Entry");
            m_StumbleId = Animator.StringToHash("Stumble");
            m_OwnerBall = Animator.StringToHash("OwnerBall");
            m_Trip = Animator.StringToHash("Trip");


        }
        public void DoAnimator(float speed, float direction, bool ownerBall, bool isAI)
        {
            if (inStumble || inTrip || inTrack)
                return;

            m_ai = isAI;

            bool inTransition = m_Animator.IsInTransition(0);
            AnimatorStateInfo state = m_Animator.GetCurrentAnimatorStateInfo(0);

            float runCycleLeg = m_Gravity.CycleLefOffset;
            bool isGrounded = m_Gravity.IsGrounded;

            inTransition = m_Animator.IsInTransition(0);

            float speedDampTime = inIdle ? 0 : this.speedDampTime;
            float angularSpeedDampTime = inWalkRun || inTransition ? anguarSpeedDampTime : 0;
            float directionDampTime = inTurn || inTransition ? 1000000 : 0;

            float angularSpeed = direction / directionResponseTime;

            // calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            float runCycle = Mathf.Repeat(state.normalizedTime + runCycleLeg, 1);
            float jumpLeg = (runCycle < 0.5f ? 1 : -1) * speed;

            m_Animator.SetFloat(m_SpeedId, speed, speedDampTime, Time.deltaTime);
            m_Animator.SetFloat(m_AgularSpeedId, angularSpeed, angularSpeedDampTime, Time.deltaTime);
            m_Animator.SetFloat(m_DirectionId, direction, directionDampTime, Time.deltaTime);
            m_Animator.SetBool(m_GroundedId, isGrounded);
            m_Animator.SetBool("Strafe", inStrafe);
            m_Animator.SetBool("Locomotion", inSoccer);
            m_Animator.SetBool("OwnerBall", ownerBall);
            if (!isGrounded)
                m_Animator.SetFloat(m_JumpId, m_Rigidbody.velocity.y);
            else
                m_Animator.SetFloat(m_JumpLegCycleId, jumpLeg);

            if (motionType == LocomotionType.normal)
            {
                if (isGrounded && speed + direction > 0)
                    m_Animator.speed = animSpeedMultiplier;
                else
                    m_Animator.speed = 1;
            }

            if (useExtraRotation && motionType == LocomotionType.normal)
            {
                if (inNormal)
                {
                    // help the character turn faster (this is in addition to root rotation in the animation)
                    float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, speed);
                    m_Rigidbody.transform.Rotate(0, direction * turnSpeed * Time.deltaTime, 0);
                }
            }

        }
        public void OnAnimatorMove()
        {
            
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (m_Gravity.IsGrounded)
            {
                if (m_Rigidbody.isKinematic)
                {
                    if (m_ai)
                    {
                        m_Rigidbody.GetComponent<NavMeshAgent>().velocity = Vector3.zero;
                    }

                    m_Rigidbody.transform.position = m_Animator.rootPosition;
                    m_Rigidbody.transform.rotation = m_Animator.rootRotation;

                }
                else
                {
                    Vector3 v = (m_Animator.deltaPosition * animSpeedMultiplier) / Time.deltaTime;
                    v.y = m_Rigidbody.velocity.y;


                    if (m_ai)
                    {

                        m_Rigidbody.GetComponent<NavMeshAgent>().velocity = Vector3.Lerp(m_Rigidbody.GetComponent<NavMeshAgent>().velocity, v, animatorSpeedDamp * Time.deltaTime);

                    }
                    else
                    {
                        m_Rigidbody.velocity = Vector3.Lerp(m_Rigidbody.velocity, v, animatorSpeedDamp * Time.deltaTime);
                    }

                    if (motionType == LocomotionType.normal)
                        m_Rigidbody.transform.rotation = Quaternion.Slerp(m_Rigidbody.transform.rotation, m_Animator.rootRotation, animatorSpeedDamp * Time.deltaTime);
                    else
                        m_Rigidbody.transform.rotation = m_Animator.rootRotation;
                }
            }
        }
        public bool IsAgentDone
        {
            get
            {
                return !m_Agent.pathPending && IsAgentStopping;
            }
        }
        public bool IsAgentStopping
        {
            get
            {
                if (m_Agent.enabled == false)
                    return true;

                return m_Agent.remainingDistance <= m_Agent.stoppingDistance;
            }
        }
        public Vector2 GetDirection(Vector3 position)
        {
            Vector2 result = Vector2.zero;


            Vector3 heading = (position - m_Rigidbody.transform.position) * 1;
            Vector3 m_CamForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            // Vector3 m_Move = heading.z * m_CamForward + heading.x * Camera.main.transform.right;
            Vector3 m_Move = heading;

            if (m_Move.magnitude > 1f) m_Move.Normalize();
            m_Move = m_Rigidbody.transform.InverseTransformDirection(m_Move);
            m_Move = Vector3.ProjectOnPlane(m_Move, m_Gravity.GroudNormal);


            float direction = 0;
            float speed = 0;

            switch (motionType)
            {
                case LocomotionType.soccer:
                    {


                        direction = (float)((Mathf.Atan2(m_Move.x, m_Move.z)) * 45 / 3.14159);
                        speed = m_Move.z * 5.5f;

                        break;
                    }
                case LocomotionType.normal:

                    direction = Mathf.Atan2(m_Move.x, m_Move.z);
                    speed = m_Move.z;

                    break;
                case LocomotionType.strafe:

                    direction = m_Move.x;
                    speed = m_Move.z;

                    break;
            }
            result = new Vector2(direction, speed);

            return result;
        }
        public Vector2 GetDirectionAI()
        {

            Vector3 nmove = Vector3.zero;
            Vector2 result = Vector2.zero;

            nmove = Quaternion.Inverse(m_Rigidbody.transform.rotation) * m_Rigidbody.gameObject.GetComponent<NavMeshAgent>().desiredVelocity;


            float direction = 0;
            float speed = 0;

            switch (motionType)
            {
                case LocomotionType.soccer:
                    {

                        direction = (float)((Mathf.Atan2(nmove.x, nmove.z)) * 180 / 3.14159);
                        speed = m_Rigidbody.GetComponent<NavMeshAgent>().desiredVelocity.magnitude;

                        break;
                    }
                case LocomotionType.normal:

                    direction = Mathf.Atan2(nmove.x, nmove.z);
                    speed = nmove.z;

                    break;
                case LocomotionType.strafe:

                    direction = nmove.x;
                    speed = nmove.z;

                    break;
            }
            result = new Vector2(direction, speed);

            return result;
        }
        public Vector2 GetDirectionAxis1()
        {
            float h = ControllerInput.GetAxisHorizontal1(m_controller.GetInputType());
            float v = ControllerInput.GetAxisVertical1(m_controller.GetInputType());
            return GetDirectionAxis(h, v);
        }
        public Vector2 GetDirectionAxis2()
        {
            float h = ControllerInput.GetAxisHorizontal2(m_controller.GetInputType());
            float v = ControllerInput.GetAxisVertical2(m_controller.GetInputType());
            return GetDirectionAxis(h, v);
        }

        public Vector3 GetMouseTarget(Transform transform)
        {
            Vector3 target = Vector3.zero;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 destination = hit.point;
                destination.y = transform.position.y;
                target = destination;
            }

            return target;
        }

        public Vector3 GetRandomNavCircle(PlayerController origin, float dist)
        {
            return GetRandomNavCircle(origin.transform.position, dist);
        }
        public Vector3 GetRandomNavCircle(Vector3 origin, float dist)
        {
            Vector3 randDirection = UnityEngine.Random.insideUnitCircle * dist;

            randDirection += origin;

            NavMeshHit navHit;

            NavMesh.SamplePosition(randDirection, out navHit, dist, NavMesh.AllAreas);

            return navHit.position;
        }
        public void SetSpeedMultiplies(float mult)
        {
            animSpeedMultiplier = mult;
        }
        public void ResetSpeedMultiples()
        {
            animSpeedMultiplier = 1.0f;
        }
        public void TriggerKick()
        {
            if (inSoccer == false && inStrafe == false && inStumble == false)
            {
                if (inWalkRun == true || inIdle == true)
                {
                    m_Animator.SetTrigger(m_KickId);
                }
            }
        }
        public void TriggerPass()
        {
            if (inSoccer == false && inStrafe == false && inStumble == false)
            {
                if (inWalkRun == true || inIdle == true)
                {
                    m_Animator.SetTrigger(m_PassId);
                }
            }
        }
        public void TriggerEntry()
        {
            if (!inEntry)
                m_Animator.SetTrigger(m_EntryId);
        }
        public void SetTrip()
        {

            m_Animator.SetBool(m_Trip, true);
        }
        public void ResetTrip()
        {
            m_Animator.SetBool(m_Trip, false);
        }
        public void TriggerStumb()
        {
            if (!inStumble)
                m_Animator.SetTrigger(m_StumbleId);
        }

        private Vector3 GetDirectionAxis(float h, float v)
        {

            Vector2 result = Vector2.zero;

            Vector3 m_CamForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 m_Move = v * m_CamForward + h * Camera.main.transform.right;

            if (m_Move.magnitude > 1f) m_Move.Normalize();
            m_Move = m_Rigidbody.transform.InverseTransformDirection(m_Move);
            m_Move = Vector3.ProjectOnPlane(m_Move, m_Gravity.GroudNormal);

            float direction = 0;
            float speed = 0;

            switch (motionType)
            {
                case LocomotionType.soccer:

                    direction = (float)((Mathf.Atan2(m_Move.x, m_Move.z)) * 180 / 3.14159);
                    speed = m_Move.z * 10.0f;
                    break;
                case LocomotionType.normal:

                    direction = Mathf.Atan2(m_Move.x, m_Move.z);
                    speed = m_Move.z;

                    break;
                case LocomotionType.strafe:

                    direction = m_Move.x;
                    speed = m_Move.z;

                    break;
            }

            result = new Vector2(direction, speed);
            return result;
        }
    }
}