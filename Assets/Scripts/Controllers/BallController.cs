using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;

public static class BallControllerExtensions
{
    /// <summary>
    /// Pesquisa se a bola esta de poce do jogador
    /// </summary>
    /// <param name="player">jogador pesquisado</param>
    /// <returns>Verdadeiro se a bola estiver de poce deste jogador ou falso caso contrario</returns>
    public static bool IsMyBall(this PlayerController player)
    {
        return BallController.IsOwner(player);
    }
    public static bool IsMyBall(this CampTeam team)
    {
        return BallController.instance.IsMyTeam(team);
    }
    /// <summary>
    /// Verifica se a boal esta de poce de algum do time do jogador pesquisado
    /// Tenha em mente que falso sera retornado se a bola não tiver dono ou se o dono não for
    /// o jogador pesquisado
    /// </summary>
    /// <param name="">jogador pesquisado</param>
    /// <returns>Faso se a bola não tiver dono ou se o dono nao for o jogador pesquisado
    /// e Verdadeiro caso a bola esteja de poce do jogador pesquisado</returns>
    public static bool IsBallfromMyTeam(this PlayerController player)
    {
        return BallController.IsFromTeam(player);
    }
    /// <summary>
    /// Informa se o jogaodr é o jogador mais proximo da bola atualmente
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public static bool IsBallMostNear(this PlayerController player)
    {

        PlayerController mostNeat = GameManager.instance.GetPlayerNearBall(player.GetCampTeam());
        if (mostNeat != null)
            return player == mostNeat;
        else
            return true;
    }
    public static bool IsBallNear(this PlayerController player, float relativeDistance = 5.5f)
    {
        float distance = BallController.instance.transform.Distance(player.transform);
        if (distance > relativeDistance)
            return false;
        else
            return true;
    }
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class BallController : MonoBehaviour
{

    [System.Serializable]
    public class OffsetConfiguration
    {
        [SerializeField]
        [Range(-2.00f, 2.00f)]
        private float forward = 0.65f;
        public float Forwad { get { return forward; } }
        [SerializeField]
        [Range(-2.00f, 2.00f)]
        private float up = 0.25f;
        public float Up { get { return up; } }
        [SerializeField]
        [Range(-2.00f, 2.00f)]
        private float right = 0f;
        public float Right { get { return right; } }

        public Vector3 GetMult(Transform transform)
        {

            Vector3 target = transform.position + transform.forward * Forwad + transform.right * Right + transform.up * Up;
            return target;

        }
    }
    public delegate void OnSetOwner(PlayerController owner, PlayerController lasOwner);
    public delegate void OnRemoveOwner(PlayerController lasOwner);

    public static BallController instance;

    [Header("Ball Controller")]
    [SerializeField]
    private PlayerController owner;
    [SerializeField]
    private Collider fovTriger;
    [SerializeField]
    private float speed_return = 5.5f;
    [SerializeField]
    private float speed_forward = 5.5f;
    [SerializeField]
    private OffsetConfiguration iddleOffset;
    [SerializeField]
    private OffsetConfiguration runOffset;

    public bool HasMyOwner { get { return owner != null; } }

    public event OnSetOwner onSetMyOwner;
    public event OnRemoveOwner onRemoveMyOwner;

    private new Rigidbody rigidbody;

    PlayerController lastOwner;
    PlayerController protectedTo;

    private float timeToSetOwner = 0.0f;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        if (owner)
        {
            timeToSetOwner = 99f;
            PlayerController player = owner;
            UnsetmeOwner();
            SetmeOwner(player);
        }
    }

    void Update()
    {


        if (owner)
        {
            if (owner.isMovie == false)
            {
                //Posiciona a bola no offset predefinido em iddleOffset e
                //Rotaciona a bola para a mesma rotação do player, isto evita bugs com animações rootmotion da bola
                Vector3 targetPos = iddleOffset.GetMult(owner.transform);
                transform.position = Vector3.Lerp(transform.position, targetPos, speed_return * Time.deltaTime);
                transform.rotation = Quaternion.Lerp(transform.rotation, owner.transform.rotation, 3.2f * Time.deltaTime);
            }
            else
            {
                //Posiciona a bola no offset predefinido em runOffset e
                //Rotaciona a bola no sentido de movimento do jogador para dar a impreção de bola rolando
                Vector3 targetPos = runOffset.GetMult(owner.transform);
                float targetRot = (owner.transform.forward * 360 * Time.deltaTime).magnitude;


                transform.position = Vector3.Lerp(transform.position, targetPos, speed_forward * Time.deltaTime);
                transform.Rotate(owner.transform.right, targetRot * 1.8f, Space.World);


            }
        }
        timeToSetOwner += Time.deltaTime;
    }
    private void FixedUpdate()
    {
        if (transform.position.y < 0)
        {
            transform.position = new Vector3(transform.position.x, 0.2f, transform.position.z);
            rigidbody.velocity = Vector3.zero;
            SetBallDesprotectTo();
            return;
        }
    }

    void OnAnimatorMove()
    {
        /*Usado para animação da bola em sincronia com a do jogador
        if (animator.GetBool("IdleDrible"))
        {
            transform.position = animator.rootPosition;
            transform.rotation = animator.rootRotation;
        }
        */
    }

