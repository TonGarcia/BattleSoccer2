using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{


    public Transform target;
    [Range(0, 10)]
    public float dampPosition = 2.5f;
    [Range(0, 10)]
    public float dampRotation = 2.5f;
    public Vector3 targetOffsetPos;
    private Vector3 oldPos;

   
    // Update is called once per frame
    void Update()
    {
        
        oldPos = transform.position;
        Vector3 newPos = new Vector3(target.position.x + targetOffsetPos.x, target.position.y + targetOffsetPos.y, target.position.z + targetOffsetPos.z);
        
        //Transform to new position damped
        transform.position = Vector3.Lerp(oldPos, newPos, dampPosition * Time.deltaTime);
        //Look at and dampen the rotation
        Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
       
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * dampRotation);
        
    }
}
