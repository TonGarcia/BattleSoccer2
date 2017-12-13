using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SoccerGame
{
    [System.Serializable]
    public class ControllerGravity
    {
        private Rigidbody rigidbody;
        private Animator animator;

        [SerializeField]
        private float groundCheckDistance = 0.25f;

        [SerializeField]
        private float jumpPower = 12f;
        [SerializeField]
        [Range(0, 9)]
        private float gravityMultiplier = 2f;
        [SerializeField]
        private float runCycleLegOffset = 0.2f;
        public float CycleLefOffset { get { return runCycleLegOffset; } }

        private Vector3 m_GroundNormal;
        public Vector3 GroudNormal { get { return m_GroundNormal; } }

        private bool m_IsGrounded;
        public bool IsGrounded { get { return m_IsGrounded; } }
        private float m_OrigGroundCheckDistance;

        public void Start(Rigidbody rigidbody, Animator animator)
        {
            this.rigidbody = rigidbody;
            this.animator = animator;
            m_OrigGroundCheckDistance = groundCheckDistance;
        }
        /// <summary>
        /// Aplique no fixedUpdate para atualizar o motor de gravidade a cada frame
        /// </summary>

        public void Update(bool jump)
        {
            UpdateGroundStatus();

            if (m_IsGrounded)
                HandleGroundedMovement(jump);
            else
                HandleAirborneMovement();
        }


        //Atualiza as variaveis responsaveis pela gravidade
        private void UpdateGroundStatus()
        {
            //Sabe deus porque as vezes o agent cai para baixo do plano com navmesh.
            //Como este jogo nao tem posição negativa em y, eu garanto aqui que o jogador nao vai sumir no vazio
            //caindo para baixo do campo. 
            if (rigidbody.transform.position.y < 0)
            {
                Vector3 npos = rigidbody.transform.position;
                npos.y = 0;
                rigidbody.transform.position = npos;
            }
            RaycastHit hitInfo;
#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            Debug.DrawLine(rigidbody.transform.position + (Vector3.up * 0.1f), rigidbody.transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));
#endif
            // 0.1f is a small offset to start the ray from inside the character
            // it is also good to note that the transform position in the sample assets is at the base of the character
            if (Physics.Raycast(rigidbody.transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance))
            {
                m_GroundNormal = hitInfo.normal;
                m_IsGrounded = true;
                animator.applyRootMotion = true;
                if (rigidbody.GetComponent<NavMeshAgent>())
                {
                    if (rigidbody.GetComponent<PlayerController>())
                        if (rigidbody.GetComponent<PlayerController>().IsIA)
                            rigidbody.GetComponent<NavMeshAgent>().updatePosition = true;
                }

            }
            else
            {
                m_IsGrounded = false;
                m_GroundNormal = Vector3.up;
                animator.applyRootMotion = false;
            }
        }
        private void HandleAirborneMovement()
        {
            // apply extra gravity from multiplier:
            Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
            rigidbody.AddForce(extraGravityForce);

            groundCheckDistance = rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
        }
        private void HandleGroundedMovement(bool jump)
        {
            // check whether conditions are right to allow a jump:
            if (jump && animator.GetCurrentAnimatorStateInfo(0).IsName("WalkRun"))
            {

                // jump!
                if (rigidbody.GetComponent<NavMeshAgent>())
                    rigidbody.GetComponent<NavMeshAgent>().updatePosition = false;

                rigidbody.velocity = new Vector3(rigidbody.velocity.x, jumpPower, rigidbody.velocity.z);
                m_IsGrounded = false;
                animator.applyRootMotion = false;
                groundCheckDistance = 0.1f;

            }


        }
    }
}