    public void SetmeOwner(PlayerController player)
    {
        if (player == null)
            return;

        if (player.Locomotion.inTrip || player.Locomotion.inStumble)
            return;

        if (timeToSetOwner <= 0.5f)
            return;
                
        if (onSetMyOwner != null)
            onSetMyOwner(player, owner);


        lastOwner = owner;
        owner = player;


        SetKinematic();

        timeToSetOwner = 0.0f;
    }
    public void UnsetmeOwner()
    {
        lastOwner = owner;
        owner = null;

        if (onRemoveMyOwner != null)
            onRemoveMyOwner(lastOwner);

        //fovTriger.enabled = true;
        UnsetKinematic();


    }
    public void SetmeKick()
    {

        if (owner == null)
            return;

        PlayerController playerFromKick = owner;
        // SetBallDesprotectTo();
        UnsetmeOwner();
        rigidbody.AddForce(playerFromKick.transform.forward * 20, ForceMode.Impulse);
        rigidbody.AddForce(playerFromKick.transform.up * 8, ForceMode.Impulse);

    }
    public void SetmePass(float distance)
    {

        if (owner == null)
            return;

        PlayerController playerFromKick = owner;
        SetBallDesprotectTo();
        UnsetmeOwner();
        rigidbody.AddForce(playerFromKick.transform.forward * distance, ForceMode.Impulse);
        rigidbody.AddForce(playerFromKick.transform.up * distance / 4, ForceMode.Impulse);


    }
    public void ChangemeDirection()
    {
        if (owner == null)
            return;


        PlayerController old = owner;
        UnsetmeOwner();


        rigidbody.AddForce(old.transform.up * 3.5f, ForceMode.Impulse);
        rigidbody.AddForce(-old.transform.forward * 2.0f, ForceMode.Impulse);



    }
    public void SetBallProtectedTo(PlayerController player)
    {
        if (protectedTo != null)
            return;

        protectedTo = player;
        int maskPlayer = LayerMask.NameToLayer("SoccerPlayer");
        int maskBall = LayerMask.NameToLayer("SoccerBall");

        Physics.IgnoreLayerCollision(maskPlayer, maskBall, true);
        fovTriger.enabled = false;
    }
    public void SetBallDesprotectTo(PlayerController player)
    {
        if (protectedTo != player)
            return;

        int maskPlayer = LayerMask.NameToLayer("SoccerPlayer");
        int maskBall = LayerMask.NameToLayer("SoccerBall");

        Physics.IgnoreLayerCollision(maskPlayer, maskBall, false);
        protectedTo = null;
        fovTriger.enabled = true;
    }
    private void SetBallDesprotectTo()
    {
        int maskPlayer = LayerMask.NameToLayer("SoccerPlayer");
        int maskBall = LayerMask.NameToLayer("SoccerBall");

        Physics.IgnoreLayerCollision(maskPlayer, maskBall, false);
        protectedTo = null;
        fovTriger.enabled = true;
    }
    public PlayerController GetMyOwner()
    {
        return owner;
    }
    public PlayerController GetMyLastOwner()
    {
        return lastOwner;
    }

    public bool IsMyOwner(PlayerController player)
    {
        return player == owner;
    }
    public bool IsMyLastOwner(PlayerController player)
    {
        return player == lastOwner;
    }
    /// <summary>
    /// Verifica se a bola esta de poce de alguem do time do jogador pesquisado
    /// </summary>
    /// <param name="player">Jogador a ser pesquisado</param>
    /// <returns>Falso se a bola estiver sem nenhum dono ou se o atual dono
    /// nnão pertencer ao time do jogador pesquisado</returns>
    public bool IsMyTeam(PlayerController player)
    {
        bool result = false;

        if (owner != null)
        {
            if (owner.GetCampTeam() == player.GetCampTeam())
            {
                result = true;
            }
        }

        return result;
    }
    public bool IsMyTeam(CampTeam team)
    {
        bool result = false;
        if (owner != null)
            if (owner.GetCampTeam() == team)
                result = true;

        return result;
    }

    public static void ChangeDirection()
    {
        instance.ChangemeDirection();
    }
    public static bool HasOwner()
    {
        return instance.HasMyOwner;
    }
    public static bool IsOwner(PlayerController player)
    {
        return instance.IsMyOwner(player);
    }
    public static bool IsLastOwner(PlayerController player)
    {
        return instance.IsMyLastOwner(player);
    }
    public static void SetKick()
    {
        instance.SetmeKick();
    }
    public static void SetPass(float distance)
    {
        instance.SetmePass(distance);
    }
    public static void SetOwner(PlayerController player)
    {
        if (player != null)
        {
            instance.SetmeOwner(player);
        }
    }
    public static void UnsetOwner()
    {
        instance.UnsetmeOwner();
    }
    public static bool IsFromTeam(PlayerController player)
    {
        return BallController.instance.IsMyTeam(player);
    }

    public static Vector3 GetPosition()
    {
        return instance.transform.position;
    }
    public static PlayerController GetOwner()
    {
        return instance.GetMyOwner();
    }
    public static PlayerController GetLastOwner()
    {
        return instance.GetMyLastOwner();
    }
    public static bool isReady
    {
        get { return instance != null; }
    }
    private void SetKinematic()
    {
        rigidbody.isKinematic = true;
        GetComponent<Collider>().isTrigger = true;

    }
    private void UnsetKinematic()
    {
        GetComponent<Collider>().isTrigger = false;
        rigidbody.isKinematic = false;
    }


}
