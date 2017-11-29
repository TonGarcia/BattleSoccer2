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
        public Image imageStamina;
        public Image imageSpelKick;
        public Image imageSpelPass;
        public Image imageStaminaCooldownMask;
        public Image imageKickCooldownMask;
        public Image imagePassCooldownMask;

        public void ActiveHud()
        {
            container.gameObject.SetActive(true);
        }
        public void DesactiveHud()
        {
            container.gameObject.SetActive(false);
        }
        public void Update(Sprite staminaSprite, Sprite kickSprite, Sprite hitSprite, Sprite passSprite, Sprite defenseSprite, bool showIA)
        {
            //Esconde HUD se FOR AI e não estiver habilitado
            if (showIA == false && inputType == ControllerInputType.ControllerCPU)
            {
                container.gameObject.SetActive(false);
                return;

            }

            //Ativamos HUD
            container.gameObject.SetActive(true);

            //Modifica as sprites para mostrar as imagens corretas de ações com Bola e ações sem bola
            if (team.IsMyBall())//Sprites Ações com bola
            {
                imageStamina.sprite = staminaSprite;
                imageSpelKick.sprite = kickSprite;
                imageSpelPass.sprite = passSprite;
            }
            else //Sprites Ações sem bola
            {
                imageStamina.sprite = staminaSprite;
                imageSpelKick.sprite = hitSprite;
                imageSpelPass.sprite = defenseSprite;
            }

            //Atualiza a mascara de cooldown das ações
            PlayerController player = team.GetSelectedPlayer();
            if (player == null)
                return;

            SkillVar stamina = player.GetSkill_Stamina();
            imageStaminaCooldownMask.fillAmount = stamina.FillAmounValue;
            float staminCritical = (stamina.MaxValue / 2);
            if (stamina.IsCritical)
                imageStaminaCooldownMask.color = Color.red;
            else
                imageStaminaCooldownMask.color = Color.blue;


        }
    }

    public Sprite staminaSprite;
    public Sprite kickSprite;
    public Sprite hitSprite;
    public Sprite passSprite;
    public Sprite defenseSprite;

    public AbilitiesTeamHud abilitiesTeamA;
    public AbilitiesTeamHud abilitiesTeamB;

    public bool showAIHud = false;


    public void Update()
    {
        if (!GameManager.isReady)
            return;

        abilitiesTeamA.Update(staminaSprite, kickSprite, hitSprite, passSprite, defenseSprite, showAIHud);
        abilitiesTeamB.Update(staminaSprite, kickSprite, hitSprite, passSprite, defenseSprite, showAIHud);

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
