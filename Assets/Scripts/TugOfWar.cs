using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;
using SoccerGame;
using System;

public class TugOfWar : MonoBehaviour
{

    public BipedIK biped;
    public Transform spline;
    public float speedIKDump = 3.5f;
    public float handDistance = 1.5f;
    public float tugWarDistance = 1.0f;

    public bool activeWithBall = false;
    PlayerController player;
    PlayerController jointPlayer;

    private void Start()
    {
        player = GetComponent<PlayerController>();
        GetComponent<PlayerAnimatorEvents>().OnHandingAttackStart += OnHandAttak;
        GetComponent<PlayerAnimatorEvents>().OnHandingAttackOk += OnHandAttak;
        GetComponent<PlayerAnimatorEvents>().OnHandingAttackFinish += OnHandAttak;
    }

    private void OnDestroy()
    {
        GetComponent<PlayerAnimatorEvents>().OnHandingAttackStart -= OnHandAttak;
        GetComponent<PlayerAnimatorEvents>().OnHandingAttackOk -= OnHandAttak;
        GetComponent<PlayerAnimatorEvents>().OnHandingAttackFinish -= OnHandAttak;
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
            if (player.isOk == false || player.Locomotion.inStumble || player.Locomotion.inTrack)
                RemoveJoint();
        }
        else if (player.Locomotion.inHandAttak)
        {
            if (mnPlayer != null && !player.Locomotion.inAir && player.isOk && !mnPlayer.IsMyTeaM(player))
            {
                float dist = mnPlayer.Distance(player);
                if (dist <= handDistance && dist > 0.5f && mnPlayer.isOk && !mnPlayer.Locomotion.inAir)
                {
                    player.Locomotion.JointTo(mnPlayer);
                    jointPlayer = mnPlayer;
                    jointPlayer.GetAnimatorEvents().OnChangeDirStart += EnemyOnChangeDir;
                }
            }
        }

    }


    private void OnHandAttak(string arg)
    {
        if (jointPlayer)
        {
            RemoveJoint();
            return;

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
        jointPlayer.GetAnimatorEvents().OnChangeDirStart -= EnemyOnChangeDir;
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
