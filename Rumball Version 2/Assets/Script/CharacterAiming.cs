using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterAiming : MonoBehaviour
{
    RayCastWeapon weapon;
    [Header("Transition Settings")]
    public FloatValue transitionSpeed;    // Speed for smooth transitions
    public FloatValue turnspeed;          // Speed for aiming transitions
    Camera maincamera;
    public bool isFiring = false;  
    
    // INITIALIZATION FUNCTIONS

    // Called at the start of the game to initialize references and settings
    void Start()
    {
        maincamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        weapon =GetComponentInChildren<RayCastWeapon>();
    }

    // CHARACTER MOVEMENT FUNCTIONS

    // Handles character rotation to align with the camera's view
    void FixedUpdate()
    {
        float ycamCamera = maincamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.Euler(0, ycamCamera, 0),
            transitionSpeed.value * Time.deltaTime
        );
    }

    // CHARACTER ACTION FUNCTIONS

    // Handles aiming, rig transitions, and weapon firing logic
    void LateUpdate(){
        if (weapon)
        {
            if (Input.GetMouseButtonDown(1))
            {
                isFiring=true;
                weapon.StartFiring();
            }
            if (weapon.isFiring){
                weapon.UpdateFiring(Time.deltaTime);
            }
            weapon.UpdateBullets(Time.deltaTime);
            if (Input.GetMouseButtonUp(1))
            {
                isFiring=false;
                weapon.StopFiring();
            }
        }
    }
}
