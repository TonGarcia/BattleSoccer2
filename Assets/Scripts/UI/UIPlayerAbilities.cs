using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoccerGame;


public class UIPlayerAbilities : MonoBehaviour
{
    [System.Serializable]
    public class AbilitiesTeamHud
    {

        public ControllerInputType inputType
        {
            get { return team.GetInputType(); }
        }
        public CampTeam team;
        public RectTransform container;
        public UISkillAbility UIStamina;
        public UISkillAbility UIAttack;
        public UISkillAbility UIDeffense;     

        public void ActiveHud()
        {
            container.gameObject.SetActive(true);
        }
        public void DesactiveHud()
        {
            container.gameObject.SetActive(false);
        }
        public void Update(bool showIA)
        {
            //Esconde HUD se FOR AI e não estiver habilitado
            if (showIA == false && inputType == ControllerInputType.ControllerCPU)
            {                
                container.gameObject.SetActive(false);
                return;
            }
            
            PlayerController player = team.GetSelectedPlayer();
            if (player == null)
            {
                container.gameObject.SetActive(false);
                return;
            }

            //Ativamos HUD
            container.gameObject.SetActive(true);

            HandleStamina(player);
            HandleActionOne(player);
            HandleActionTwo(player);

        }
        private void HandleStamina(PlayerController player)
        {
            SkillVar stamina = player.GetSkill_Stamina();

            //Modifica as sprites para mostrar as imagens corretas de ações com Bola e ações sem bola
            if (team.IsMyBall())//Sprites Ações com bola
            {
                UIStamina.SetIcon(stamina.ability.icon_withBall);
            }
            else //Sprites Ações sem bola
            {
                UIStamina.SetIcon(stamina.ability.icon_withOutBall);
            }

            UIStamina.SetValue(stamina.FillAmounValue);
            if (stamina.IsCritical)
                UIStamina.SetValueColor(Color.red);
            else
                UIStamina.ResetValueColor();
        }
        private void HandleActionOne(PlayerController player)
        {
            SkillVar skill_kick = player.GetSkill_BasicKick();
            SkillVar skill_ActionOne = player.GetSkill_BasicActionOne();
            SkillVar skill_selected = null;
          
            //Select usage skill
            if (team.IsMyBall())          
                skill_selected = skill_kick;            
            else            
                skill_selected = skill_ActionOne;            

            UIAttack.SetIcon(skill_selected.ability.icon_withBall);
            UIAttack.SetValue(skill_selected.FillAmounValue);
            UIAttack.SetCooldown(skill_selected.FillAmountCooldown);

            if (skill_selected.IsCritical)
                UIAttack.SetValueColor(Color.red);
            else
                UIAttack.ResetValueColor();
        }
        private void HandleActionTwo(PlayerController player)
        {
            SkillVar skill_pass = player.GetSkill_BasicPass();
            SkillVar skill_ActionTwo = player.GetSkill_BasicActionTwo();
            SkillVar skill_selected = null;

            //Select usage skill
            if (team.IsMyBall())
                skill_selected = skill_pass;
            else
                skill_selected = skill_ActionTwo;

            UIDeffense.SetIcon(skill_selected.ability.icon_withBall);
            UIDeffense.SetValue(skill_selected.FillAmounValue);
            UIDeffense.SetCooldown(skill_selected.FillAmountCooldown);

            if (skill_selected.IsCritical)
                UIDeffense.SetValueColor(Color.red);
            else
                UIDeffense.ResetValueColor();
        }
    }


    public AbilitiesTeamHud abilitiesTeamA;
    public AbilitiesTeamHud abilitiesTeamB;

    public bool showAIHud = false;


    public void Update()
    {
        if (!GameManager.isReady)
            return;

        abilitiesTeamA.Update(showAIHud);
        abilitiesTeamB.Update(showAIHud);

    }

    //Unity Events
    private void OnEnable()
    {
        DisableAllHuds();
    }
    private void OnDisable()
    {
        DisableAllHuds();
    }

    private void DisableAllHuds()
    {
        abilitiesTeamA.DesactiveHud();
        abilitiesTeamB.DesactiveHud();
    }
    private AbilitiesTeamHud GetTeamHud(PlayerController controller)
    {
        ControllerInputType it = controller.GetInputType();
        if (abilitiesTeamA.inputType == it)
            return abilitiesTeamA;
        else
            return abilitiesTeamB;
    }



}
