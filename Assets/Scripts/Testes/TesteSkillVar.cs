using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;

public class TesteSkillVar : MonoBehaviour
{
   public SkillVar Stamina;

	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        Stamina.Update();

        if (Input.GetKeyDown(KeyCode.A))
            Stamina.AddCurrentValue(100);
        else if (Input.GetKeyDown(KeyCode.S))
            Stamina.SubCurrentValue(100);
        else if (Input.GetKeyDown(KeyCode.Space))
            Stamina.TriggerCooldown();

     
	}
}
