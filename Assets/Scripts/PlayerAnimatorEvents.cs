using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorEvents : MonoBehaviour
{

    public delegate void EvntAnimatorArgs();
    public delegate void EvntAnimatorArgsStr(string arg);

    //Pass
    public event EvntAnimatorArgs OnPassStart;
    public event EvntAnimatorArgs OnPassOk;
    public event EvntAnimatorArgs OnPassFinish;
    //Kick
    public event EvntAnimatorArgs OnKickStart;
    public event EvntAnimatorArgs OnKickOk;
    public event EvntAnimatorArgs OnKickFinish;
    //Change direction
    public event EvntAnimatorArgs OnChangeDirStart;
    public event EvntAnimatorArgs OnChangeDirOk;
    public event EvntAnimatorArgs OnChangeDirFinish;
    //Turn direction
    public event EvntAnimatorArgs OnTurnStart;
    public event EvntAnimatorArgs OnTurnOk;
    public event EvntAnimatorArgs OnTurnFinish;
    //Enttry
    public event EvntAnimatorArgs OnEnttryStart;
    public event EvntAnimatorArgs OnEnttryFinish;
    //Stumble
    public event EvntAnimatorArgs OnStumblesStart;
    public event EvntAnimatorArgs OnStumblesFinish;
    //Attack
    public event EvntAnimatorArgs OnTrackingStart;
    public event EvntAnimatorArgs OnTrackingOk;
    public event EvntAnimatorArgs OnTrackingFinish;
    //Triping
    public event EvntAnimatorArgs OnTripingStart;
    public event EvntAnimatorArgs OnTripingFinish;
    //StandUP
    public event EvntAnimatorArgs OnOnStandingupFinish;
    //Hand Attack
    /// <summary>
    /// Triger event when handing attack has startet. Use argument to hand side attack
    /// </summary>
    public event EvntAnimatorArgsStr OnHandingAttackStart;
    /// <summary>
    /// Triger event when handing attack has trigered. Use argument to hand side attack
    /// </summary>
    public event EvntAnimatorArgsStr OnHandingAttackOk;
    /// <summary>
    /// Triger event when handing attack has finishied. Use argument to hand side attack
    /// </summary>
    public event EvntAnimatorArgsStr OnHandingAttackFinish;

    //Hello

    //Change direction
    private void Anim_OnChangeDirectionStart()
    {
        if (OnChangeDirStart != null)
            OnChangeDirStart();
    }
    private void Anim_OnChangeDirectionOk()
    {
        if (OnChangeDirOk != null)
            OnChangeDirOk();
    }
    private void Anim_OnChangeDirectionFinish()
    {
        if (OnChangeDirFinish != null)
            OnChangeDirFinish();
    }
    //Turn direction
    private void Anim_OnTurnDirectionStart()
    {
        if (OnTurnStart != null)
            OnTurnStart();
    }
    private void Anim_OnTurnDirectionOk()
    {
        if (OnTurnOk != null)
            OnTurnOk();
    }
    private void Anim_OnTurnDirectionFinish()
    {
        if (OnTurnFinish != null)
            OnTurnFinish();
    }
    //Kick
    private void Anim_OnLongKickOk()
    {
        if (OnKickOk != null)
            OnKickOk();
    }
    private void Anim_OnLongKickStart()
    {
        if (OnKickStart != null)
            OnKickStart();
    }
    private void Anim_OnLongKickFinish()
    {
        if (OnKickFinish != null)
            OnKickFinish();
    }
    //Enttry - Esbarrão
    private void Anim_OnEntryStart()
    {
        if (OnEnttryStart != null)
            OnEnttryStart();
    }
    private void Anim_OnEntryFinish()
    {
        if (OnEnttryFinish != null)
            OnEnttryFinish();
    }
    //Pass
    private void Anim_OnShortPassStart()
    {
        if (OnPassStart != null)
            OnPassStart();
    }
    private void Anim_OnShortPassOk()
    {
        if (OnPassOk != null)
            OnPassOk();

    }
    private void Anim_OnShortPassFinish()
    {
        if (OnPassFinish != null)
            OnPassFinish();
    }
    //Stumble - Tropeçar
    private void Anim_OnStumbleStart()
    {
        if (OnStumblesStart != null)
            OnStumblesStart();
    }
    private void Anim_OnStumbleFinish()
    {
        if (OnStumblesFinish != null)
            OnStumblesFinish();
    }
    //Track - Rasteira
    private void Anim_OnTrackStart()
    {
        if (OnTrackingStart != null)
            OnTrackingStart();
    }
    private void Anim_OnTrackOk()
    {
        if (OnTrackingOk != null)
            OnTrackingOk();
    }
    private void Anim_OnTrackFinish()
    {
        if (OnTrackingFinish != null)
            OnTrackingFinish();
    }
    //Trip - Cair
    private void Anim_OnTripStart()
    {
        if (OnTripingStart != null)
            OnTripingStart();
    }
    private void Anim_OnTripFinish()
    {
        if (OnTripingFinish != null)
            OnTripingFinish();
    }
    //StandUp - Levantar
    private void Anim_OnStandupFinish()
    {
        if (OnOnStandingupFinish != null)
            OnOnStandingupFinish();
    }
    //Hand attack
    private void Anim_OnHandAtk_Start(string side)
    {
        if (OnHandingAttackStart != null)
            OnHandingAttackStart(side);
    }
    private void Anim_OnHandAtk_Ok(string side)
    {
        if (OnHandingAttackOk != null)
            OnHandingAttackOk(side);
    }
    private void Anim_OnHandAtk_Finish(string side)
    {
        if (OnHandingAttackFinish != null)
            OnHandingAttackFinish(side);
    }
}
