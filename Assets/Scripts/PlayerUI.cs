using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoccerGame;
using System;

public class PlayerUI : MonoBehaviour
{
    public PlayerController player;
    public GameObject canvas;
    public SkillVar injuried;

    private void Start()
    {
        injuried.panel.gameObject.SetActive(false);
        canvas.gameObject.SetActive(false);
        GetComponent<PlayerAnimatorEvents>().OnTripingStart += OnTripingStar;
        GetComponent<PlayerAnimatorEvents>().OnOnStandingupFinish += OnTripingFinish;

        injuried.OnCooldown += InjuriedOnCooldown;
    }

    //Ao finalizar o cooldown injuried Ativamos locomotion de se levantar do jogador
    private void InjuriedOnCooldown()
    {
        player.Locomotion.ResetTrip();
    }
    //Ao finalizar animação de levantar-se do jogador, desativamos canvas e icone de injuried
    private void OnTripingFinish()
    {
        injuried.panel.gameObject.SetActive(false);
        canvas.gameObject.SetActive(false);
    }
    //Ativamos a canvas e o icone de injuried para que seja apresentado visualmente o cooldown
    private void OnTripingStar()
    {
       
        canvas.gameObject.SetActive(true);
        injuried.panel.gameObject.SetActive(true);
        injuried.TriggerCooldown();
    }

    public void Update()
    {
        injuried.Update();

    }


}
