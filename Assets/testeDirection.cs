using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class testeDirectionExtensions
{
    public static Vector3 direction(this Transform origim, Transform target)
    {
        return origim.position-target.position;
    }
    public static float angle(this Vector3 dirOrigim, Vector3 dirTarget)
    {
        return Vector3.Angle(dirOrigim, dirTarget);
    }
    public static float distance(this Vector3 origim, Vector3 to)
    {
        return Vector3.Distance(origim, to);
    }
    
    public static Transform minAngle(this List<Transform> transforms, Transform transform, Vector3 dirorigim)
    {
        float min = transforms.Min(r => r.direction(transform).angle(dirorigim));
        return transforms.FirstOrDefault(r => r.direction(transform).angle(dirorigim)==min);
    }
}
public class testeDirection : MonoBehaviour {

    public Transform selector;
    public List<Transform> objects;
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 mdirection = mouseDirection();

        Transform target = objects.minAngle(transform, mdirection);
        selector.transform.position = target.transform.position;       
        
        Quaternion rotation = Quaternion.LookRotation(mdirection, Vector3.up);
        transform.rotation = rotation;
        
    }

    Vector3 mouseDirection()
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 target = Vector3.zero;
        float hitdist = 0.0f;
        if (playerPlane.Raycast(ray, out hitdist))
        {
            Vector3 targetPoint = ray.GetPoint(hitdist);
            // Quaternion targetRotation = Quaternion.LookRotation(targetPoint - transform.position);
            // transform.rotation = targetRotation;
            target = targetPoint - transform.position;
        }

        return target;
    }
}
