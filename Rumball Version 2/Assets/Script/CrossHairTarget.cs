using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairTarget : MonoBehaviour
{
    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hitInfo;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        ray.origin = mainCamera.transform.position;
        ray.direction = mainCamera.transform.forward;

        if (Physics.Raycast(ray, out hitInfo))
        {
            transform.position = hitInfo.point; // Position crosshair at hit point
        }
        else
        {
            transform.position = ray.origin + ray.direction * 1000f; // Default far point
        }
    }
}
