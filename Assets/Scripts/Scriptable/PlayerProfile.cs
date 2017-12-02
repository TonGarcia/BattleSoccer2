using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Profile")]
public class PlayerProfile : ScriptableObject
{
    public PlayerClass playerClass;
    public Ability stamina;
    public Ability basicKick;
    public Ability basicPass;

}
