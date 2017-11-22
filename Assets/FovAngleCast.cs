using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoccerGame;

public class FovAngleCast : MonoBehaviour
{

    public PlayerController player;
    public Transform center;
    public Vector3 target
    {
        get
        {
            Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.up);
            Vector3 direction = transform.forward * 20;
            Vector3 target = rotation * direction;
            return target;
        }
    }

    public bool IsHitting { get { return isHit; } }

    bool isHit = false;
    float angle = 0;

    // Update is called once per frame
    void LateUpdate()
    {
        if (isHit == true)
        {
            angle += 30;

            Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.up);
            //Vector3 direction = transform.forward * 20;
            //Vector3 target = rotation * direction;
            center.rotation = rotation;
        }
    }


    private void OnTriggerEnter(Collider other)
    {

        isHit = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isHit = false;
    }
}
