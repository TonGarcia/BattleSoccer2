using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;
using System;

public class Manualcontroller : MonoBehaviour
{
    public Collider FovBallTryger;
    private PlayerController player;
    private ControllerLocomotion locomotion { get { return player.Locomotion; } }
    private PlayerInput playerInput;
    private PlayerAction action;

    private float dir { get { return player.dir; } set { player.dir = value; } }
    private float speed { get { return player.speed; } set { player.speed = value; } }

    float oldSpeed = 0;

    void Start()
    {
        player = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
        BallController.instance.onSetMyOwner += OnBallSetOwner;
        BallController.instance.onRemoveMyOwner += OnBallRemoveOwner;
    }



    void Update()
    {
        if (player == null)
            return;

        //Move jogador para cordenadas do joystick
        Vector2 move = locomotion.GetDirectionAxis();

        float atualdir = move.x;

        oldSpeed = speed;

        float atualspeed = move.y;


        dir = atualdir;
        speed = atualspeed;

        //Ações de chute
        if (ControllerInput.GetButtonDown(playerInput.InputType, playerInput.Input_Kick))
        {
            locomotion.TrygerKick();
        }

    }

    //Unity Events
    private void OnEnable()
    {
        action = PlayerAction.Controlling;
    }

    //BallEvents
    private void OnBallSetOwner(PlayerController owner, PlayerController lasOwner)
    {
        //Animação de entrada de bola
        if (owner == player && lasOwner != null)
        {
            float distance = lasOwner.Distance(player);
            if (distance <= 1.5f)
            {               
                locomotion.TrygerEntry();
            }
        }
    }
    private void OnBallRemoveOwner(PlayerController lasOwner)
    {
      
    }


    //Animations Event Tryger
    //Estes eventos são chamados apartir das animações rerentes em quadros espesificos
    private void OnChangeDirectionStart()
    {

        SetKinematic();
        BallController.ChangeDirection();


    }
    private void OnChangeDirectionOk()
    {
        UnsetKinematic();
    }
    private void OnChangeDirectionFinish()
    {

    }
    private void OnLongKickOk()
    {
        if (BallController.IsOwner(player))
            BallController.SetKick();
    }
    private void OnEntryStart()
    {
        SetKinematic();
        if (BallController.IsOwner(player))
            BallController.instance.SetBallProtected();
    }
    private void OnEntryFinish()
    {
        UnsetKinematic();
        if (BallController.IsOwner(player))
            BallController.instance.SetDesprotectBall();
    }
    //Private methods

    private bool isInverse(float a, float b)
    {
        if (a > 0 && b < 0)
            return true;
        else if (a < 0 && b > 0)
            return true;
        else return false;
    }
    private void SetKinematic()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Collider>().enabled = false;
        FovBallTryger.enabled = false;
    }
    private void UnsetKinematic()
    {
        gameObject.GetComponent<Collider>().enabled = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        FovBallTryger.enabled = true;
    }
}
