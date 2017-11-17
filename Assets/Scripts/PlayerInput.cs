using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;

public class PlayerInput : MonoBehaviour
{
    public ControllerInputType InputType;
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

    public ButtomInputType Input_Jump { get { return InputJump; } }
    public ButtomInputType Input_Pass { get { return InputPass; } }
    public ButtomInputType Input_Kick { get { return InputKick; } }
    public ButtomInputType Input_Stamina { get { return InputStamina; } }
    public ButtomInputType Input_Strafe { get { return InputStrafe; } }

    public bool IsAI { get { return InputType == ControllerInputType.ControllerCPU; } }
    public bool IsController1 { get { return InputType == ControllerInputType.Controller1; } }
    public bool IsController2 { get { return InputType == ControllerInputType.Controller2; } }


}
