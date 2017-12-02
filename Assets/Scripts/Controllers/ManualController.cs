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

    //Seleção manual de jogado para passe de bola
    private PlayerController playerToPass = null;

    private float dir { get { return player.dir; } set { player.dir = value; } }
    private float speed { get { return player.speed; } set { player.speed = value; } }
    private float timeToSelect = 0;

    void Awake()
    {
        player = GetComponent<PlayerController>();
    }
    void Update()
    {
        if (player == null)
            return;

        //Seleciona outro jogador manual mais proximo se eu estiver muito longe da bola
        if (player.GetCampTeam().GetSelectionMode() == GameOptionMode.automatric)
        {
            timeToSelect += Time.deltaTime;
            if (timeToSelect > 1.5f)
            {
                if (player.Distance(BallController.GetPosition()) > 3.5f) //Procurando jogador mais proximo
                {
                    PlayerController nearBall = GameManager.instance.GetPlayerNearBall(player.GetCampTeam());
                    if (nearBall != player)
                    {
                        speed = 0;
                        dir = 0;

                        nearBall.SelectME();
                        timeToSelect = 0.0f;
                        return;
                    }
                }
            }
        }

        //Loockat na bola se estiver em strafe
        if (locomotion.inStrafe)
        {
            locomotion.ResetSpeedMultiples();
            Vector3 ballposition = BallController.GetPosition();
            ballposition.y = transform.position.y;
            transform.LookAt(ballposition);
        }

        //Gerenciamento de stamina em movimento acelerado
        /*
        if (locomotion.inSoccer)
        {
            //Se a stamina alcançar o nivel zero, vamos resetar o movimento de soccer e 
            //e ativa a stamina para se auto regenerar. Tenha em mente que o tempo de regeneração é setado
            //no profile do player na skill especifica            
            stamina.mode = SkillVarMode.autoSubtract;

            if (stamina.IsMin)
            {
                stamina.mode = SkillVarMode.autoRegen;
                locomotion.ResetSpeedMultiples();
                player.SetMotionNormal();
            }
        }
        */

        //Para ações manuais se estiver tropeçando
        if (player.Locomotion.inStumble)//Tropeçando
        {
            locomotion.ResetSpeedMultiples();
            speed = 0;
            dir = 0;
            return;
        }

        //Solicita avoid dos aliados a frente
        if (player.IsMyBall())
        {
            PlayerController allyBtw = null;
            if (player.IsHitForwad(5.5f, out allyBtw, player.GetCampTeam()))
            {
                if (allyBtw.IsLookAt(player))
                {
                    Vector3 origim = allyBtw.transform.position + (-allyBtw.transform.forward * 4.5f);
                    Vector3 freePos = locomotion.GetRandomNavCircle(origim, 4.5f);
                    allyBtw.GetComponent<AIController>().GoToPosition(freePos, BallController.instance.transform);
                }
            }
        }

        //Se houver um jogador selecionado para o passe de bola vou esperar ficar distante que o jogador efetue alguma ação difernte de se mover
        //para remover a seleção do jogador a passar a bola
        if (playerToPass != null)
        {
            if (playerToPass.Distance(player) > 11.5f)
            {
                playerToPass = null;
                GameManager.instance.ResetIndicator();
            }
        }

        Vector2 move = locomotion.GetDirectionAxis1();
        dir = move.x;
        speed = move.y;

        //Ações basica de chute
        if (ControllerInput.GetButtonDown(player.GetInputType(), player.GetInputs().Input_Kick))
        {
            playerToPass = null;
            GameManager.instance.ResetIndicator();

            locomotion.TriggerKick();
            GameManager.instance.ResetIndicator();
        }

        //Soccer Motion
        if (ControllerInput.GetButtonDown(player.GetInputType(), player.GetInputs().Input_Stamina))
        {
            //Se houver stamina suficiente ativo movimento soccer e aceleração.
            //Tenha em mente que a stamina suficiente é acima do nivel critico setado pela skill. Se a 
            //stamina nao estiver acima do nivel critico não podera ser utilizada
            if (!stamina.IsCritical)
            {
                playerToPass = null;
                GameManager.instance.ResetIndicator();
                player.SetMotionSoccer();
                player.Locomotion.SetSpeedMultiplies(1.2f);
            }

        }
        if (ControllerInput.GetButtonUp(player.GetInputType(), player.GetInputs().Input_Stamina))
        {
            //Se o jogador estiver indicado para passe de bola, resetemaos a indicação
            playerToPass = null;
            GameManager.instance.ResetIndicator();
            //Retornamos aceleração normal do personagem e setamos animação de movimento normal
            player.Locomotion.ResetSpeedMultiples();
            player.SetMotionNormal();
            //Se a stamina estiver em auto subtract revertemos para auto regen
            // stamina.mode = SkillVarMode.autoRegen;

        }

        //Strafe Motion
        if (ControllerInput.GetButtonDown(player.GetInputType(), player.GetInputs().Input_Strafe))
        {
            //Se o jogador estiver indicado para passe de bola, resetemaos a indicação
            playerToPass = null;
            GameManager.instance.ResetIndicator();
            //Modificamos animação de movimento para strafe e aplicamos aceleração ao movimento
            player.SetMotionStrafe();
            player.Locomotion.SetSpeedMultiplies(1.2f);
        }
        if (ControllerInput.GetButtonUp(player.GetInputType(), player.GetInputs().Input_Strafe))
        {
            //Se o jogador estiver indicado para passe de bola, resetemaos a indicação
            playerToPass = null;
            GameManager.instance.ResetIndicator();
            //Retornamos aceleração normal do personagem e setamos animação de movimento normal
            player.Locomotion.ResetSpeedMultiples();
            player.SetMotionNormal();

        }

        //Seleção para passe de bola
        if (ControllerInput.GetButton(player.GetInputType(), player.GetInputs().Input_Pass))
        {
            if (player.IsMyBall() == true && locomotion.inNormal)
            {

                Vector3 mdirection = transform.forward;
                List<PlayerController> players = player.GetPlayersNear(10.5f);

                if (players.Count > 0)
                {
                    PlayerController target = players.MinAngle(player, mdirection);
                    playerToPass = target;
                    GameManager.instance.IndicatePlayer(playerToPass);
                }
            }
        }
        //Passe de bola
        if (ControllerInput.GetButtonUp(player.GetInputType(), player.GetInputs().Input_Pass))
        {
            //playerToPass = null;
            //GameManager.instance.ResetIndicator();

            if (player.IsMyBall() == true && locomotion.inNormal)
            {
                locomotion.TriggerPass();
            }
        }


    }

    //Unity Events
    private void OnEnable()
    {
        if (player == null)
            player = GetComponent<PlayerController>();

        player.SetMotionNormal();
        SignEvents();
    }
    private void OnDisable()
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Animator>().rootPosition = transform.position;

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
    private void EvChangeDirectionStart()
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
    private void EvChangeDirectionOk()
    {

        BallController.instance.SetBallDesprotectTo(player);

    }
    private void EvChangeDirectionFinish()
    {
        BallController.instance.SetBallDesprotectTo(player);
        player.UnsetKinematic();
    }

    private void EvTurnDirectionStart()
    {
        player.SetKinematic();

        if (!player.IsMyBall())
            return;

        BallController.instance.SetBallProtectedTo(player);
    }
    private void EvTurnDirectionFinish()
    {
        BallController.instance.SetBallDesprotectTo(player);
        player.UnsetKinematic();
    }

    private void EvPassStart()
    {
        if (player.IsMyBall())
            BallController.instance.SetBallProtectedTo(player);
        player.SetKinematic();
    }
    private void EvPassOk()
    {
        BallController.instance.SetBallDesprotectTo(player);

        if (player.IsMyBall())
        {
            if (playerToPass != null)
            {
                Vector3 dir = playerToPass.transform.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = rotation;

                playerToPass.GetAiControlelr().WaitPass();
                BallController.SetPass(playerToPass.Distance(player) * 2.0f);
            }
            else
            {

                BallController.SetPass(20.0f);
            }

            playerToPass = null;
            GameManager.instance.ResetIndicator();
        }
    }
    private void EvPassFinish()
    {
        player.UnsetKinematic();
    }


    private void EvLongKickOk()
    {
        if (BallController.IsOwner(player))
            BallController.SetKick();
    }
    private void EvEntryStart()
    {
        if (BallController.IsOwner(player))
            BallController.instance.SetBallProtectedTo(player);

        player.SetKinematic();

    }
    private void EvEntryFinish()
    {

        BallController.instance.SetBallDesprotectTo(player);
        player.UnsetKinematic();

    }
    private void EvStumbleStart()
    {
        player.SetKinematic();
    }
    private void EvStumbleFinish()
    {
        player.UnsetKinematic();
    }

    //Private methods
    private SkillVar stamina;

    private void SignEvents()
    {
        StartCoroutine(IESignevents());
    }
    private void UnsignEvents()
    {
        PlayerAnimatorEvents animatorEvents = GetComponent<PlayerAnimatorEvents>();
        animatorEvents.OnChangeDirStart -= EvChangeDirectionStart;
        animatorEvents.OnChangeDirOk -= EvChangeDirectionOk;
        animatorEvents.OnChangeDirFinish -= EvChangeDirectionFinish;
        animatorEvents.OnTurnStart -= EvTurnDirectionStart;
        animatorEvents.OnTurnFinish -= EvTurnDirectionFinish;
        animatorEvents.OnKickOk -= EvLongKickOk;
        animatorEvents.OnPassStart -= EvPassStart;
        animatorEvents.OnPassOk -= EvPassOk;
        animatorEvents.OnPassFinish -= EvPassFinish;
        animatorEvents.OnEnttryStart -= EvEntryStart;
        animatorEvents.OnEnttryFinish -= EvEntryFinish;
        animatorEvents.OnStumblesStart -= EvStumbleStart;
        animatorEvents.OnStumblesFinish -= EvStumbleFinish;

        BallController.instance.onSetMyOwner -= OnBallSetOwner;
        BallController.instance.onRemoveMyOwner -= OnBallRemoveOwner;

    }

    private IEnumerator IESignevents()
    {


        PlayerAnimatorEvents animatorEvents = GetComponent<PlayerAnimatorEvents>();
        animatorEvents.OnChangeDirStart += EvChangeDirectionStart;
        animatorEvents.OnChangeDirOk += EvChangeDirectionOk;
        animatorEvents.OnChangeDirFinish += EvChangeDirectionFinish;
        animatorEvents.OnTurnStart += EvTurnDirectionStart;
        animatorEvents.OnTurnFinish += EvTurnDirectionFinish;
        animatorEvents.OnKickOk += EvLongKickOk;
        animatorEvents.OnPassStart += EvPassStart;
        animatorEvents.OnPassOk += EvPassOk;
        animatorEvents.OnPassFinish += EvPassFinish;
        animatorEvents.OnEnttryStart += EvEntryStart;
        animatorEvents.OnEnttryFinish += EvEntryFinish;
        animatorEvents.OnStumblesStart += EvStumbleStart;
        animatorEvents.OnStumblesFinish += EvStumbleFinish;


        while (BallController.instance == null)
            yield return null;

        BallController.instance.onSetMyOwner += OnBallSetOwner;
        BallController.instance.onRemoveMyOwner += OnBallRemoveOwner;
    }


}
