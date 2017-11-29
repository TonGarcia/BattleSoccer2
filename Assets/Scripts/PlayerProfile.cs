using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoccerGame;

public static class PlayerProfileExtensions
{

    public static SkillVar GetSkill_Stamina(this PlayerController controller)
    {
        return controller.GetComponent<PlayerProfile>().stamina;
    }
}
public class PlayerProfile : MonoBehaviour
{
    [SerializeField]
    private PlayerPerfil perfil;

    [HideInInspector]
    public SkillVar stamina;
    

    private void Awake()
    {
        stamina.regenDuration = 30.0f;
        stamina.subtractDuration = 5.0f;
        stamina.CriticalValue = 0.5f;
        stamina.SetCurrentValue(stamina.MaxValue);
    }
    private void Update()
    {
        stamina.Update();
    }

    private void OnEnable()
    {
        StartCoroutine(SignEvents());
    }
    private void OnDisable()
    {
        UnSignEvents();
    }
    private void OnDestroy()
    {
        UnSignEvents();
    }

    private void UnSignEvents()
    {
        BallController.instance.onSetMyOwner -= OnSetOwnerBall;
    }
    private IEnumerator SignEvents()
    {
        while (BallController.instance == null)
            yield return null;

        BallController.instance.onSetMyOwner += OnSetOwnerBall;
    }

    private void OnSetOwnerBall(PlayerController owner, PlayerController lasOwner)
    {

    }


}
