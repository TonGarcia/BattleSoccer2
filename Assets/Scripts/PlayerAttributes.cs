using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;


public class PlayerAttributes : MonoBehaviour
{  
    public SkillVar Stamina;
    
    public void Update()
    {
        
        Stamina.Update();
    }

}
