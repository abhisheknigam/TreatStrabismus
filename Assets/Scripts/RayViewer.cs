using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayViewer : MonoBehaviour {
    public float weaponRange = 50f;                       // Distance in Unity units over which the Debug.DrawRay will be drawn

    public Transform gunBarrelTransform;                                // Holds a reference to the first person camera

    void Start()
    {
    }


    void Update()
    {
        // Draw a line in the Scene View  from the point lineOrigin in the direction of fpsCam.transform.forward * weaponRange, using the color green
        Debug.DrawRay(gunBarrelTransform.position, gunBarrelTransform.forward * weaponRange, Color.red);
    }
}
