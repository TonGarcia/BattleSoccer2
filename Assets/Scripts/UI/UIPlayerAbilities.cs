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
