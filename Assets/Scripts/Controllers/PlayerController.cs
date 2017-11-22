using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SoccerGame;


public static class PlayerControllerExtensions
{

    public static ControllerInputType GetPlayerInputType(this PlayerController controller)
    {
        PlayerInput pinput = controller.GetComponent<PlayerInput>();
        return pinput.InputType;
    }
    public static CampTeam GetCampTeam(this PlayerController controller)
    {
        PlayerTeam pinput = controller.GetComponent<PlayerTeam>();
        return pinput.Team;
    }
    public static CampActionAttribute GetCampAction(this PlayerController controller)
    {
        PlayerTeam pinput = controller.GetComponent<PlayerTeam>();
        return pinput.PlaceAction;
    }
    public static CampPlaceMarcation GetPlaceMarcation(this PlayerController controller)
    {
        PlayerTeam pinput = controller.GetComponent<PlayerTeam>();
        return pinput.PlaceMarcation;
    }
    public static AIController GetAiControlelr(this PlayerController controller)
    {
        return controller.GetComponent<AIController>();
    }
    public static bool IsMyTeaM(this PlayerController controller, PlayerController player)
    {
        return controller.GetCampTeam() == player.GetCampTeam();
    }

    public static bool IsHitBetween(this PlayerController player, PlayerController to)
    {
        bool result = false;
        PlayerController hitedPlayer = null;

        if (player.IsHitBetween(to.transform, out hitedPlayer))
        {
            if (hitedPlayer != to)
                result = true;
        }


        return result;

    }
    public static bool IsHitBetween(this PlayerController player, Vector3 to)
    {
        bool result = false;
        PlayerController hitedPlayer = null;

        if (player.IsHitBetween(to, out hitedPlayer))
        {
            if (player.transform.position.Equals(to) == false)
                result = true;
        }


        return result;

    }
    public static bool IsHitBetween(this PlayerController player, PlayerController to, out PlayerController hitPlayer)
    {
        bool result = false;
        PlayerController hitedPlayer = null;

        if (player.IsHitBetween(to.transform, out hitedPlayer))
        {
            if (hitedPlayer != to)
                result = true;
        }

        hitPlayer = hitedPlayer;
        return result;

    }
    public static bool IsHitBetween(this PlayerController player, Transform to, out PlayerController hitPlayer)
    {
        bool result = false;
        PlayerController hitedPlayer = null;

        if (player.IsHitBetween(to.position, out hitedPlayer))
        {
            if (hitedPlayer.transform != to)
                result = true;
        }

        hitPlayer = hitedPlayer;
        return result;

    }
    public static bool IsHitBetween(this PlayerController player, Vector3 to, out PlayerController hitPlayer)
    {

        bool result = false;
        PlayerController hitedPlayer = null;

        Vector3 origem = player.transform.position;
        Vector3 halfExtents = Vector3.one / 2f;
        Vector3 direction = player.transform.forward;
        Quaternion orientation = player.transform.rotation;

        float distance = player.Distance(to);

        RaycastHit hitInfo;
        if (Physics.BoxCast(origem, halfExtents, direction, out hitInfo, orientation, distance))
        {

            if (hitInfo.transform.GetComponent<PlayerController>() != null)
            {
                hitedPlayer = hitInfo.transform.GetComponent<PlayerController>();
                if (hitedPlayer.transform.position != to)
                    result = true;
            }

            ExtDebug.DrawBoxCastOnHit(origem, halfExtents, orientation, direction, hitInfo.distance, Color.red);
        }

        hitPlayer = hitedPlayer;
        return result;
    }

    public static bool IsHitForwad(this PlayerController player, float distance, out PlayerController hitPlayer)
    {
        bool result = false;
        PlayerController hitedPlayer = null;

        Vector3 origem = player.transform.position;
        Vector3 halfExtents = Vector3.one / 2f;
        Vector3 direction = player.transform.forward;
        Quaternion orientation = player.transform.rotation;

        RaycastHit hitInfo;
        if (Physics.BoxCast(origem, halfExtents, direction, out hitInfo, orientation, distance))
        {

            if (hitInfo.transform.GetComponent<PlayerController>() != null)
            {
                hitedPlayer = hitInfo.transform.GetComponent<PlayerController>();
                result = true;
            }

            ExtDebug.DrawBoxCastOnHit(origem, halfExtents, orientation, direction, hitInfo.distance, Color.red);
        }

        hitPlayer = hitedPlayer;
        return result;

    }
    public static bool IsHitForwad(this PlayerController player, float distance, out PlayerController hitPlayer, CampTeam team)
    {
        bool result = false;
        PlayerController hitedPlayer = null;

        Vector3 origem = player.transform.position;
        Vector3 halfExtents = Vector3.one / 2f;
        Vector3 direction = player.transform.forward;
        Quaternion orientation = player.transform.rotation;

        RaycastHit hitInfo;
        if (Physics.BoxCast(origem, halfExtents, direction, out hitInfo, orientation, distance))
        {

            if (hitInfo.transform.GetComponent<PlayerController>() != null)
            {

                hitedPlayer = hitInfo.transform.GetComponent<PlayerController>();

                if (hitedPlayer.GetCampTeam() == team)
                    result = true;
            }

            ExtDebug.DrawBoxCastOnHit(origem, halfExtents, orientation, direction, hitInfo.distance, Color.red);
        }

        hitPlayer = hitedPlayer;
        return result;

    }

