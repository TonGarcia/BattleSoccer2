using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SoccerGame;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private ControllerLocomotion locomotion;
    public ControllerLocomotion Locomotion { get { return locomotion; } }
    [SerializeField]
    private ControllerGravity gravity;
    public ControllerGravity Gravity { get { return gravity; } }
    [HideInInspector]
    public float speed = 0;
    [HideInInspector]
    public float dir = 0;

    public bool isAI { get { return playerInput.IsAI; } }
    public bool isMovie { get { return (speed > 0.0f || dir > 0.0f); } }

    private new Rigidbody rigidbody;
    private NavMeshAgent agent;
    private Animator animator;
    private PlayerInput playerInput;
    private Manualcontroller manualController;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        manualController = GetComponent<Manualcontroller>();

    }
    private void Start()
    {
        gravity.Start(rigidbody, animator);
        locomotion.Start(transform, gravity, playerInput);
    }
    private void Update()
    {
        agent.enabled = playerInput.IsAI;
        manualController.enabled = !playerInput.IsAI;
        locomotion.DoAnimator(speed, dir);

    }
    private void FixedUpdate()
    {
        //Atualiza forças da gravidade
        gravity.Update(false);

    }
    void OnAnimatorMove()
    {
        locomotion.OnAnimatorMove();

    }

}
