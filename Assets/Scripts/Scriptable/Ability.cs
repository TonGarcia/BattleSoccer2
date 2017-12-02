using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;

public enum AbilityMode
{
    witBall,
    witOutBall,
    both
}
public abstract class Ability : ScriptableObject
{
   
    public Sprite Sprite;
    public AbilityMode mode;

    [SerializeField]
    private SkillVar skill;
    public SkillVar Skill { get { return skill; } }

    public abstract void Initialize(PlayerController controller);
    public abstract void TriggerAbility();
  

}
