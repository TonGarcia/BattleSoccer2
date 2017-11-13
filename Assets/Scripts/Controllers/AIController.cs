using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{

    public enum AIState
    {
        nothing,
        goToDefaultPosition,
        followBall,
        marcando,
        goToGoal,


    }

    [Tooltip("Se o jogador tomar a bola e estiver a uma distancia igual ou menor, a animação de tomada de bola sera executada")]
    public float distanceToEntry = 2.5f;
    public Collider FovBallTryger;

    private PlayerController player;
    private ControllerLocomotion locomotion { get { return player.Locomotion; } }
    private PlayerInput playerInput;
    private NavMeshAgent agent;

    private float dir { get { return player.dir; } set { player.dir = value; } }
    private float speed { get { return player.speed; } set { player.speed = value; } }
    private AIState aiState;

    private float nvSpeed, nvAngularSpeed, nvAceleration;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        nvSpeed = agent.speed;
        nvAngularSpeed = agent.angularSpeed;
        nvAceleration = agent.acceleration;
    }

    void Update()
    {
        if (!agent || !playerInput || !player)
            return;

        UpdateNavMeshAgent();

        if(player.IsMyBall())
        {

        }
        else
        {
            switch (aiState)
            {
                case AIState.nothing:
                    Handlle_NothingState();
                    break;
                case AIState.goToDefaultPosition:
                    Handle_GoToDefaultMarcation();
                    break;
                case AIState.followBall:
                    break;
                case AIState.goToGoal:
                    break;
                case AIState.marcando:
                    Handle_MarcandoState();
                    break;
            }
        }
       
    }

    private void OnEnable()
    {
        aiState = AIState.nothing;

    }
    /// <summary>
    /// Verdadeiro se o Agent em questão ja esta no trajeto espisificado após uma 
    /// trajetória anterior
    /// </summary>
    public bool IsAgentDone
    {
        get
        {
            return !agent.pathPending && IsAgentStopping;
        }
    }
    /// <summary>
    /// Verdadeiro se o agent esta parado no momento
    /// </summary>
    public bool IsAgentStopping
    {
        get
        {
            if (agent.enabled == false)
                return true;

            return agent.remainingDistance <= agent.stoppingDistance;
        }
    }
    private void Handlle_NothingState()
    {
        //Indo para a origem
        //Se eu não estiver no ponto de origem vou ir para ele.
        //Se a bola estiver com o time o ponto de marcao é o de ataque
        //se a bola não estiver a marcação é o de desefesa
        //Se eu ja estiver na origem entao vou marcar o jogador mais proximo caso a bola esteja muito longe de mim
        //Se a bola estiver perto entao vou ir atraz dela

        bool inMarcation = player.InMarcation(1.5f);

        if (!inMarcation)
        {           
            aiState = AIState.goToDefaultPosition;
            return;
        }


        //Olhando para a bola
        speed = 0;
        dir = locomotion.GetDirection(BallController.GetPosition()).x;


        /*
                  //Indo atraz da bola se estiver perto
                  bool isBallNear = player.IsBallNear();
                  if (!isBallNear)
                  {
                      aiState = AIState.followBall;
                      return;
                  }

                  //Se eu estiver na defesa, vou marcar o jogador da minha posição no time adversario mas se eu esiver no ataque
                  //por hora vou ficar parado.
                  bool teamHasBall = player.IsBallfromMyTeam();
                  if (!teamHasBall)
                  {
                      PlayerController enemy = player.GetEnemyInMarcation();
                      if (enemy != null)
                      {
                          aiState = AIState.marcando;
                          return;
                      }
                  }
          */






    }
    private void Handle_GoToDefaultMarcation()
    {
        //Vou para a minha posição de marcação origem
        //Marcação de dfesa caso meu time nao tenha a bola, marcação de ataque caso tenha.
        //Vou finalizar quando:
        //> Eu estiver com a bola
        //> Eu chegar ao destino

        //Eu mesmo possuo a bola
        if (player.IsMyBall())
        {
            aiState = AIState.nothing;
            return;
        }

        Transform defPos = player.GetMarcationPosition(CampPlaceType.defense);
        Transform attkPos = player.GetMarcationPosition(CampPlaceType.attack);
        bool myTeamHasBall = player.IsBallfromMyTeam();

        Vector2 move = locomotion.GetDirectionAI();
        speed = move.y;
        dir = move.x;
        agent.destination = myTeamHasBall ? attkPos.position : defPos.position;

        //Eu cheguei ao destino
        if (IsAgentDone)
        {
            aiState = AIState.nothing;
            return;
        }
        
    }
    private void Handle_MarcandoState()
    {
        Debug.Log("Marcando");
        /*
        //Se meu time estiver na defesa e houver um jogador inimigo de marcação proximo a mim, vou marcar
        //Minha marcação vai terminar caso
        //>O joagdor se afaste de mais de mim
        //>Eu ou o Meu time fique de poce da bola
        //Se eu me machucar

        //Verifico se meu time esta de poce de bola
        bool teamHasBall = player.IsBallfromMyTeam();
        if (teamHasBall)
        {
            aiState = AIState.nothing;
            return;
        }

        //Verifico se o inimigo ainda existe ou se ele se afastou de mais
        PlayerController enemy = player.GetEnemyInMarcation();
        if (enemy == null)
        {
            aiState = AIState.nothing;
            return;
        }
        float distance = enemy.Distance(player);
        if (distance > 10.5f)
        {
            aiState = AIState.nothing;
            return;
        }

        //Existe inimigo, esotu proximo dele e estou na defesa. Bora seguir 
        //Vou pegar o ponto um pouco a frente do inimigo e da bola e entao vou ficar entre os 2.

        //Posição da bola
        Vector3 ballPosition = BallController.GetPosition();
        //Posição do inimigo
        Vector3 enemyPos = enemy.transform.position;
        //Direção inimigo bola
        Vector3 direction = ballPosition - enemyPos;
        //Posição entre o inimigo e a bola
        Vector3 between = enemyPos + (direction * 2.5f);
        
        //Seta trajeto para a posição between
        */
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
            agent.acceleration = 1;
        }
    }
}
