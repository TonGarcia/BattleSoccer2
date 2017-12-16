using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;
using UnityEngine.AI;
using System;

public class AIController : MonoBehaviour
{

    [Tooltip("Se o jogador tomar a bola e estiver a uma distancia igual ou menor, a animação de tomada de bola sera executada")]
    public float distanceToEntry = 2.5f;
    public float distanceToDrop = 1.5f;

    [Range(0.0f, 10.0f)]
    public float distanceToDrible = 5.5f;
    [Range(0.0f, 10.0f)]
    public float distanceToPass = 10.0f;
    public float timeTodrible = 0.5f;

    public Collider FovBallTryger;

    private PlayerController player;
    private ControllerLocomotion locomotion { get { return player.Locomotion; } }
    private float direction { get { return player.dir; } set { player.dir = value; } }
    private float speed { get { return player.speed; } set { player.speed = value; } }


    private NavMeshAgent agent;
    private SoccerAIGeneric AIUnselected;
    private SoccerAIGeneric AISelected;
    private SoccerAIGeneric AIWithBall;
    private SoccerAIGeneric SelectedAIState;

    private float nvSpeed, nvAngularSpeed, nvAceleration;

    private bool waitingToPass = false;
    private AIController AiToPass = null; //Player que vai receber algum lance de passe


    private void Awake()
    {
        //NavMesh.avoidancePredictionTime = 10.0f;

        player = GetComponent<PlayerController>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        //agent.updatePosition = false;

        AIUnselected = new SoccerAIUnSelected(this, player);
        AISelected = new SoccerAISelected(this, player);
        AIWithBall = new SoccerAIwithBall(this, player);

        nvSpeed = agent.speed;
        nvAngularSpeed = agent.angularSpeed;
        nvAceleration = agent.acceleration;

        SelectedAIState = AIUnselected;
    }

    void Update()
    {
        if (!agent || !player)
            return;

        UpdateNavMeshAgent();

        if (locomotion.inSoccer)
            locomotion.SetSpeedMultiplies(1.2f);
        else
            locomotion.ResetSpeedMultiples();

        //Se alguem fez um passe para mim, vou estar aguardando o passe. Neste caso
        //Vou ajudar correndo para perto da bola
        if (waitingToPass)
        {
            if (locomotion.IsAgentDone)
            {
                direction = 0;
                speed = 0;
                agent.destination = transform.position;
                waitingToPass = false;
                return;
            }
            locomotion.motionType = LocomotionType.strafe;

            //Move and loocakt
            Vector3 target = BallController.GetPosition();
            target.y = transform.position.y;

            agent.SetDestination(target);
            transform.LookAt(target);

            Vector2 move = locomotion.GetDirectionAI();
            direction = move.x;
            speed = move.y;

            return;
        }

        UpdateAiStates();

    }


    //Unity Events
    private void OnEnable()
    {
        SelectedAIState = AIUnselected;
        AIUnselected.StopHandleStates();

        SignEvents();
    }
    private void OnDisable()
    {
        UnsignEvents();
    }


    //BallEvents
    private void OnBallSetOwner(PlayerController owner, PlayerController lasOwner)
    {
        //Animação de entrada de bola
        if (owner == player)
        {
            if ((player.IsIA && !agent.enabled) || (player.IsIA && !agent.isOnNavMesh))
                return;

            //agent.SetDestination(BallController.GetPosition());

            if (lasOwner != null)
            {
                float distance = lasOwner.Distance(player);
                if (distance <= distanceToEntry)
                {
                    locomotion.TriggerEntry();
                }
            }
        }

        waitingToPass = false;
    }
    private void OnBallRemoveOwner(PlayerController lasOwner)
    {

    }

    //Animations Event Tryger
    //Estes eventos são chamados apartir das animações rerentes em quadros espesificos

