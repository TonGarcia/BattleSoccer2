using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{   
    
    public float CoolDown = 1.0f;

    public abstract void Initialize();
    public abstract void TriggerAbility();

}
