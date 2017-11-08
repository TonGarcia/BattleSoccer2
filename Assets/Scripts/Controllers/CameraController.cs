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

    public bool automaticZoom=true;

    private Vector3 oldPos;


    // Update is called once per frame
    void Update()
    {
        //Controle de zom automatico
        if (automaticZoom)
        {
            PlayerController p1, p2;
            p1 = GameManager.instance.GetSelectedPlayer(CampTeam.Team_A);
            p2 = GameManager.instance.GetSelectedPlayer(CampTeam.Team_B);
            if (p1 && p2)
            {
                float distance = p1.Distance(p2);
                zoom = distance / 4;
            }
        }

        oldPos = transform.position;
        Vector3 newPos = new Vector3(target.position.x + targetOffsetPos.x + zoom, target.position.y + targetOffsetPos.y + zoom, target.position.z + targetOffsetPos.z);
        //Transform to new position damped
        transform.position = Vector3.Lerp(oldPos, newPos, dampPosition * Time.deltaTime);
        //Look at and dampen the rotation
        Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * dampRotation);

    }
}