    //Change Direction
    private void EvChangeDirectionStart()
    {
        player.SetKinematic();

        if (!player.IsMyBall() || !player.isOk)
            return;

        BallController.instance.SetBallProtectedTo(player);
        BallController.ChangeDirection();

        //Se o jogador selecionado do time adversario estiver proximo a mim na hora do lésinho, vou fazer ele tropeçar       
        List<PlayerController> enemys = player.GetEnemysNear(2.5f);
        if (enemys.Count > 0)
            foreach (PlayerController enemy in enemys)
                if (enemy.Locomotion.inAir == false)
                    enemy.Locomotion.TriggerStumb();


    }
    private void EvChangeDirectionOk()
    {
        BallController.instance.SetBallDesprotectTo(player);
    }
    private void EvChangeDirectionFinish()
    {
        BallController.instance.SetBallDesprotectTo(player);
        player.UnsetKinematic();
    }
    //Turn Direction
    private void EvTurnDirectionStart()
    {
        player.SetKinematic();

        if (!player.IsMyBall() || !player.isOk)
            return;


        BallController.instance.SetBallProtectedTo(player);
    }
    private void EvTurnDirectionFinish()
    {
        BallController.instance.SetBallDesprotectTo(player);
        player.UnsetKinematic();
    }
    //Kick
    private void EvLongKickOk()
    {
        if (BallController.IsOwner(player))
            BallController.SetKick();
    }
    private void EvKickFinish()
    {
        //BUGFIX - previne tompo atrazado
        locomotion.ResetTrip();
    }
    //Enttry
    private void EvEntryStart()
    {
        if (BallController.IsOwner(player))
            BallController.instance.SetBallProtectedTo(player);

        player.SetKinematic();

    }
    private void EvEntryFinish()
    {

        BallController.instance.SetBallDesprotectTo(player);
        player.UnsetKinematic();

    }
    //Pass
    private void EvShortPassStart()
    {
        player.SetKinematic();
        if(AiToPass!=null)
        {
            Vector3 dir = AiToPass.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = rotation;
        }
    }
    private void EvShortPassOk()
    {


        if (BallController.IsOwner(player))
        {
            if (AiToPass != null)
            {
                Vector3 dir = AiToPass.transform.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = rotation;

                AiToPass.WaitPass();
                BallController.SetPass(AiToPass.player.Distance(player) * 2.0f);
            }
            else
            {
                BallController.SetPass(12.0f);
            }
        }

        AiToPass = null;



    }
    private void EvShortPassFinish()
    {
        player.UnsetKinematic();
        //BUGFIX - previne tompo atrazado
        locomotion.ResetTrip();
    }
    //Stumble
    private void EvStumbleStart()
    {
        if (player.IsMyBall())
        {
            BallController.instance.SetBallDesprotectTo(player);
            BallController.SetPass(8.0f);
        }

        player.SetKinematic();
    }
    private void EvStumbleFinish()
    {
        player.UnsetKinematic();
    }
    //Traking
    private void EvTrakStart()
    {

    }
    private void EvTrakOkt()
    {

        List<PlayerController> enemys = player.GetEnemysNear(distanceToDrop);
        if (enemys.Count > 0)
            foreach (PlayerController enemy in enemys)
                if (enemy.Locomotion.inAir == false)
                    enemy.Locomotion.SetTrip();
    }
    private void EvTrakFinish()
    {

    }
    //Triping
    private void EvTripFinish()
    {

    }
    private void EvTripStart()
    {
        if (player.IsMyBall())
        {
            BallController.instance.SetBallDesprotectTo(player);
            BallController.SetPass(8.0f);
        }

        player.SetKinematic();
    }

