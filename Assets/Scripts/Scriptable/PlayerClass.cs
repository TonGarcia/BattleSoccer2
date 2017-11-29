using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerClassType
{
    assassin,
    warior,
    guardian
}
[CreateAssetMenu(menuName = "Player/Class")]
public class PlayerClass : ScriptableObject
{

    public PlayerClassType classType;

    [Range(0,100)]
    public int speed;
    [Range(0, 100)]
    public int attack;
    [Range(0, 100)]
    public int defense;
    [Range(0, 100)]
    public int kickPower;

}
