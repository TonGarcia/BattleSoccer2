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
        public UIAbilitySkill ui_locomotionSkill;
        public UIAbilitySkill ui_atkSkill;
        public UIAbilitySkill ui_defSkill;

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

            //Atualiza a mascara de cooldown das ações
            PlayerController player = team.GetSelectedPlayer();
            if (player == null)
            {
                container.gameObject.SetActive(false);
                return;
            }

            //Ativamos HUD
            container.gameObject.SetActive(true);

            //Locomotion skill
            Ability abilityLocomotion = player.GetAbility(0);
            HandleAbility(player, abilityLocomotion);

        }

        private bool CheckAbility(PlayerController controller, Ability ability)
        {
            bool result = false;

            if (ability != null)
            {
                switch (ability.mode)
                {
                    case AbilityMode.witBall:
                        if (controller.IsMyBall())
                            result = true;

                        break;

                    case AbilityMode.witOutBall:
                        if (!controller.IsMyBall())
                            result = true;

                        break;
                    case AbilityMode.both:
                        result = true;
                        break;
                }
            }

            return result;

        }
        private void HandleAbility(PlayerController controller, Ability ability)
        {
            if (CheckAbility(controller,ability))
            {
                SkillVar skill = ability.Skill;
                ui_locomotionSkill.SetIcon(ability.Sprite);
                ui_locomotionSkill.EnableCooldown();
                ui_locomotionSkill.EnableValue();

                ui_locomotionSkill.SetValue(skill.FillAmounValue);
                ui_locomotionSkill.SetCooldown(skill.FillAmountCooldown);

                if (skill.IsCritical)
                    ui_locomotionSkill.SetValueColor(Color.red);
                else
                    ui_locomotionSkill.ResetValueColor();
            }
            else
            {
                ui_locomotionSkill.ResetIcon();
                ui_locomotionSkill.DisableCooldown();
                ui_locomotionSkill.DisableValue();
            }
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
