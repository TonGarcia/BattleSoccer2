using System;
using System.Collections;
using System.Collections.Generic;
using SoccerGame;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Abilities/Locomotion")]
public class LocomotionAbility : Ability
{
    [HideInInspector]
    public PlayerController controller;
    [HideInInspector]
    public PlayeSkills playerSkills;

    [Range(0, 1)]
    public float speedMultply = 1;

    public override void Initialize(PlayerController controller)
    {
        this.controller = controller;
        this.playerSkills = controller.GetPlayerSkills();
    }
   
    public override void TriggerAbility()
    {

    }


}