    private void EvStandup()
    {
        player.UnsetKinematic();
    }
    //Private methods
    private void UpdateAiStates()
    {
        if (player.IsMyBall())           //Estados de Jogador com a bola
        {
            if (SelectedAIState != AIWithBall)
            {
                SelectedAIState.StopHandleStates();
                SelectedAIState = AIWithBall;
            }

            SelectedAIState.ToWithBall().checkTimeToDrible = timeTodrible;
            SelectedAIState.ToWithBall().checkDistanceToDrible = distanceToDrible;
            SelectedAIState.ToWithBall().checkDistanceToPass = distanceToPass;

        }
        else if (player.IsSelected())    //Estados de jogador sem bola mas selecionado 
        {
            if (SelectedAIState != AISelected)
            {

                SelectedAIState.StopHandleStates();
                SelectedAIState = AISelected;
            }
        }
        else                            //Estado jogador sem bola e não selecionado
        {

            if (SelectedAIState != AIUnselected)
            {

                SelectedAIState.StopHandleStates();
                SelectedAIState = AIUnselected;
            }
        }

        SelectedAIState.UpdateHandleStates();
    }
    private void SignEvents()
    {
        StartCoroutine(IESignevents());
    }
    private void UnsignEvents()
    {
        //Animatro sign
        PlayerAnimatorEvents animatorEvents = GetComponent<PlayerAnimatorEvents>();
        animatorEvents.OnChangeDirStart -= EvChangeDirectionStart;
        animatorEvents.OnChangeDirOk -= EvChangeDirectionOk;
        animatorEvents.OnChangeDirFinish -= EvChangeDirectionFinish;
        animatorEvents.OnTurnStart -= EvTurnDirectionStart;
        animatorEvents.OnTurnFinish -= EvTurnDirectionFinish;
        animatorEvents.OnKickOk -= EvLongKickOk;
        animatorEvents.OnKickFinish -= EvKickFinish;
        animatorEvents.OnEnttryStart -= EvEntryStart;
        animatorEvents.OnEnttryFinish -= EvEntryFinish;
        animatorEvents.OnPassStart -= EvShortPassStart;
        animatorEvents.OnPassOk -= EvShortPassOk;
        animatorEvents.OnPassFinish += EvShortPassFinish;

        animatorEvents.OnStumblesStart -= EvStumbleStart;
        animatorEvents.OnStumblesFinish -= EvStumbleFinish;

        BallController.instance.onSetMyOwner -= OnBallSetOwner;
        BallController.instance.onRemoveMyOwner -= OnBallRemoveOwner;

        animatorEvents.OnTrackingStart -= EvTrakStart;
        animatorEvents.OnTrackingOk -= EvTrakOkt;
        animatorEvents.OnTrackingFinish -= EvTrakFinish;

        animatorEvents.OnTripingStart -= EvTripStart;
        animatorEvents.OnTripingFinish -= EvTripFinish;
        animatorEvents.OnOnStandingupFinish += EvStandup;

    }
    private void UpdateNavMeshAgent()
    {
        if (locomotion.inSoccer)
        {
            agent.speed = nvSpeed;
            agent.angularSpeed = nvAngularSpeed;
            agent.acceleration = nvAceleration;
        }
        else
        {
            agent.speed = 1;
            agent.angularSpeed = 1;
            agent.acceleration = 8;
        }
    }
    private IEnumerator IESignevents()
    {
        //Animatro sign
        PlayerAnimatorEvents animatorEvents = GetComponent<PlayerAnimatorEvents>();
        animatorEvents.OnChangeDirStart += EvChangeDirectionStart;
        animatorEvents.OnChangeDirOk += EvChangeDirectionOk;
        animatorEvents.OnChangeDirFinish += EvChangeDirectionFinish;
        animatorEvents.OnTurnStart += EvTurnDirectionStart;
        animatorEvents.OnTurnFinish += EvTurnDirectionFinish;
        animatorEvents.OnKickOk += EvLongKickOk;
        animatorEvents.OnKickFinish += EvKickFinish;
        animatorEvents.OnEnttryStart += EvEntryStart;
        animatorEvents.OnEnttryFinish += EvEntryFinish;
        animatorEvents.OnPassStart += EvShortPassStart;
        animatorEvents.OnPassOk += EvShortPassOk;
        animatorEvents.OnPassFinish += EvShortPassFinish;
        animatorEvents.OnStumblesStart += EvStumbleStart;
        animatorEvents.OnStumblesFinish += EvStumbleFinish;
        animatorEvents.OnTrackingStart += EvTrakStart;
        animatorEvents.OnTrackingOk += EvTrakOkt;
        animatorEvents.OnTrackingFinish += EvTrakFinish;
        animatorEvents.OnTripingStart += EvTripStart;
        animatorEvents.OnTripingFinish += EvTripFinish;
        animatorEvents.OnOnStandingupFinish += EvStandup;

        while (BallController.instance == null)
            yield return null;

        BallController.instance.onSetMyOwner += OnBallSetOwner;
        BallController.instance.onRemoveMyOwner += OnBallRemoveOwner;
    }



    //InternalMethods
    internal bool TriggerPass(PlayerController toPlayer)
    {
        bool result = false;
        if (locomotion.TriggerPass())
        {
            AiToPass = toPlayer.GetAiControlelr();
            result = true;
        }
        return result;
    }
    internal void GoToPosition(Vector3 position)
    {
        SelectedAIState.ForceGoTo(position);

    }
    internal void GoToPosition(Vector3 position, Transform loockat)
    {
        SelectedAIState.ForceGoTo(position, loockat);

    }
    internal void WaitPass()
    {
        waitingToPass = true;
        SelectedAIState.StopHandleStates();
    }
}

