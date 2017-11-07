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
    private ButtomInputType InputShortPass;
    [SerializeField]
    private ButtomInputType InputKick;


    public ButtomInputType Input_Jump { get { return InputJump; } }
    public ButtomInputType Input_ShortPass { get { return InputShortPass; } }
    public ButtomInputType Input_Kick { get { return InputKick; } }
    public bool IsAI { get { return InputType == ControllerInputType.ControllerCPU; } }
    public bool IsController1 { get { return InputType == ControllerInputType.Controller1; } }
    public bool IsController2 { get { return InputType == ControllerInputType.Controller2; } }


}
