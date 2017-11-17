using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SoccerGame;

public static class GameManagerExtensions
{
    public static Transform GetMarcationPosition(this PlayerController controller, CampPlaceType placeType)
    {
        return GameManager.instance.GetMarcationPosition(controller, placeType);
    }
    public static bool InMarcation(this PlayerController controller, CampPlaceType placeType, float relativeDistance = 2.5f)
    {
        bool result = false;
        Transform marcation = controller.GetMarcationPosition(placeType);
        if (marcation != null)
        {
            float distance = controller.transform.Distance(marcation);
            if (distance <= relativeDistance)
                result = true;
        }

        return result;
    }
    public static bool InMarcation(this PlayerController controller, float relativeDistance = 2.5f)
    {
        bool myTeamHasBall = controller.IsBallfromMyTeam();

        CampPlaceType myplace = myTeamHasBall ? CampPlaceType.attack : CampPlaceType.defense;

        return InMarcation(controller, myplace, relativeDistance);
    }
    public static PlayerController GetEnemyInMarcation(this PlayerController controller)
    {
        PlayerController enemy = GameManager.instance.GetEnemyPlayerInMarcation(controller);
        return enemy;
    }
    public static bool IsSelected(this PlayerController controller)
    {
        bool result = false;

        if (GameManager.instance.IsSelectedPlayer(controller))
            result = true;

        return result;
    }
    public static Transform GetTeamGoalPosition(this PlayerController controller)
    {
        return GameManager.instance.GetMyTeamGoalPosition(controller);
    }
    public static Transform GetEnemyGoalPosition(this PlayerController controller)
    {
        return GameManager.instance.GetEnemyGoalPosition(controller);
    }
    public static void SelectME(this PlayerController controller)
    {
        GameManager.instance.SelectPlayer(controller);
    }
}
public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class TeamManager
    {
        public CampTeam team;
        public CampPlaceSide side;
        public ControllerInputType controllerType;
        public Transform goalPosition;
        
        [SerializeField]
        private List<PlayerController> players;
        public List<PlayerController> Players { get { return new List<PlayerController>(players); } }

        [SerializeField]
        private MultiSelection multSelection;
        public MultiSelection MultSelection { get { return multSelection; } }

        public void Start()
        {
            if (autoFoundPlayers)
                players = new List<PlayerController>();

            List<PlayerController> _players = FindObjectsOfType<PlayerController>().ToList();
            _players.RemoveAll(r => r.GetCampTeam() != team);
            SetPlayers(_players);

            multSelection.SetTeam(team);
        }
        public bool autoFoundPlayers = false;
        public void SetPlayers(List<PlayerController> players)
        {
            this.players.AddRange(players);
        }

        public PlayerController GetPlayerInMarcation(CampPlaceMarcation marcation)
        {
            PlayerController result = null;
            if (players.Count > 0)
            {
                result = players.Find(r => r.GetPlaceMarcation() == marcation);

            }
            return result;
        }
    }

    public static GameManager instance;

    [SerializeField]
    private CampPositionsManager placesManager;
    [SerializeField]
    private TeamManager teamMananger1;
    [SerializeField]
    private TeamManager teamMananger2;
    
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        teamMananger1.Start();
        teamMananger2.Start();
    }
    private TeamManager GetTeamManager(CampTeam team)
    {
        TeamManager manager = null;
        if (teamMananger1.team == team)
            manager = teamMananger1;
        else
            manager = teamMananger2;

        return manager;
    }
    private TeamManager GetOtherTeamManager(CampTeam team)
    {
        TeamManager manager = null;
        if (teamMananger1.team != team)
            manager = teamMananger1;
        else
            manager = teamMananger2;

        return manager;
    }
    /// <summary>
    /// Pesquisa em um time especifico pelo jogador mais proximo da bola. 
    /// </summary>
    /// <param name="team">Time da pesquisa</param>
    /// <returns>Retorna o Jogador mais proximo da bola, Nunca retorna nulo a menos que o time não tenha jogador nenhum</returns>
    public PlayerController GetPlayerNearBall(CampTeam team)
    {
        TeamManager manager = GetTeamManager(team);

        List<PlayerController> players = manager.Players;
        if (players.Count > 0)
        {
            float min = players.Min(r => r.transform.Distance(BallController.instance.transform));
            PlayerController player = players.FirstOrDefault(r => r.transform.Distance(BallController.instance.transform) == min);


            return player;
        }
        else
        {
            return null;
        }

    }
    public PlayerController GetSelectedPlayer(CampTeam team)
    {
        TeamManager manager = GetTeamManager(team);
        return manager.MultSelection.GetSelectedPlayer();

    }
    public List<PlayerController> GetPlayersNearBall(CampTeam team, float near)
    {
        TeamManager manager = GetTeamManager(team);
        List<PlayerController> players = manager.Players;

        players.RemoveAll(r => r.transform.Distance(BallController.instance.transform) > near);

        return players;
    }
    public bool IsSelectedPlayer(PlayerController player)
    {
        TeamManager manager = GetTeamManager(player.GetCampTeam());
        return manager.MultSelection.GetSelectedPlayer() == player;
    }
    /// <summary>
    /// Pesquisa e retorna o jogador do time adversario na mesma marcação que o jogador pesqiusado. Tenha em mente
    /// que e o jogador retornado nao precisa estar necessáriamente proxima e ou que esta pesquisa pode retornar nula
    /// se o time adversário não tiver nenhum jogador nesta marcação
    /// </summary>
    /// <param name="player"></param>
    /// <returns>Jogador asversario na mesma marcação e ou nullo se não houver jogador do time adversario nesta marcação</returns>
    public PlayerController GetEnemyPlayerInMarcation(PlayerController player)
    {
        CampTeam campteam = player.GetCampTeam();
        CampTeam otherCampTeam = campteam == CampTeam.Team_A ? CampTeam.Team_B : CampTeam.Team_A;
        TeamManager team = GetTeamManager(otherCampTeam);
        PlayerController enemy = team.GetPlayerInMarcation(player.GetPlaceMarcation());

        return enemy;
    }
    /// <summary>
    /// Pesquisa e retorna o jogador do time e marcação pesqiusado. Tenha em mente
    /// que esta pesquisa pode retornar nula se o time adversário não tiver nenhum jogador nesta marcação
    /// </summary>
    /// <param name="campteam"></param>
    /// <param name="marcation"></param>
    /// <returns>Jogador na mesma marcação e ou nullo se não houver jogador do time nesta marcação</returns>
    public PlayerController GetPlayerInMarcation(CampTeam campteam, CampPlaceMarcation marcation)
    {

        CampTeam otherCampTeam = campteam == CampTeam.Team_A ? CampTeam.Team_B : CampTeam.Team_A;
        TeamManager team = GetTeamManager(otherCampTeam);
        PlayerController enemy = team.GetPlayerInMarcation(marcation);

        return enemy;
    }

    /// <summary>
    /// Pesquisa de quem é a entrada de jogador que controla determinado time
    /// </summary>
    /// <param name="team">Time a ser pesquisado </param>
    /// <returns>O tipo de controlador de entrada.</returns>
    public ControllerInputType GetControllerType(CampTeam team)
    {
        TeamManager manager = GetTeamManager(team);
        return manager.controllerType;
    }
    public CampPlaceSide GetTeamPlaceSide(CampTeam team)
    {
        TeamManager manager = GetTeamManager(team);
        return manager.side;
    }
    public Transform GetMarcationPosition(PlayerController player, CampPlaceType placeType)
    {
        CampTeam campTeam = player.GetCampTeam();
        CampPlaceSide side = GetTeamPlaceSide(campTeam);
        CampPlaceMarcation marcation = player.GetPlaceMarcation();
        CampPosition campPosition = placesManager.GetPosition(side, marcation, placeType);

        return campPosition.transform;
    }

    public Transform GetMyTeamGoalPosition(PlayerController player)
    {
        TeamManager team = GetTeamManager(player.GetCampTeam());
        return team.goalPosition;
    }
    public Transform GetEnemyGoalPosition(PlayerController player)
    {
        TeamManager otherTeam = GetOtherTeamManager(player.GetCampTeam());
        return otherTeam.goalPosition;
    }

    public void SelectPlayer(PlayerController player)
    {
        if (player.IsSelected())
            return;
        TeamManager team = GetTeamManager(player.GetCampTeam());
        team.MultSelection.SelectPlayer(player);
    }


}
