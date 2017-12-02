using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;

public abstract class Ability : ScriptableObject
{
    public Sprite icon_withBall;
    public Sprite icon_withOutBall;
    public SkillVar skill;

    public abstract void Initialzie(out SkillVar skillClone);
   
    
}
