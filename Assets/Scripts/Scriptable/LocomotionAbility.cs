using System;
using System.Collections;
using System.Collections.Generic;
using SoccerGame;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Abilities/Locomotion")]
public class LocomotionAbility : Ability
{
    [Range(0,1)]
    public float speedMultiply = 0;

    public override void Initialzie(out SkillVar skillClone)
    {
        skill.ability = this;
        skillClone = (SkillVar)skill.Clone();
    }
}
