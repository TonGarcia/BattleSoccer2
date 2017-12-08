using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorEvents : MonoBehaviour
{
    
    public delegate void EvntAnimatorArgs();    

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
    //Hello


    private void OnChangeDirectionStart()
    {
        if (OnChangeDirStart != null)
            OnChangeDirStart();
    }
    private void OnChangeDirectionOk()
    {
        if (OnChangeDirOk != null)
            OnChangeDirOk();
    }
    private void OnChangeDirectionFinish()
    {
        if (OnChangeDirFinish != null)
            OnChangeDirFinish();
    }

    private void OnTurnDirectionStart()
    {
        if (OnTurnStart != null)
            OnTurnStart();
    }
    private void OnTurnDirectionOk()
    {
        if (OnTurnOk != null)
            OnTurnOk();
    }
    private void OnTurnDirectionFinish()
    {
        if (OnTurnFinish != null)
            OnTurnFinish();
    }

    private void OnLongKickOk()
    {
        if (OnKickOk != null)
            OnKickOk();
    }
    private void OnLongKickStart()
    {
        if (OnKickStart != null)
            OnKickStart();
    }
    private void OnLongKickFinish()
    {
        if (OnKickFinish != null)
            OnKickFinish();
    }

    private void OnEntryStart()
    {
        if (OnEnttryStart != null)
            OnEnttryStart();
    }
    private void OnEntryFinish()
    {
        if (OnEnttryFinish != null)
            OnEnttryFinish();
    }

    private void OnShortPassStart()
    {
        if (OnPassStart != null)
            OnPassStart();
    }
    private void OnShortPassOk()
    {
        if (OnPassOk != null)
            OnPassOk();   

    }
    private void OnShortPassFinish()
    {
        if (OnPassFinish != null)
            OnPassFinish();
    }

    private void OnStumbleStart()
    {
        if (OnStumblesStart != null)
            OnStumblesStart();
    }
    private void OnStumbleFinish()
    {
        if (OnStumblesFinish != null)
            OnStumblesFinish();
    }

    private void OnTrackStart()
    {
        if (OnTrackingStart != null)
            OnTrackingStart();
    }
    private void OnTrackOk()
    {
        if (OnTrackingOk != null)
            OnTrackingOk();
    }
    private void OnTrackFinish()
    {
        if (OnTrackingFinish != null)
            OnTrackingFinish();
    }

    private void OnTripStart()
    {
        if (OnTripingStart != null)
            OnTripingStart();
    }
    private void OnTripFinish()
    {
        if (OnTripingFinish != null)
            OnTripingFinish();
    }

    private void OnStandupFinish()
    {
        if (OnOnStandingupFinish != null)
            OnOnStandingupFinish();
    }
}
