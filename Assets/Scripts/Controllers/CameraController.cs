using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;

public class CameraController : MonoBehaviour
{


    public Transform target;
    [Range(0, 10)]
    public float dampPosition = 2.5f;
    [Range(0, 10)]
    public float dampRotation = 2.5f;
    [Range(0, 10)]
    public float zoom = 0.0f;

    public Vector3 targetOffsetPos;

    public bool automaticZoom = true;

    private Vector3 oldPos;


    // Update is called once per frame
    void Update()
    {
        PlayerController p1, p2;
        p1 = GameManager.instance.GetSelectedPlayer(CampTeam.Team_A);
        p2 = GameManager.instance.GetSelectedPlayer(CampTeam.Team_B);

        //Controle de zom automatico
        if (automaticZoom)
        {

            if (p1 && p2)
            {
                float playersDistance = p1.Distance(p2);
                zoom = playersDistance / 4;

            }
        }

        Vector3 to = targetOffsetPos;
        to.x -= zoom;
        to.y += zoom;

        //  to.z -= Mathf.Cos(to.x - to.y)/180;

        oldPos = transform.position;
        Vector3 newPos = target.position + to;

        //Transform to new position damped
        transform.position = Vector3.Lerp(oldPos, newPos, dampPosition * Time.deltaTime);
        //Look at and dampen the rotation
        Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * dampRotation);

    }
}
