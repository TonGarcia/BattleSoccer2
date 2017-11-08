using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;
using System;

public class MultiSelection : MonoBehaviour
{
    public GameManager gameMananger;

    [SerializeField]
    private ButtomInputType selectInputButtom;

    [SerializeField]
    private GameObject selectorPrefab;

    private CampTeam team;
    private BallController ball = null;
    private PlayerController selectedPlayer = null;
    private GameObject selector = null;

    private void Start()
    {

        ball = BallController.instance;
        if (selectorPrefab != null)
            selector = Instantiate(selectorPrefab);

        ball.onSetMyOwner += OnBallSetOwner;
       
    }



    void LateUpdate()
    {
        //Seleção manual de jogador mais proximo da bola
        if (selectedPlayer.IsMyBall() == false)
        {
            if (ControllerInput.GetButtonDown(gameMananger.GetControllerType(team), selectInputButtom))
            {
                PlayerController nearPlayer = gameMananger.GetPlayerNearBall(team);
                if (nearPlayer != null)
                {
                    SelectPlayer(nearPlayer);

                }
            }
        }


        SelectorUpdate();
    }

    //UnityEvents
    private void OnDestroy()
    {
        ball.onSetMyOwner -= OnBallSetOwner;
      
    }

    //BallController Events
    private void OnBallSetOwner(PlayerController owner, PlayerController lasOwner)
    {
        if(owner.PlayerTeam() == team)
        {
            if(owner!=selectedPlayer)
            {
                SelectPlayer(owner);
            }
        }
    }

   
    //Methods private
    private void SelectPlayer(PlayerController player)
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
   
    //Methods public
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
    }
    public PlayerController GetSelectedPlayer()
    {
        return selectedPlayer;
    }


}

