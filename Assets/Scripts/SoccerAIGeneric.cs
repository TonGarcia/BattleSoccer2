using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;
using UnityEngine.AI;
using System;

public enum SoccerAIState
{
    nothing,
    goToDefaultPosition,
    followBall,
    marcando,
    goToGoal,


}

public abstract class SoccerAIGeneric
{

    public GameObject Owner
    {
        get { return owner; }
    }
    private GameObject owner;

    public ControllerLocomotion Locomotion { get { return player.Locomotion; } }

    public PlayerController Player
    {
        get
        {
            if (player == null)
                player = owner.GetComponent<PlayerController>();
            return player;
        }
    }
    private PlayerController player;


    public PlayerInput InputPlayer
    {
        get
        {
            if (playerInput == null)
                playerInput = owner.GetComponent<PlayerInput>();
            return playerInput;
        }
    }
    private PlayerInput playerInput;

    public NavMeshAgent Agent
    {
        get
        {
            if (agent == null)
                agent = owner.GetComponent<NavMeshAgent>();

            return agent;
        }
    }
    private NavMeshAgent agent;

    public float Direction
    {
        get
        {
            if (player == null)
                player = owner.GetComponent<PlayerController>();
            return player.dir;
        }
        set
        {
            if (player == null)
                player = owner.GetComponent<PlayerController>();
            player.dir = value;
        }
    }
    public float Speed
    {
        get
        {
            if (player == null)
                player = owner.GetComponent<PlayerController>();
            return player.speed;
        }
        set
        {
            if (player == null)
                player = owner.GetComponent<PlayerController>();
            player.speed = value;
        }
    }

    public SoccerAIGeneric(GameObject owner)
    {
        this.owner = owner;
    }

    public abstract void HandleStates();

}

public class SoccerAIUnSelected : SoccerAIGeneric
{
    public SoccerAIState AiState { get { return aiState; } }
    private SoccerAIState aiState;

    public SoccerAIUnSelected(GameObject owner) : base(owner)
    {
        aiState = SoccerAIState.nothing;
    }

    public override void HandleStates()
    {
        switch (aiState)
        {
            case SoccerAIState.nothing:
                Handlle_NothingState();
                break;
            case SoccerAIState.goToDefaultPosition:
                Handle_GoToDefaultMarcation();
                break;
            case SoccerAIState.followBall:
                break;
            case SoccerAIState.goToGoal:
                break;
            case SoccerAIState.marcando:
                Handle_MarcandoState();
                break;
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

        bool inMarcation = Player.InMarcation(1.5f);

        if (!inMarcation)
        {
            aiState = SoccerAIState.goToDefaultPosition;
            return;
        }


        //Olhando para a bola
        Speed = 0;
        Direction = Locomotion.GetDirection(BallController.GetPosition()).x;


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
        if (Player.IsMyBall())
        {
            aiState = SoccerAIState.nothing;
            return;
        }

        Transform defPos = Player.GetMarcationPosition(CampPlaceType.defense);
        Transform attkPos = Player.GetMarcationPosition(CampPlaceType.attack);
        bool myTeamHasBall = Player.IsBallfromMyTeam();

        Vector2 move = Locomotion.GetDirectionAI();
        Speed = move.y;
        Direction = move.x;
        Agent.destination = myTeamHasBall ? attkPos.position : defPos.position;

        //Eu cheguei ao destino
        if (IsAgentDone)
        {
            aiState = SoccerAIState.nothing;
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
    private bool IsAgentDone
    {
        get
        {
            return !Agent.pathPending && IsAgentStopping;
        }
    }
    private bool IsAgentStopping
    {
        get
        {
            if (Agent.enabled == false)
                return true;

            return Agent.remainingDistance <= Agent.stoppingDistance;
        }
    }
}
public class SoccerAISelected : SoccerAIGeneric
{
    public SoccerAIState AiState { get { return aiState; } }
    private SoccerAIState aiState;
    private float timeToSelect = 0;

    public SoccerAISelected(GameObject owner) : base(owner)
    {
        aiState = SoccerAIState.nothing;
    }

    public override void HandleStates()
    {
        if (!Agent.isActiveAndEnabled || !Agent.isOnNavMesh)
            return;



        aiState = SoccerAIState.followBall;

        switch (aiState)
        {
            case SoccerAIState.nothing:
                Handlle_NothingState();
                break;
            case SoccerAIState.followBall:
                Handle_FollowBallState();
                break;
            default:
                Handlle_NothingState();
                break;
        }
    }

    private void Handlle_NothingState()
    {
        //Indo para a origem
        //Se eu estiver proximo do jogador que possui a bola vou correr atraz da bola
        //Se eu estiver longe Vou passar a seleção para o jogador mais proximo da bola e se não houver vou atraz
        //da bola procurando um novo jogador 
        if (!Player.IsMyBall() && Player.IsSelected())
        {
            aiState = SoccerAIState.followBall;
            return;
        }

        timeToSelect = 0;
        Speed = 0;
        Direction = 0;


    }
    private void Handle_FollowBallState()
    {
        //Corre atraz da bola se ela estiver a uma distancia aceitavel ou procura um jogador mais proximo
        //enquanto continua indo atraz da bola.
        //Ação finalizada se: Jogador deixar de estar selecionado ou pegar a bola

        if (Player.IsMyBall() || !Player.IsSelected())
        {
            aiState = SoccerAIState.nothing;
            return;
        }

        timeToSelect += Time.deltaTime;

        BallController ball = BallController.instance;
        float balldistance = ball.transform.Distance(Player.transform);

        //Corre atraz da bola
        Vector2 move = Locomotion.GetDirectionAI();
        Speed = move.y;
        Direction = move.x;
        Agent.destination = ball.transform.position;

        //Verifica a distancia da bola, se estiver muito longe procuro outor jogador mais proximo para selecionar

        if (timeToSelect > 2.5f)
        {
            if (balldistance >= 5.0f) //Procurando jogador mais proximo
            {
                PlayerController nearBall = GameManager.instance.GetPlayerNearBall(Player.GetCampTeam());
                if (nearBall != Player)
                    Player.SelectME();

                timeToSelect = 0.0f;
            }
        }

    }
    private bool IsAgentDone
    {
        get
        {
            return !Agent.pathPending && IsAgentStopping;
        }
    }
    private bool IsAgentStopping
    {
        get
        {
            if (Agent.enabled == false)
                return true;

            return Agent.remainingDistance <= Agent.stoppingDistance;
        }
    }
}
public class SoccerAIwithBall : SoccerAIGeneric
{
    public SoccerAIState AiState { get { return aiState; } }
    private SoccerAIState aiState;

    public SoccerAIwithBall(GameObject owner) : base(owner)
    {
        aiState = SoccerAIState.nothing;
    }

    public override void HandleStates()
    {
        switch (aiState)
        {
            case SoccerAIState.nothing:
                Handlle_NothingState();
                break;

            case SoccerAIState.goToGoal:
                break;

            default:
                Handlle_NothingState();
                break;
        }
    }

    private void Handlle_NothingState()
    {


        Speed = 0;
        Direction = 0;

    }

    private bool IsAgentDone
    {
        get
        {
            return !Agent.pathPending && IsAgentStopping;
        }
    }
    private bool IsAgentStopping
    {
        get
        {
            if (Agent.enabled == false)
                return true;

            return Agent.remainingDistance <= Agent.stoppingDistance;
        }
    }
}
