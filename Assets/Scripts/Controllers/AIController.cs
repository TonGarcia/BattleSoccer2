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
    public float timeTodrible = 0.5f;

    public Collider FovBallTryger;

    private PlayerController player;
    private ControllerLocomotion locomotion { get { return player.Locomotion; } }
    private float direction { get { return player.dir; } set { player.dir = value; } }
    private float speed { get { return player.speed; } set { player.speed = value; } }

    private PlayerInput playerInput;
    private NavMeshAgent agent;
    private SoccerAIUnSelected AIUnselected;
    private SoccerAISelected AISelected;
    private SoccerAIwithBall AIWithBall;

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

        AIUnselected = new SoccerAIUnSelected(this);
        AISelected = new SoccerAISelected(this);
        AIWithBall = new SoccerAIwithBall(this);

        nvSpeed = agent.speed;
        nvAngularSpeed = agent.angularSpeed;
        nvAceleration = agent.acceleration;
    }

    void Update()
    {
        if (!agent || !playerInput || !player)
            return;

        UpdateNavMeshAgent();

        if (locomotion.inStumble) //Tropeçando
        {
            speed = 0;
            direction = 0;
            return;
        }

        //Se alguem fez um passe para mim, vou estar aguardando o passe. Neste caso
        //Vou ajudar correndo para perto da bola
        if (waitingToPass)
        {
            Vector3 target = BallController.GetPosition();
            target.y = player.transform.position.y;
            agent.SetDestination(target);

            Vector2 move = locomotion.GetDirectionAI();
            direction = move.x;
            speed = move.y;


            return;
        }

        if (player.IsMyBall())           //Estados de Jogador com a bola
        {
            AIWithBall.checkDistanceToDrible = distanceToDrible;
            AIWithBall.checkTimeToDrible = timeTodrible;
            AIWithBall.HandleStates();
        }
        else if (player.IsSelected())    //Estados de jogador sem bola mas selecionado 
        {
            AISelected.HandleStates();
        }
        else                            //Estado jogador sem bola e não selecionado
        {
            AIUnselected.HandleStates();
        }

    }
    //Unity Events
    private void OnEnable()
    {
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

    private void OnTurnDirectionStart()
    {
        player.SetKinematic();

        if (!player.IsMyBall())
            return;

        BallController.instance.SetBallProtectedTo(player);
    }
    private void OnTurnDirectionOk()
    {
        // BallController.instance.SetBallDesprotectTo(player);
    }
    private void OnTurnDirectionFinish()
    {
        BallController.instance.SetBallDesprotectTo(player);
        player.UnsetKinematic();
    }

    private void OnLongKickOk()
    {
        if (BallController.IsOwner(player))
            BallController.SetKick();
    }
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
    private void OnStumbleStart()
    {
        player.SetKinematic();
    }
    private void OnStumbleFinish()
    {
        player.UnsetKinematic();
    }

    //Private methods

    private void SignEvents()
    {
        StartCoroutine(IESignevents());
    }
    private void UnsignEvents()
    {

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
        while (BallController.instance == null)
            yield return null;

        BallController.instance.onSetMyOwner += OnBallSetOwner;
        BallController.instance.onRemoveMyOwner += OnBallRemoveOwner;
    }

    //InternalMethods
    internal void TriggerPass(AIController toController, float power)
    {
        AiToPass = toController;
        powerToPass = power;
        locomotion.TriggerPass();
    }
    internal void WaitPass()
    {
        waitingToPass = true;
    }
}

