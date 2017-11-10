using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;
using System;

public class ManualController : MonoBehaviour
{
    [Tooltip("Se o jogador tomar a bola e estiver a uma distancia igual ou menor, a animação de tomada de bola sera executada")]
    public float distanceToEntry = 2.5f;
    public Collider FovBallTryger;

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

        //Move jogador para cordenadas do joystick
        Vector2 move = locomotion.GetDirectionAxis();
        dir = move.x;
        speed = move.y;

        //Ações de chute
        if (ControllerInput.GetButtonDown(playerInput.InputType, playerInput.Input_Kick))
        {
            locomotion.TrygerKick();
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

    //BallEvents
    private void OnBallSetOwner(PlayerController owner, PlayerController lasOwner)
    {
        //Animação de entrada de bola
        if (owner == player && lasOwner != null)
        {
            float distance = lasOwner.Distance(player);
            if (distance <= distanceToEntry)
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
