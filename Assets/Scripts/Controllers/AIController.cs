using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;
using UnityEngine.AI;
using System;

public class AIController : MonoBehaviour
{

    private PlayerController player;
    private ControllerLocomotion locomotion { get { return player.Locomotion; } }
    private PlayerInput playerInput;
    private NavMeshAgent agent;
    private SoccerAIUnSelected AIUnselected;
    private SoccerAISelected AISelected;
    private SoccerAIwithBall AIWithBall;

    private float nvSpeed, nvAngularSpeed, nvAceleration;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        AIUnselected = new SoccerAIUnSelected(gameObject);
        AISelected = new SoccerAISelected(gameObject);
        AIWithBall = new SoccerAIwithBall(gameObject);

        nvSpeed = agent.speed;
        nvAngularSpeed = agent.angularSpeed;
        nvAceleration = agent.acceleration;
    }

    void Update()
    {
        if (!agent || !playerInput || !player)
            return;

        UpdateNavMeshAgent();

        if (player.IsMyBall())           //Estados de Jogador com a bola
        {
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
