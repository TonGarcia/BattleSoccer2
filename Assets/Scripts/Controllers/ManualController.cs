using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;
using System;

public class ManualController : MonoBehaviour
{
    [Tooltip("Se o jogador tomar a bola e estiver a uma distancia igual ou menor, a animação de tomada de bola sera executada")]
    public float distanceToEntry = 2.5f;


    private PlayerController player;
    private ControllerLocomotion locomotion { get { return player.Locomotion; } }
    private PlayerInput playerInput;

    private float dir { get { return player.dir; } set { player.dir = value; } }
    private float speed { get { return player.speed; } set { player.speed = value; } }

    void Start()
    {
        player = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();

    }
    void Update()
    {
        if (player == null)
            return;

        if (player.Locomotion.inStumble)//Tropeçando
        {
            speed = 0;
            dir = 0;

            return;
        }

        //Move jogador para cordenadas do joystick
        Vector2 move = locomotion.GetDirectionAxis();
        dir = move.x;
        speed = move.y;

        //Ações de chute
        if (ControllerInput.GetButtonDown(playerInput.InputType, playerInput.Input_Kick))
        {
            locomotion.TriggerKick();
        }

        //Soccer Motion
        if (ControllerInput.GetButtonDown(playerInput.InputType, playerInput.Input_Stamina))
        {

            player.SetMotionSoccer();
            player.Locomotion.SetSpeedMultiplies(1.2f);
        }
        if (ControllerInput.GetButtonUp(playerInput.InputType, playerInput.Input_Stamina))
        {
            player.Locomotion.ResetSpeedMultiples();
            player.SetMotionNormal();

        }

        //Strafe Motion
        if (ControllerInput.GetButtonDown(playerInput.InputType, playerInput.Input_Strafe))
        {

            player.SetMotionStrafe();
            player.Locomotion.SetSpeedMultiplies(1.2f);
        }
        if (ControllerInput.GetButtonUp(playerInput.InputType, playerInput.Input_Strafe))
        {
            player.Locomotion.ResetSpeedMultiples();
            player.SetMotionNormal();

        }
    }

    //Unity Events
    private void OnEnable()
    {
        SignEvents();
    }
    private void OnDisable()
    {
        UnsignEvents();
    }
    private void OnCollisionEnter(Collision collision)
    {
        PlayerController colPlayer = collision.gameObject.GetComponent<PlayerController>();
        if (colPlayer)
        {
            locomotion.TriggerEntry();
        }

    }
    //BallEvents
    private void OnBallSetOwner(PlayerController owner, PlayerController lasOwner)
    {
        //Animação de entrada de bola
        if (owner == player && lasOwner != null)
        {
            float distance = lasOwner.Distance(player);
            if (distance <= distanceToEntry)
            {
                locomotion.TriggerEntry();
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

        player.SetKinematic();

        if (!player.IsMyBall())
            return;

        BallController.instance.SetBallProtectedTo(player);
        BallController.ChangeDirection();

        //Se o jogador selecionado do time adversario estiver proximo a mim na hora do lésinho, vou fazer ele tropeçar
        CampTeam adversary = player.GetCampTeam() == CampTeam.Team_A ? CampTeam.Team_B : CampTeam.Team_A;

        List<PlayerController> enemys = GameManager.instance.GetPlayersNearBall(adversary, 2.5f);
        if (enemys.Count > 0)
            foreach (PlayerController enemy in enemys)
                enemy.Locomotion.TriggerStumb();

    }
    private void OnChangeDirectionOk()
    {

        BallController.instance.SetBallDesprotectTo(player);

    }
    private void OnChangeDirectionFinish()
    {
        BallController.instance.SetBallDesprotectTo(player);
        player.UnsetKinematic();
    }

    private void OnTurnDirectionStart()
    {
        player.SetKinematic();

        if (!player.IsMyBall())
            return;

        BallController.instance.SetBallProtectedTo(player);
    }
    private void OnTurnDirectionOk()
    {
        // BallController.instance.SetBallDesprotectTo(player);
    }
    private void OnTurnDirectionFinish()
    {
        BallController.instance.SetBallDesprotectTo(player);
        player.UnsetKinematic();
    }

    private void OnLongKickOk()
    {
        if (BallController.IsOwner(player))
            BallController.SetKick();
    }
    private void OnEntryStart()
    {
        if (BallController.IsOwner(player))
            BallController.instance.SetBallProtectedTo(player);

        player.SetKinematic();

    }
    private void OnEntryFinish()
    {

        BallController.instance.SetBallDesprotectTo(player);
        player.UnsetKinematic();

    }
    private void OnStumbleStart()
    {
        player.SetKinematic();
    }
    private void OnStumbleFinish()
    {
        player.UnsetKinematic();
    }

    //Private methods
    private void SignEvents()
    {
        StartCoroutine(IESignevents());
    }
    private void UnsignEvents()
    {

        BallController.instance.onSetMyOwner -= OnBallSetOwner;
        BallController.instance.onRemoveMyOwner -= OnBallRemoveOwner;

    }

    private IEnumerator IESignevents()
    {
        while (BallController.instance == null)
            yield return null;

        BallController.instance.onSetMyOwner += OnBallSetOwner;
        BallController.instance.onRemoveMyOwner += OnBallRemoveOwner;
    }

}
