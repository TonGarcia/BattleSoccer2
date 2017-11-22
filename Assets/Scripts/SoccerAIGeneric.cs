using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;
using UnityEngine.AI;
using System;
using System.Linq;

public static class SoccerAIGenericExtensions
{
    public static SoccerAIUnSelected ToUnSelected(this SoccerAIGeneric generic)
    {
        return (SoccerAIUnSelected)generic;
    }
    public static SoccerAISelected ToSelected(this SoccerAIGeneric generic)
    {
        return (SoccerAISelected)generic;
    }
    public static SoccerAIwithBall ToWithBall(this SoccerAIGeneric generic)
    {
        return (SoccerAIwithBall)generic;
    }

}
public enum SoccerAIState
{
    nothing,
    goToDefaultPosition,
    followBall,
    marcando,
    findToPass,
    goToGoal,
    drible,
}
public abstract class SoccerAIGeneric
{

    public AIController Owner
    {
        get { return owner; }
    }
    private AIController owner;

    public ControllerLocomotion Locomotion { get { return Player.Locomotion; } }


    public PlayerController Player { get { return player; } }
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

    public LocomotionType motionType;

    private bool isForcedGoTo = false;

    private Vector3 forceGoTo;
    private Transform forceLoockat;

    public SoccerAIGeneric(AIController owner, PlayerController controller)
    {
        this.owner = owner;
        this.player = controller;
        motionType = LocomotionType.normal;
    }

    public virtual bool UpdateHandleStates()
    {
        if (Locomotion == null)
            return false;

        Locomotion.motionType = motionType;

        if (Locomotion.inStumble) //Tropeçando
        {
            Agent.destination = Player.transform.position;
            Speed = 0;
            Direction = 0;
            return false;
        }

        if (isForcedGoTo)
        {           
            if (Player.IsMyBall())
            {
                Stop();
                isForcedGoTo = false;
                return true;
            }

            Vector2 move = Locomotion.GetDirectionAI();
            Speed = move.y;
            Direction = move.x;

            //Lookat
            Vector3 loockposition = forceLoockat.position;
            loockposition.y = Player.transform.position.y;
            Player.transform.LookAt(loockposition);

            //Move AI
            Agent.destination = forceGoTo;

            if (Locomotion.IsAgentDone)
            {
                Stop();
                isForcedGoTo = false;
            }

            return false;
        }

        return true;
    }
    public virtual void StopHandleStates()
    {
        Stop();

    }
    public virtual void ForceGoTo(Vector3 position)
    {
        if (isForcedGoTo)
            return;

        Stop();

        isForcedGoTo = true;
        forceGoTo = position;
        Agent.destination = forceGoTo;
    }
    public virtual void ForceGoTo(Vector3 position, Transform loockat)
    {
        if (isForcedGoTo)
            return;

        Stop();

        this.motionType = LocomotionType.strafe;

        isForcedGoTo = true;
        forceLoockat = loockat;
        forceGoTo = position;
        Agent.destination = forceGoTo;
    }

    protected void Move(Vector3 position)
    {
        Agent.SetDestination(position);
        //Move para o destino
        Vector2 move = Locomotion.GetDirectionAI();
        Speed = move.y;
        Direction = move.x;
    }
    protected void Lookat(Vector3 position)
    {
        //Move para o destino
        Agent.SetDestination(Player.transform.position);

        Vector2 move = Locomotion.GetDirection(position);
        Speed = 0;
        Direction = move.x;
    }
    protected void Stop()
    {
        if (Agent.isOnNavMesh && Agent.enabled)
            Agent.destination = Player.transform.position;


        Speed = 0;
        Direction = 0;
        motionType = LocomotionType.normal;
        Player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Player.GetComponent<Animator>().rootPosition = Player.transform.position;
        isForcedGoTo = false;
        forceGoTo = Player.transform.position;

    }
}

public class SoccerAIUnSelected : SoccerAIGeneric
{
    public SoccerAIState AiState { get { return aiState; } }
    private SoccerAIState aiState;

    public SoccerAIUnSelected(AIController owner, PlayerController controller) : base(owner, controller)
    {
        aiState = SoccerAIState.nothing;
    }

    public override bool UpdateHandleStates()
    {
        if (base.UpdateHandleStates() == false)
            return false;


        switch (aiState)
        {
            case SoccerAIState.nothing:
                Handlle_NothingState();
                break;
            case SoccerAIState.goToDefaultPosition:
                Handle_GoToDefaultState();
                break;
            case SoccerAIState.followBall:
                Handle_FollowBallState();
                break;
            case SoccerAIState.goToGoal:
                break;
            case SoccerAIState.marcando:
                Handle_MarcandoState();
                break;
        }

        return true;
    }

