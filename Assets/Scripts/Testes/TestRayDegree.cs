using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRayDegree : MonoBehaviour
{
    public Transform joint;
    float angle = 0;
    bool isHit = false;


    // Update is called once per frame
    void Update()
    {
        /*
        angle += 1;

        Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.up);
        Vector3 direction = transform.forward * 20;
        Vector3 target = rotation * direction;        

        if (Physics.Linecast(transform.position, rotation * direction) == false)
        {
            Debug.DrawRay(transform.position, target, Color.green);
        }
        else
        {
            Debug.DrawRay(transform.position, target, Color.red);
        }
        */

        if (isHit == true)
        {
            angle += 30;

            Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.up);
            //Vector3 direction = transform.forward * 20;
            //Vector3 target = rotation * direction;
            joint.rotation = rotation;
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Epa");
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter");
        isHit = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isHit = false;
        Debug.Log("exit");
    }

}
