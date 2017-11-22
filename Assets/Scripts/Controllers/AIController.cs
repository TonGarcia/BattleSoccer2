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

    private PlayerInput playerInput;
    private NavMeshAgent agent;
    private SoccerAIGeneric AIUnselected;
    private SoccerAIGeneric AISelected;
    private SoccerAIGeneric AIWithBall;
    private SoccerAIGeneric SelectedAIState;

    private float nvSpeed, nvAngularSpeed, nvAceleration;

    private bool waitingToPass = false;
    private AIController AiToPass = null; //Player que vai receber algum lance de passe

    private float powerToPass = 0; //Potencia para disparar o pace

    private void Awake()
    {
        //NavMesh.avoidancePredictionTime = 10.0f;

        player = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
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
        if (!agent || !playerInput || !player)
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
    private void OnCollisionEnter(Collision collision)
    {
        PlayerController colPlayer = collision.gameObject.GetComponent<PlayerController>();
        if (colPlayer)
        {
            locomotion.TriggerEntry();
        }

    }

    //BallEvents
    private void OnBallSetOwner(PlayerController owner, PlayerController lasOwner)
    {
        //Animação de entrada de bola
        if (owner == player)
        {
            if (!agent.enabled || !agent.isOnNavMesh)
                return;

            agent.SetDestination(BallController.GetPosition());

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
    private void OnChangeDirectionStart()
    {
        player.SetKinematic();

        if (!player.IsMyBall())
            return;

        BallController.instance.SetBallProtectedTo(player);
        BallController.ChangeDirection();

        //Se o jogador selecionado do time adversario estiver proximo a mim na hora do lésinho, vou fazer ele tropeçar
        CampTeam adversary = player.GetCampTeam() == CampTeam.Team_A ? CampTeam.Team_B : CampTeam.Team_A;

        List<PlayerController> enemys = GameManager.instance.GetPlayersNearBall(adversary, 2.5f);
        if (enemys.Count > 0)
            foreach (PlayerController enemy in enemys)
                enemy.Locomotion.TriggerStumb();


    }
    private void OnChangeDirectionOk()
    {
        BallController.instance.SetBallDesprotectTo(player);
    }
    private void OnChangeDirectionFinish()
    {
        BallController.instance.SetBallDesprotectTo(player);
        player.UnsetKinematic();
    }
    //Turn Direction
    private void OnTurnDirectionStart()
    {
        player.SetKinematic();

        if (!player.IsMyBall())
            return;

        BallController.instance.SetBallProtectedTo(player);
    }
    private void OnTurnDirectionFinish()
    {
        BallController.instance.SetBallDesprotectTo(player);
        player.UnsetKinematic();
    }
    //Kick
    private void OnLongKickOk()
    {
        if (BallController.IsOwner(player))
            BallController.SetKick();
    }
    //Enttry
    private void OnEntryStart()
    {
        if (BallController.IsOwner(player))
            BallController.instance.SetBallProtectedTo(player);

        player.SetKinematic();

    }
    private void OnEntryFinish()
    {

        BallController.instance.SetBallDesprotectTo(player);
        player.UnsetKinematic();

    }
    //Pass
    private void OnShortPassOk()
    {


        if (BallController.IsOwner(player))
        {
            if (AiToPass != null)
                AiToPass.WaitPass();

            BallController.SetPass(powerToPass);
        }

        AiToPass = null;
        powerToPass = 0;


    }
    //Stumble
    private void OnStumbleStart()
    {
        player.SetKinematic();
    }
    private void OnStumbleFinish()
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
        animatorEvents.OnChangeDirStart -= OnChangeDirectionStart;
        animatorEvents.OnChangeDirOk -= OnChangeDirectionOk;
        animatorEvents.OnChangeDirFinish -= OnChangeDirectionFinish;
        animatorEvents.OnTurnStart -= OnTurnDirectionStart;
        animatorEvents.OnTurnFinish -= OnTurnDirectionFinish;
        animatorEvents.OnKickOk -= OnLongKickOk;
        animatorEvents.OnEnttryStart -= OnEntryStart;
        animatorEvents.OnEnttryFinish -= OnEntryFinish;
        animatorEvents.OnPassOk -= OnShortPassOk;
        animatorEvents.OnStumblesStart -= OnStumbleStart;
        animatorEvents.OnStumblesFinish -= OnStumbleFinish;

        BallController.instance.onSetMyOwner -= OnBallSetOwner;
        BallController.instance.onRemoveMyOwner -= OnBallRemoveOwner;

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
        animatorEvents.OnChangeDirStart += OnChangeDirectionStart;
        animatorEvents.OnChangeDirOk += OnChangeDirectionOk;
        animatorEvents.OnChangeDirFinish += OnChangeDirectionFinish;
        animatorEvents.OnTurnStart += OnTurnDirectionStart;
        animatorEvents.OnTurnFinish += OnTurnDirectionFinish;
        animatorEvents.OnKickOk += OnLongKickOk;
        animatorEvents.OnEnttryStart += OnEntryStart;
        animatorEvents.OnEnttryFinish += OnEntryFinish;
        animatorEvents.OnPassOk += OnShortPassOk;
        animatorEvents.OnStumblesStart += OnStumbleStart;
        animatorEvents.OnStumblesFinish += OnStumbleFinish;


        while (BallController.instance == null)
            yield return null;

        BallController.instance.onSetMyOwner += OnBallSetOwner;
        BallController.instance.onRemoveMyOwner += OnBallRemoveOwner;
    }

    //InternalMethods
    internal void TriggerPass(PlayerController toPlayer)
    {
        AiToPass = toPlayer.GetAiControlelr();
        powerToPass = player.Distance(toPlayer) * 2;
        locomotion.TriggerPass();
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

