using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float dampTime = 1.0f;
    private Vector3 velocity = Vector3.zero;
    public Transform target;
 
     // Update is called once per frame
     void Update () 
     {
        Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target.position);

         Debug.Log("Target Point: " + point.y);
         if (point.y >= 1)
         {
             Vector3 delta = target.position;// - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(point.x, 0.1f, point.z)); //(new Vector3(0.5, 0.5, point.z));
             Vector3 destination = transform.position + delta;
             transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
         }
     
     }
}
