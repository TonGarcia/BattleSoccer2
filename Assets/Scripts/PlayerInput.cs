using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;

[System.Serializable]
public class PlayerInput
{
   
    [SerializeField]
    private ButtomInputType InputJump;
    [SerializeField]
    private ButtomInputType InputPass;
    [SerializeField]
    private ButtomInputType InputKick;
    [SerializeField]
    private ButtomInputType InputStamina;
    [SerializeField]
    private ButtomInputType InputStrafe;
    [SerializeField]
    private ButtomInputType InputSelection;

    public ButtomInputType Input_Jump { get { return InputJump; } }
    public ButtomInputType Input_Pass { get { return InputPass; } }
    public ButtomInputType Input_Kick { get { return InputKick; } }
    public ButtomInputType Input_Stamina { get { return InputStamina; } }
    public ButtomInputType Input_Strafe { get { return InputStrafe; } }
    public ButtomInputType Input_Selection { get { return InputSelection; } }





}