    public static bool GetFreeHitRight(this PlayerController player, float distance, out Vector3 FreePosition)
    {
        bool result = false;
        Vector3 resultPosition = Vector3.zero;


        for (int angle = 0; angle <= 90; angle += 30)
        {

            //Angle Target to rotate box
            Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 angleDirection = player.transform.forward * distance;
            Vector3 angleTarget = angleAxis * angleDirection;

            //Projection of the box
            Vector3 origin = player.transform.position;
            Vector3 halfExtents = Vector3.one / 2f;
            Vector3 direction = angleTarget - player.transform.position; //Transform.forward
            Quaternion orientation = Quaternion.LookRotation(direction); //Transform.rotation

            RaycastHit hitInfo;
            if (Physics.BoxCast(origin, halfExtents, direction, out hitInfo, orientation, distance, LayerMask.GetMask("SoccerPlayer")) == false)
            {
                result = true;
                resultPosition = angleTarget;
                break;

            }

        }


        FreePosition = resultPosition;
        return result;

    }
    public static bool GetFreeHitLeft(this PlayerController player, float distance, out Vector3 FreePosition)
    {
        bool result = false;
        Vector3 resultPosition = Vector3.zero;


        for (int angle = 0; angle <= 90; angle += 30)
        {

            //Angle Target to rotate box
            Quaternion angleAxis = Quaternion.AngleAxis(-angle, Vector3.up);
            Vector3 angleDirection = player.transform.forward * distance;
            Vector3 angleTarget = angleAxis * angleDirection;

            //Projection of the box
            Vector3 origin = player.transform.position;
            Vector3 halfExtents = Vector3.one / 2f;
            Vector3 direction = angleTarget - player.transform.position; //Transform.forward
            Quaternion orientation = Quaternion.LookRotation(direction); //Transform.rotation

            RaycastHit hitInfo;
            if (Physics.BoxCast(origin, halfExtents, direction, out hitInfo, orientation, distance, LayerMask.GetMask("SoccerPlayer")) == false)
            {
                result = true;
                resultPosition = angleTarget;
                break;

            }

        }


        FreePosition = resultPosition;
        return result;

    }
    public static bool GetFreeHitLeftOrRight(this PlayerController player, float distance, float leftOrRight, out Vector3 FreePosition)
    {
        if (leftOrRight > 0)
        {

            return player.GetFreeHitRight(distance, out FreePosition);
        }
        else if (leftOrRight < 0)
        {
            return player.GetFreeHitLeft(distance, out FreePosition);
        }
        else
        {
            FreePosition = Vector3.zero;
            return false;
        }

    }

    public static float LeftRightDir(this PlayerController player, Vector3 position)
    {

        Vector3 targetDir = position - player.transform.position;

        Vector3 perp = Vector3.Cross(player.transform.forward, targetDir);
        float dir = Vector3.Dot(perp, player.transform.up);

        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir < 0f)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }

    public static bool IsLookAt(this PlayerController player, Vector3 to)
    {
        Vector3 dirFromAtoB = (to - player.transform.position).normalized;
        float dotProd = Vector3.Dot(dirFromAtoB, player.transform.forward);
        bool result = false;

        if (dotProd > 0.9)
            result = true;

        return result;
    }
    public static bool IsLookAt(this PlayerController player, Transform to)
    {
        return player.IsLookAt(to.position);
    }
    public static bool IsLookAt(this PlayerController player, PlayerController to)
    {
        return player.IsLookAt(to.transform);
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

    public Collider FovBallTryger;

    public bool isAI { get { return playerInput.IsAI; } }
    public bool isMovie { get { return (speed > 0.0f || dir > 0.0f); } }

    private new Rigidbody rigidbody;
    private Animator animator;
    private NavMeshAgent agent;
    private PlayerInput playerInput;
    private ManualController manualController;
    private AIController aicontroller;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        manualController = GetComponent<ManualController>();
        aicontroller = GetComponent<AIController>();

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
        aicontroller.enabled = playerInput.IsAI;
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

    public void SetKinematic()
    {
        GetComponent<Animator>().rootPosition = transform.position;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Collider>().enabled = false;
        FovBallTryger.enabled = false;
    }
    public void UnsetKinematic()
    {
        gameObject.GetComponent<Collider>().enabled = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        FovBallTryger.enabled = true;
    }
    public void SetManual()
    {
        playerInput.InputType = GameManager.instance.GetControllerType(this.GetCampTeam());
    }
    public void SetAutomatic()
    {
        playerInput.InputType = ControllerInputType.ControllerCPU;
    }
    public void SetMotionNormal()
    {
        locomotion.motionType = LocomotionType.normal;
    }
    public void SetMotionSoccer()
    {
        locomotion.motionType = LocomotionType.soccer;
    }
    public void SetMotionStrafe()
    {
        locomotion.motionType = LocomotionType.strafe;
    }
}
