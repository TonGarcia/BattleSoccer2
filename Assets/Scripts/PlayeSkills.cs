using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoccerGame;

public static class PlayerSkillseExtensions
{
    public static SkillVar GetSkill(this PlayerController controller, int index)
    {
        PlayeSkills playerSkills = controller.GetComponent<PlayeSkills>();
        SkillVar result = null;

        if (playerSkills.Skills.Count > index && index >= 0)
        {
            result = playerSkills.Skills[index];
        }

        return result;
    }
    public static Ability GetAbility(this PlayerController controller, int index)
    {
        PlayeSkills playerSkills = controller.GetComponent<PlayeSkills>();
        Ability result = null;

        if (playerSkills.Profile.Abilities.Length > index && index >= 0)
        {
            result = playerSkills.Profile.Abilities[index];
        }

        return result;
    }

}
public class PlayeSkills : MonoBehaviour
{

    [SerializeField]
    private PlayerProfile profile;
    public PlayerProfile Profile { get { return profile; } }

    private List<SkillVar> skills;
    public List<SkillVar> Skills { get { return new List<SkillVar>(skills); } }

    PlayerController player;

    private void Start()
    {
        skills = new List<SkillVar>();
        player = GetComponent<PlayerController>();
        //Criando clone das habilidades do database global de abilidades
        //para poderem ser manipuladas individualmente por cada personagem
        foreach (Ability ability in profile.Abilities)
        {
            ability.Initialize(player);
            SkillVar skill = ability.Skill.Clone();
            skills.Add(skill);
        }
    }

    private void Update()
    {
        foreach (SkillVar skill in skills)
            skill.Update();


    }

    //Eventos da Unity
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
    //Eventos de bola
    private void OnSetOwnerBall(PlayerController owner, PlayerController lasOwner)
    {

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



}
