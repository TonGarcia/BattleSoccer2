using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;
using System;


public class MultiSelection : MonoBehaviour
{
    public GameManager gameMananger;

    [SerializeField]
    private CampTeam team;
    
    [SerializeField]
    private GameObject selectorPrefab;

    private BallController ball = null;
    private PlayerController selectedPlayer = null;
    private GameObject selector = null;

    private void Start()
    {
        ball = BallController.instance;
        if (selectorPrefab != null)
            selector = Instantiate(selectorPrefab);
    }
    private void LateUpdate()
    {

        //Seleção manual de jogador mais proximo
        if (selectedPlayer.IsMyBall() == false && team.GetSelectionMode() == GameOptionMode.manual)
        {
            if (ControllerInput.GetButtonDown(team.GetInputType(), team.GetInputs().Input_Selection))
            {
                PlayerController nearPlayer = null;
                if (selectedPlayer != null)
                    nearPlayer = selectedPlayer.GetTeamPlayerNear();
                else
                    nearPlayer = gameMananger.GetPlayerNearBall(team);

                if (nearPlayer != null && nearPlayer != selectedPlayer)
                {
                    SelectPlayer(nearPlayer);

                }
            }
        }


        SelectorUpdate();
    }
    //Start MANUAL do componente. 
    public void SetTeam(CampTeam team)
    {
        this.team = team;
        //Seleção primaria, selecionamos o jogador mais proximo da bola
        if (selectedPlayer == null)
        {
            PlayerController nearPlayer = gameMananger.GetPlayerNearBall(team);
            if (nearPlayer != null)
            {
                SelectPlayer(nearPlayer);
            }
        }

        ball.onSetMyOwner += OnBallSetOwner;
    }
    public PlayerController GetSelectedPlayer()
    {
        return selectedPlayer;
    }

    //UnityEvents
    private void OnDestroy()
    {
        ball.onSetMyOwner -= OnBallSetOwner;
    }

    //Ballcontroller Events
    private void OnBallSetOwner(PlayerController owner, PlayerController lasOwner)
    {
        //Seleção automatica no jogador com a bola
        if (owner.GetCampTeam() == team)
        {
            if (owner != selectedPlayer)
            {
                SelectPlayer(owner);
            }
        }
    }

    public void SelectPlayer(PlayerController player)
    {
        if (selectedPlayer == null)
        {
            selectedPlayer = player;
            selectedPlayer.SetManual();
        }
        else
        {
            selectedPlayer.SetAutomatic();
            selectedPlayer = player;
            selectedPlayer.SetManual();

        }
    }
    private void SelectorUpdate()
    {
        if (selector == null)
            return;

        if (selectedPlayer != null)
            selector.transform.position = selectedPlayer.transform.position;
        else
            selector.transform.position = new Vector3(100, 100, 100);
    }


}