    private void Handlle_NothingState()
    {
        //Indo para a origem
        //Se eu não estiver no ponto de origem vou ir para ele.
        //Se a bola estiver com o time o ponto de marcao é o de ataque
        //se a bola não estiver a marcação é o de desefesa
        //Se eu ja estiver na origem entao vou marcar o jogador mais proximo caso a bola esteja muito longe de mim
        //Se a bola estiver perto entao vou ir atraz dela

        //Indo atraz da bola se estiver perto e nao estiver no pe do meu time
        bool isBallNear = Player.IsBallMostNear();
        bool teamHasBall = Player.IsBallfromMyTeam();

        if (isBallNear == true && !teamHasBall)
        {           
            
            aiState = SoccerAIState.followBall;
            return;
        }

        //Indo para posição padrão
        bool inMarcation = Player.InMarcation(1.5f);
        bool inSelection = Player.IsSelected();
        bool ballHasOwner = BallController.HasOwner();

        if (!inMarcation && !inSelection && ballHasOwner)
        {
            Stop();
            aiState = SoccerAIState.goToDefaultPosition;
            return;
        }

        //Marcando jogador mais proximo
        //...

        //Olhando para a bola
        Agent.destination = Player.transform.position;
        Lookat(BallController.GetPosition());


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
    private void Handle_GoToDefaultState()
    {
        //Vou para a minha posição de marcação origem
        //Marcação de dfesa caso meu time nao tenha a bola, marcação de ataque caso tenha.
        //Vou finalizar quando:
        //> Eu estiver com a bola
        //> Eu chegar ao destino
        //> Time estiver com bola
        //> Eu estiver perto da bola

        if (Player.IsSelected() || BallController.HasOwner() == false)
        {
            Stop();
            aiState = SoccerAIState.nothing;
            return;
        }

       
        bool isBallNear = Player.IsBallNear();

        if (isBallNear == true && BallController.IsFromTeam(Player) == false)
        {
            Speed = 0;
            aiState = SoccerAIState.nothing;
            return;
        }

        Transform defPos = Player.GetMarcationPosition(CampPlaceType.defense);
        Transform attkPos = Player.GetMarcationPosition(CampPlaceType.attack);
        bool myTeamHasBall = Player.IsBallfromMyTeam();

        Vector2 move = Locomotion.GetDirectionAI();
        Speed = move.y;
        Direction = move.x;
        Vector3 destination = myTeamHasBall ? attkPos.position : defPos.position;
        Agent.SetDestination(destination);

        //Eu cheguei ao destino
        if (Locomotion.IsAgentDone)
        {
            Stop();
            aiState = SoccerAIState.nothing;
            return;
        }

    }
    private void Handle_FollowBallState()
    {
        //Indo atraz da bola se estiver perto
        bool isBallNear = Player.IsBallMostNear();
        bool teamHasBall = Player.IsBallfromMyTeam();

        if (isBallNear == false || teamHasBall == true || Player.IsMyBall() || Player.IsSelected())
        {
            Stop();
            aiState = SoccerAIState.nothing;
            return;
        }

        Vector3 ballPosition = BallController.GetPosition();
        ballPosition.y = Player.transform.position.y;


        Vector2 move = Locomotion.GetDirectionAI();
        Direction = move.x;
        Speed = move.y;
        Agent.SetDestination(ballPosition);

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

}
public class SoccerAISelected : SoccerAIGeneric
{
    public SoccerAIState AiState { get { return aiState; } }
    private SoccerAIState aiState;
    private float timeToSelect = 0;

    public SoccerAISelected(AIController owner, PlayerController controller) : base(owner, controller)
    {
        aiState = SoccerAIState.nothing;
    }

    public override bool UpdateHandleStates()
    {
        if (base.UpdateHandleStates() == false)
            return false;

        if (!Agent.isActiveAndEnabled || !Agent.isOnNavMesh)
            return false;

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

        return true;
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
        Agent.destination = Player.transform.position;


    }
    private void Handle_FollowBallState()
    {
        //Corre atraz da bola se ela estiver a uma distancia aceitavel ou procura um jogador mais proximo
        //enquanto continua indo atraz da bola.
        //Ação finalizada se: Jogador deixar de estar selecionado ou pegar a bola

        if (Player.IsMyBall() || !Player.IsSelected())
        {
            Agent.destination = Player.transform.position;
            Speed = 0;
            Direction = 0;

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
        Vector3 destination = ball.transform.position;
        Agent.SetDestination(destination);

        //Verifica a distancia da bola, se estiver muito longe procuro outor jogador mais proximo para selecionar

        if (timeToSelect > 1.5f)
        {
            if (balldistance > 3.5f) //Procurando jogador mais proximo
            {
                PlayerController nearBall = GameManager.instance.GetPlayerNearBall(Player.GetCampTeam());
                if (nearBall != Player)
                {
                    nearBall.SelectME();
                    timeToSelect = 0.0f;
                }
            }
        }

    }

}
public class SoccerAIwithBall : SoccerAIGeneric
{
    public float checkDistanceToPass = 10.0f;
    public float checkDistanceToDrible = 2.5f;
    /// <summary>
    /// Dribles serao feito a cada fração de tempo aqui estipulado. valores maiores
    /// deixam os movimentos de drible uma vez que permanece mais tempo na nova rota escolhida
    /// e valores mais baixos deixam o player mais agitado uma vez que procura novas rotas em intervalos muito curto.
    /// Valores indicados são de 0.5 a 1.5f.
    /// </summary>
    public float checkTimeToDrible = 0.5f;

    public SoccerAIState AiState { get { return aiState; } }
    private SoccerAIState aiState;

    private float timeToGoal = 0;
    private float timeToDrible = 100;

    private Vector3 MidCampPosition { get { return GameManager.instance.midCampTransform.position; } }
    Vector3 toGo = Vector3.zero;
    PlayerController toPass = null;
    private bool inPass = false;

    public SoccerAIwithBall(AIController owner, PlayerController controller) : base(owner, controller)
    {
        aiState = SoccerAIState.nothing;
        PlayerAnimatorEvents animatorEvents = Player.GetComponent<PlayerAnimatorEvents>();
        animatorEvents.OnPassFinish += OnPassFinish;

    }

    ~SoccerAIwithBall()
    {
        if (Player == null)
            return;
        PlayerAnimatorEvents animatorEvents = Player.gameObject.GetComponent<PlayerAnimatorEvents>();
        animatorEvents.OnPassFinish -= OnPassFinish;
    }

    public override bool UpdateHandleStates()
    {
        if (base.UpdateHandleStates() == false)
            return false;

        if (!Agent.isActiveAndEnabled || !Agent.isOnNavMesh)
            return false;

        Handlle_NothingState();

        return true;
    }

    private void Handlle_NothingState()
    {
        if (Player.IsMyBall() == false)
        {
            aiState = SoccerAIState.nothing;
            return;
        }

        Transform goalPosition = Player.GetEnemyGoalPosition();
        PlayerController playerBtw = null;
        timeToDrible += Time.deltaTime;

        if (toPass != null)
        {
            motionType = LocomotionType.normal;

            Debug.DrawLine(Player.transform.position, toPass.transform.position, Color.red);

            toGo = toPass.transform.position;

            if (Player.IsLookAt(toGo) == true && inPass == false)
            {
                if (Player.IsHitBetween(toPass) == false)
                {
                    Owner.TriggerPass(toPass);
                    inPass = true;
                }
                else
                {
                    toPass = null;
                }
            }

            Move(toGo);
            return;
        }

        //Vou passar a bola se existir um cara livre entre e o gol. Esta e a prioridade ja q ele tem caminho livre
        if (Player.IsHitForwad(checkDistanceToPass, out playerBtw))
        {
            motionType = LocomotionType.soccer;

            if (playerBtw.IsMyTeaM(Player)) //Player e meu amigo, vou pedir para ele sair do caminho
            {
                Vector3 origim = playerBtw.transform.position + (-playerBtw.transform.forward * 4.5f);
                Vector3 freePos = Locomotion.GetRandomNavCircle(origim, 4.5f);
                playerBtw.GetComponent<AIController>().GoToPosition(freePos, BallController.instance.transform);
            }
            else if (playerBtw.Distance(Player) <= checkDistanceToDrible) //Player inimigo e perto de mais. Drible
            {
                Vector3 positionToDrible = Vector3.zero;

                if (GetPositionToDrible(playerBtw, out positionToDrible))
                    toGo = positionToDrible;
                else
                {
                    //Se posição boa para o drible.
                    toGo = positionToDrible;
                }
            }
            else //Player inimigo mas longe de mais vou tentar um passe de bola
            {

                PlayerController topass = null;
                if (GetToPass(out topass))
                {
                    toPass = topass;
                    toGo = Player.transform.position;
                    //Stop();
                }
                else
                {
                    toGo = goalPosition.position;
                }
            }
        }
        else
        {
            timeToGoal += Time.deltaTime;
            if (Locomotion.IsAgentDone || timeToGoal >=1.5f)
            {
                motionType = LocomotionType.normal;
                toGo = goalPosition.position;
                timeToGoal = 0;

            }
        }

        Move(toGo);

    }

    private bool GetPositionToDrible(PlayerController enemy, out Vector3 toGo)
    {
        bool result = false;
        Vector3 resultPosition = Vector3.zero;
        Vector3 freePos = Vector3.zero;

        if (GetFreeHit(out freePos, enemy.transform.position, true))
        {
            resultPosition = freePos;
            result = true;
        }
        else
        {
            Vector3 pos = Locomotion.GetRandomNavCircle(Player.transform.position, 3.5f);
            resultPosition = pos;

            if (Player.IsHitBetween(pos) == false)
            {

                result = true;
            }
        }

        toGo = resultPosition;
        return result;
    }
    private bool GetToPass(out PlayerController playerPassed)
    {
        bool result = false;
        PlayerController selec = null;

        //Procura jogadores proximo de mim que estejam livres
        List<PlayerController> players = GameManager.instance.GetPlayersNearBall(Player.GetCampTeam(), checkDistanceToPass);
        List<PlayerController> playersNear = new List<PlayerController>(players);
        playersNear.RemoveAll(p => p.IsHitBetween(Player));
        playersNear.Remove(Player);
        playersNear.Remove(BallController.GetLastOwner());

        if (playersNear.Count > 0) //Existem jogadores proximos
        {

            if (playersNear.Exists(r => r.GetCampAction() == CampActionAttribute.attack))
            {
                playersNear.RemoveAll(r => r.GetCampAction() != CampActionAttribute.attack);
                float max = playersNear.Max(r => r.Distance(Player));
                selec = playersNear.FirstOrDefault(r => r.Distance(Player) == max);

            }
            else if (playersNear.Exists(r => r.GetCampAction() == CampActionAttribute.middle))
            {
                playersNear.RemoveAll(r => r.GetCampAction() != CampActionAttribute.middle);
                float min = playersNear.Min(r => r.Distance(Player));
                selec = playersNear.FirstOrDefault(r => r.Distance(Player) == min);
            }
            else
            {
                float min = playersNear.Min(r => r.Distance(Player));
                selec = playersNear.FirstOrDefault(r => r.Distance(Player) == min);
            }

            if (selec != null)
                result = true;
        }

        playerPassed = selec;
        return result;
    }
    private void Handle_ToGoalStateOLD()
    {
        /*
        timeToDrible += Time.deltaTime;
        timeToPass += Time.deltaTime;

        Transform goalPosition = Player.GetEnemyGoalPosition();
        PlayerController playerBtw = GetAnyBtwForward(checkDistanceToDrible);

        if (playerBtw != null)
        {
            if (!playerBtw.IsMyTeaM(Player))//Jogador inimigo a frente
            {
                //Drible
                if (timeToDrible >= checkTimeToDrible) //Nova posição para o drible
                {
                    Vector3 pos = Locomotion.GetRandomNavCircle(Player.transform.position, 2.5f);
                    toGo = pos;
                    timeToDrible = 0.0f;
                }
            }
            else //Jogador amigo a frente
            {
                //Passe
                float dist = playerBtw.Distance(Player);

                if (dist <= checkDistanceToDrible / 2)
                {
                    //Drible
                    if (timeToDrible >= checkTimeToDrible) //Nova posição para o drible
                    {
                        Vector3 pos = Locomotion.GetRandomNavCircle(Player.transform.position, 2.5f);
                        toGo = pos;
                        timeToDrible = 0.0f;
                        timeToPass = 0.0f;
                    }
                }
                else
                {
                    if (timeToPass >= 1.5f)
                    {
                        toGo = playerBtw.transform.position;
                        Owner.TriggerPass(playerBtw.GetComponent<AIController>(), dist * 2);
                        timeToPass = 0.0f;
                    }
                }
            }
        }

        if (IsAgentDone)
        {
            if (Player.InMarcation(1.5f) == false)
            {
                toGo = goalPosition.position;
            }
        }

        if (Player.speed <= 0.1f)
        {
            timeToStand += Time.deltaTime;
            if (timeToStand > 1.0f)
            {
                toGo = goalPosition.position;
                timeToStand = 0;
            }
        }
        else
        { timeToStand = 0; }

        Vector2 move = Locomotion.GetDirectionAI();
        Direction = move.x;
        Speed = move.y;
        Agent.SetDestination(toGo);*/
    }
    private bool GetFreeHit(out Vector3 positionFree, Vector3 lefRightCEnter, bool inverser = false)
    {
        int leftRight = (int)Player.LeftRightDir(lefRightCEnter);
        if (inverser)
            leftRight *= -1;

        Vector3 freePos = Vector3.zero;
        bool hasfree = false;

        if (leftRight != 0)
        {
            hasfree = Player.GetFreeHitLeftOrRight(checkDistanceToDrible, leftRight, out freePos);
        }


        positionFree = freePos;
        return hasfree;
    }

    //Eventos de jogador
    private void OnPassFinish()
    {
        inPass = false;
        toPass = null;
    }

}
