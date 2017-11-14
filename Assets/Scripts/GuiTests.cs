using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiTests : MonoBehaviour
{

    public PlayerController player;

    private void OnGUI()
    {
        if (player == null || BallController.instance == null)
        {
            GUILayout.Label("Aguarde...");
            return;
        }

        GUILayout.Label(player.gameObject.name);

        if (player.Locomotion.motionType == SoccerGame.LocomotionType.normal)
            GUILayout.Label("Movimento atual: NORMAL");
        else if(player.Locomotion.motionType == SoccerGame.LocomotionType.soccer)
            GUILayout.Label("Movimento atual: SOCCER");
        else
            GUILayout.Label("Movimento atual: STRAFE");

        if (GUILayout.Button("Motion Normal"))
        {
            player.SetMotionNormal();
        }
        if (GUILayout.Button("Motion Soccer"))
        {
            player.SetMotionSoccer();
        }
        if (GUILayout.Button("Motion Strafe"))
        {
            player.SetMotionStrafe();
        }

    }
}
