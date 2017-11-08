using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SoccerGame;


public static class PlayerControllerExtensions
{

    public static ControllerInputType PlayerInputType(this PlayerController controller)
    {
        PlayerInput pinput = controller.GetComponent<PlayerInput>();
        return pinput.InputType;
    }
    public static CampTeam PlayerTeam(this PlayerController controller)
    {
        PlayerTeam pinput = controller.GetComponent<PlayerTeam>();
        return pinput.Team;
    }

    public static bool IsMyBall(this PlayerController player)
    {
        return BallController.IsOwner(player);
    }
}

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PlayerTeam))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    //Componente de locomoção
    [SerializeField]
    private ControllerLocomotion locomotion;
    public ControllerLocomotion Locomotion { get { return locomotion; } }

    //Componennte de gravidade 
    [SerializeField]
    private ControllerGravity gravity;
    public ControllerGravity Gravity { get { return gravity; } }

    //Velocidade normal aplicada ao rootMotion apartir do componente locomotion
    [HideInInspector]
    public float speed = 0;

    //Direção normal aplicada ao rootMotion apartir do componente locomotion
    [HideInInspector]
    public float dir = 0;

    public bool isAI { get { return playerInput.IsAI; } }
    public bool isMovie { get { return (speed > 0.0f || dir > 0.0f); } }

    private new Rigidbody rigidbody;
    private Animator animator;
    private NavMeshAgent agent;
    private PlayerInput playerInput;
    private ManualController manualController;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        manualController = GetComponent<ManualController>();

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

    public void SetManual()
    {
        playerInput.InputType = GameManager.instance.GetControllerType(this.PlayerTeam());
    }
    public void SetAutomatic()
    {
        playerInput.InputType = ControllerInputType.ControllerCPU;
    }
}
