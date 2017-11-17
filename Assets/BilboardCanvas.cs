using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BilboardCanvas : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float scaleSize = 0.1f;

    public float dampingTime = 5.5f;

    // Update is called once per frame
    void Update()
    {
        Vector3 target = (transform.position + Camera.main.transform.rotation * Vector3.forward);
        Vector3 direction = target - transform.position;

        Quaternion rotation = Quaternion.LookRotation(direction, Camera.main.transform.rotation * Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, dampingTime * Time.deltaTime);

        //Scalse
        Plane plane = new Plane(Camera.main.transform.forward, Camera.main.transform.position);
        float dist = plane.GetDistanceToPoint(transform.position);
        Vector3 scale = new Vector3(0.01f, 0.01f, 0.01f) * dist * scaleSize;

        transform.localScale = scale;
    }
}
