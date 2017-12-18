using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using SoccerGame;
using System;

public class TugOfWar : MonoBehaviour
{
    [SerializeField]
    private BipedIK biped;
    [SerializeField]
    private Transform spline;
    [SerializeField]
    private float speedIKDump = 3.5f;
    [SerializeField]
    private float handDistance = 1.5f;
    [SerializeField]
    private float tugWarDistance = 1.0f;
    [SerializeField]
    private bool activeWithBall = false;

    PlayerController player;
    PlayerController jointPlayer;

    private void Start()
    {
        player = GetComponent<PlayerController>();
    }
    
    // Update is called once per frame
    void Update()
    {
        SmootIKWeight();

        PlayerController mnPlayer;
        mnPlayer = player.GetPlayerNear();

        //Estica maos para o jogaodr mais proximo
        HandleHandsTo(mnPlayer);

        if (jointPlayer)
        {
            if (!player.isOk ||
                player.Locomotion.inStumble ||
                player.Locomotion.inTrack || 
                player.IsMyBall() || 
                !jointPlayer.isOk ||
                !player.GetSkill_BasicActionOne().IsReady
                
                )
                RemoveJoint();
        }
        else if (player.Locomotion.inHoldTug)
        {
            if (mnPlayer != null && !player.Locomotion.inAir && player.isOk && !mnPlayer.IsMyTeaM(player))
            {
                float dist = mnPlayer.Distance(player);
                if (dist <= tugWarDistance && dist > 0.5f && mnPlayer.isOk && !mnPlayer.Locomotion.inAir)
                {
                    //JOINT
                    player.Locomotion.JointTo(mnPlayer);
                    jointPlayer = mnPlayer;
                    jointPlayer.GetAnimatorEvents().OnChangeDirStart += EnemyOnChangeDir;
                }
            }
        }

        //CONTROLE MANUAL
        if (!player.IsIA)
        {
            if (player.IsMyBall() || player.isOk==false)
            {
                if (player.Locomotion.inHoldTug)
                    player.Locomotion.ResetHoldTugAnimator();
                player.GetSkill_BasicActionOne().mode = SkillVarMode.autoSubtract;
                return;
            }
                        
            if (ControllerInput.GetButton(player.GetInputType(), player.GetInputs().Input_Kick) && player.isOk)
            {
                SkillVar skilltug = player.GetSkill_BasicActionOne();
                skilltug.SetToggle();
                

                if (skilltug.IsMax)
                {
                    skilltug.TriggerCooldown();
                   
                }

                if (skilltug.IsReady)
                {
                    skilltug.mode = SkillVarMode.autoRegen;
                    if (jointPlayer == null)
                        player.Locomotion.SetHoldTugAnimator();
                    else
                        player.Locomotion.ResetHoldTugAnimator();
                }
                else
                {
                    skilltug.SetCurrentValue(0);
                    skilltug.mode = SkillVarMode.nothing;
                    player.Locomotion.ResetHoldTugAnimator();
                }
            }

            if (ControllerInput.GetButtonUp(player.GetInputType(), player.GetInputs().Input_Kick))
            {
                SkillVar skilltug = player.GetSkill_BasicActionOne();
                if (skilltug.isToggle == false)
                    return;

                skilltug.mode = SkillVarMode.autoSubtract;
                player.Locomotion.ResetHoldTugAnimator();                
                skilltug.ResetTogle();

                if(skilltug.IsReady)
                    RemoveJoint();
            }

        }

    }

    private void EnemyOnChangeDir()
    {
        jointPlayer.Locomotion.SetTrip();
        RemoveJoint();

    }

    void RemoveJoint()
    {

        player.Locomotion.RemoveJoint();
        if (jointPlayer == null)
            return;

        jointPlayer.GetAnimatorEvents().OnChangeDirStart -= EnemyOnChangeDir;
        jointPlayer.Locomotion.TriggerStumb();
        jointPlayer = null;

    }
    void HandleHandsTo(PlayerController mnPlayer)
    {
        if (activeWithBall == false && player.IsMyBall())
        {
            ResetArmTarget();
            return;
        }

        if (mnPlayer != null && !player.Locomotion.inAir && player.isOk)
        {
            if (mnPlayer.Distance(player) <= handDistance && mnPlayer.isOk && !mnPlayer.Locomotion.inAir)
            {
                TugOfWar tug = mnPlayer.GetComponent<TugOfWar>();
                SetArmTarget(tug);
            }
            else
            {
                ResetArmTarget();
            }

        }
        else
        {
            ResetArmTarget();

        }

    }
    void SmootIKWeight()
    {
        //Left hand
        if (biped.solvers.leftHand.target != null)
            biped.solvers.leftHand.IKPositionWeight = Mathf.Lerp(biped.solvers.leftHand.IKPositionWeight, 1, speedIKDump * Time.deltaTime);
        else
            biped.solvers.leftHand.IKPositionWeight = Mathf.Lerp(biped.solvers.leftHand.IKPositionWeight, 0, speedIKDump * Time.deltaTime);

        //Right hand

        if (biped.solvers.rightHand.target != null)
            biped.solvers.rightHand.IKPositionWeight = Mathf.Lerp(biped.solvers.rightHand.IKPositionWeight, 1, speedIKDump * Time.deltaTime);
        else
            biped.solvers.rightHand.IKPositionWeight = Mathf.Lerp(biped.solvers.rightHand.IKPositionWeight, 0, speedIKDump * Time.deltaTime);
    }
    void SetArmTarget(TugOfWar tug)
    {
        float dir = player.LeftRightDir(tug.transform.position);
        if (dir > 0)
        {
            biped.solvers.rightHand.target = tug.spline;
            biped.solvers.leftHand.target = null;

        }
        else if (dir < 0)
        {
            biped.solvers.leftHand.target = tug.spline;
            biped.solvers.rightHand.target = null;

        }
        else
        {
            biped.solvers.rightHand.target = tug.spline;
            biped.solvers.leftHand.target = tug.spline;

        }

    }
    void ResetArmTarget()
    {

        biped.solvers.leftHand.target = null;
        biped.solvers.rightHand.target = null;

    }
}
