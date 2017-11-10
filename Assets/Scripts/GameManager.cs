using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using SoccerGame;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class TeamManager
    {
        public CampTeam team;
        public CampPlaceSide side;
        public ControllerInputType controllerType;

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
            _players.RemoveAll(r => r.GetPlayerTeam() != team);
            SetPlayers(_players);

            multSelection.SetTeam(team);
        }
        public bool autoFoundPlayers = false;
        public void SetPlayers(List<PlayerController> players)
        {
            this.players.AddRange(players);
        }
    }

    public static GameManager instance;

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

}
