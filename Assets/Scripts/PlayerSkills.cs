using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoccerGame;

public static class PlayerSkillsExtensions
{

    public static SkillVar GetSkill_Stamina(this PlayerController controller)
    {
        return controller.GetComponent<PlayerSkills>().stamina;
    }
}
public class PlayerSkills : MonoBehaviour
{
    [SerializeField]
    private PlayerProfile perfil;

    [HideInInspector]
    public SkillVar stamina;
    

    private void Awake()
    {
        perfil.stamina.Initialzie(out stamina);        
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
