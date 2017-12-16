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
    public static SkillVar GetSkill_BasicKick(this PlayerController controller)
    {
        return controller.GetComponent<PlayerSkills>().basicKick;
    }
    public static SkillVar GetSkill_BasicPass(this PlayerController controller)
    {
        return controller.GetComponent<PlayerSkills>().basicPass;
    }
    public static SkillVar GetSkill_BasicActionOne(this PlayerController controller)
    {
        return controller.GetComponent<PlayerSkills>().basicActionOne;
    }
    public static SkillVar GetSkill_BasicActionTwo(this PlayerController controller)
    {
        return controller.GetComponent<PlayerSkills>().basicActionTwo;
    }
}
public class PlayerSkills : MonoBehaviour
{
    [SerializeField]
    private PlayerProfile perfil;

    [HideInInspector]
    public SkillVar stamina;
    [HideInInspector]
    public SkillVar basicKick;
    [HideInInspector]
    public SkillVar basicPass;
    [HideInInspector]
    public SkillVar basicActionOne;
    [HideInInspector]
    public SkillVar basicActionTwo;

    private void Awake()
    {
        perfil.stamina.Initialzie(out stamina);
        perfil.basicKick.Initialzie(out basicKick);
        perfil.basicPass.Initialzie(out basicPass);
        perfil.basicActionOne.Initialzie(out basicActionOne);
        perfil.basicActionTwo.Initialzie(out basicActionTwo);

    }
    private void Update()
    {
        stamina.Update();
        basicKick.Update();
        basicPass.Update();
        basicActionOne.Update();
        basicActionTwo.Update();
    }   

}
