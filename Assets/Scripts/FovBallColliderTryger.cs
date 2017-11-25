using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;

public class FovBallColliderTryger : MonoBehaviour
{

    PlayerController player;

    // Use this for initialization
    void Start()
    {
        player = GetComponentInParent<PlayerController>();
    }
   
    private void OnTriggerEnter(Collider other)
    {

        if (other == null)
            return;

        BallController ball = other.GetComponentInParent<BallController>();

        if (ball == null)
            return;

        if (ball.GetMyOwner() == player)
            return;

        //Nao podera pegar a bola se o jogaodr for do mesmo time. 
        //Considerado esbarrao. 
        PlayerController playerBall = ball.GetMyOwner();
        if(playerBall!=null)
        {
            if (playerBall.IsMyTeaM(player))
                return;
        }

        ball.SetmeOwner(player);

    }
}
