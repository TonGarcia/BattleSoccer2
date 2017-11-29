using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    PlayerController player;

    public Vector3 offset;

    void LateUpdate()
    {
        if (player != null)
        {
            transform.position = player.transform.position + offset;
        }
    }

    public void SelectPlayer(PlayerController player)
    {
        this.player = player;
    }
    public void Unselect()
    {
        this.player = null;
    }
}
