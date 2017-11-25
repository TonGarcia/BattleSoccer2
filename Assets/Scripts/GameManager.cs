using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SoccerGame;

public enum GameOptionMode
{
    automatric,
    manual
}

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
    public static PlayerController GetEnemyNear(this PlayerController controller)
    {
        return GameManager.instance.GetEnemyPlayerNear(controller);

    }
    public static PlayerController GetPlayerNear(this PlayerController controller, CampTeam team)
    {
        return GameManager.instance.GetPlayerNear(controller, team);

    }

    public static void SelectME(this PlayerController controller)
    {
        GameManager.instance.SelectPlayer(controller);
    }
    public static PlayerController GetTeamPlayerNear(this PlayerController controller)
    {
        return GameManager.instance.GetTeamPlayerNear(controller);
    }
    public static GameOptionMode GetSelectionMode(this CampTeam team)
    {
        return GameManager.instance.GetSelectionMode(team);
    }
    public static ControllerInputType GetInputType(this PlayerController controller)
    {
        return GameManager.instance.GetControllerType(controller.GetCampTeam());
    }
    public static ControllerInputType GetInputType(this CampTeam team)
    {
        return GameManager.instance.GetControllerType(team);
    }
    public static bool IsIA(this PlayerController controller)
    {
        return GameManager.instance.IsIA(controller.GetCampTeam());
    }
    public static bool IsController1(this PlayerController controller)
    {
        return GameManager.instance.IsController1(controller.GetCampTeam());
    }
    public static bool IsController2(this PlayerController controller)
    {
        return GameManager.instance.IsController2(controller.GetCampTeam());
    }
    public static bool IsIA(this CampTeam team)
    {
        return GameManager.instance.IsIA(team);
    }
    public static bool IsController1(this CampTeam team)
    {
        return GameManager.instance.IsController1(team);

    }
    public static bool IsController2(this CampTeam team)
    {
        return GameManager.instance.IsController2(team);

    }
    public static PlayerInput GetInputs(this PlayerController controller)
    {
        return GameManager.instance.GetPlayerinput(controller.GetCampTeam());
    }
    public static PlayerInput GetInputs(this CampTeam team)
    {
        return GameManager.instance.GetPlayerinput(team);

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
        public GameOptionMode selectionMode;
        public Transform goalPosition;

        public PlayerInput playerInput;

        [SerializeField]
        private List<PlayerController> players;
        public List<PlayerController> Players { get { return new List<PlayerController>(players); } }

        public bool autoFoundPlayers = false;

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
    [SerializeField]
    private PlayerIndicator indicator;
    public Transform midCampTransform;


    private void Awake()
    {
        instance = this;
        ResetIndicator();
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
    private TeamManager GetEnemyTeamManager(CampTeam team)
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
    public PlayerController GetTeamPlayerNear(PlayerController controller)
    {
        TeamManager manager = GetTeamManager(controller.GetCampTeam());
        List<PlayerController> players = manager.Players;
        players.Remove(controller);

        float min = players.Min(r => r.Distance(controller));
        PlayerController result = players.FirstOrDefault(r => r.Distance(controller) == min);

        return result;

    }
    public List<PlayerController> GetPlayersNearBall(CampTeam team, float near)
    {
        TeamManager manager = GetTeamManager(team);
        List<PlayerController> players = manager.Players;

        players.RemoveAll(r => r.transform.Distance(BallController.instance.transform) > near);

        return players;
    }
    public PlayerController GetPlayerNearBall(CampTeam team, CampActionAttribute campAcation)
    {
        TeamManager manager = GetTeamManager(team);

        List<PlayerController> players = manager.Players;
        players.RemoveAll(r => r.GetCampAction() != campAcation);

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
    /// <summary>
    /// Pesquisa o jogaodr do mesmo time e ação de campo especificada, que esteja mais proximo do jogador origem e que não tenha nenhum inimigo entre eles.
    /// caso não exista nenhum jogador nestes critérios, nullo sera retornado
    /// </summary>
    /// <param name="player">Jogador origem</param>
    /// <param name="campAcation">Tipo de ação de campo dos jogadores a serem pesquisados</param>
    /// <returns>Jogador que tenha caminho livre entre o jogaodor origem e ele, ou nulo se os criterios nao forem sucetiveis</returns>
    public PlayerController GetNearPlayerRaycast(PlayerController player, CampActionAttribute campAcation)
    {
        PlayerController result = null;
        TeamManager manager = GetTeamManager(player.GetCampTeam());

        List<PlayerController> players = manager.Players;
        players.Remove(player);
        players.RemoveAll(r => r.GetCampAction() != campAcation);

        foreach (PlayerController p in new List<PlayerController>(players))
        {
            PlayerController hitedPlayer;
            if (p.IsHitBetween(player, out hitedPlayer))
            {
                if (hitedPlayer.IsMyTeaM(player) == false)
                    players.Remove(p);
            }
        }

        if (players.Count > 0)
        {
            float min = players.Min(r => r.Distance(player));
            result = players.FirstOrDefault(r => r.Distance(player) == min);
        }

        return result;

    }
    /// <summary>
    /// Pesquisa o jogaodr do mesmo time, que esteja mais proximo do jogador origem e que não tenha nenhum inimigo entre eles.
    /// caso não exista nenhum jogador nestes critérios, nullo sera retornado
    /// </summary>
    /// <param name="player">Jogador origem</param>
    /// <param name="campAcation">Tipo de ação de campo dos jogadores a serem pesquisados</param>
    /// <returns>Jogador que tenha caminho livre entre o jogaodor origem e ele, ou nulo se os criterios nao forem sucetiveis</returns>
    public PlayerController GetNearPlayerRaycast(PlayerController player)
    {
        PlayerController result = null;
        TeamManager manager = GetTeamManager(player.GetCampTeam());

        List<PlayerController> players = manager.Players;
        players.Remove(player);
        //players.RemoveAll(r => r.GetCampAction() != campAcation);

        foreach (PlayerController p in new List<PlayerController>(players))
        {
            PlayerController hitedPlayer;
            if (p.IsHitBetween(player, out hitedPlayer))
            {
                if (hitedPlayer.IsMyTeaM(player) == false)
                    players.Remove(p);
            }
        }

        if (players.Count > 0)
        {
            float min = players.Min(r => r.Distance(player));
            result = players.FirstOrDefault(r => r.Distance(player) == min);
        }

        return result;

    }
    public GameOptionMode GetSelectionMode(CampTeam team)
    {
        TeamManager manager = GetTeamManager(team);
        return manager.selectionMode;
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
    /// Pesquisa o jogador do time adversário mais proximo do jogador origem
    /// </summary>
    /// <param name="controller">jogador origem</param>
    /// <returns>jogador proximo. nulo somente se não exisitr time adversário em jogo</returns>
    public PlayerController GetEnemyPlayerNear(PlayerController controller)
    {
        PlayerController result = null;
        TeamManager manager = GetEnemyTeamManager(controller.GetCampTeam());
        List<PlayerController> players = manager.Players;

        if (players.Count > 0)
        {
            float min = players.Min(r => r.Distance(controller));
            result = players.FirstOrDefault(r => r.Distance(controller) == min);
        }
        return result;
    }
    public PlayerController GetPlayerNear(PlayerController controller, CampTeam team)
    {
        TeamManager manager = GetTeamManager(team);
        List<PlayerController> players = manager.Players;
        if (team == controller.GetCampTeam())
            players.Remove(controller);

        float min = players.Min(r => r.Distance(controller));
        PlayerController result = players.FirstOrDefault(r => r.Distance(controller) == min);

        return result;
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
        TeamManager otherTeam = GetEnemyTeamManager(player.GetCampTeam());
        return otherTeam.goalPosition;
    }

    public bool IsIA(CampTeam team)
    {
        TeamManager manager = GetTeamManager(team);
        return team.GetInputType() == ControllerInputType.ControllerCPU;
    }
    public bool IsController1(CampTeam team)
    {
        TeamManager manager = GetTeamManager(team);
        return team.GetInputType() == ControllerInputType.Controller1;
    }
    public bool IsController2(CampTeam team)
    {
        TeamManager manager = GetTeamManager(team);
        return team.GetInputType() == ControllerInputType.Controller2;
    }
    public PlayerInput GetPlayerinput(CampTeam team)
    {
        TeamManager manager = GetTeamManager(team);

        return manager.playerInput;
    }

    public void SelectPlayer(PlayerController player)
    {
        if (player.IsSelected())
            return;
        TeamManager team = GetTeamManager(player.GetCampTeam());
        team.MultSelection.SelectPlayer(player);
    }
    public void IndicatePlayer(PlayerController player)
    {
        indicator.gameObject.SetActive(true);
        indicator.SelectPlayer(player);
    }
    public void ResetIndicator()
    {
        indicator.Unselect();
        indicator.gameObject.SetActive(false);
    }

}
