using System;
using System.Collections;
using System.Collections.Generic;
using SoccerGame;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Abilities/Action")]
public class ActionAbility : Ability
{
   
    public override void Initialzie(out SkillVar skillClone)
    {
        skill.ability = this;
        skillClone = (SkillVar)skill.Clone();
    }
}
