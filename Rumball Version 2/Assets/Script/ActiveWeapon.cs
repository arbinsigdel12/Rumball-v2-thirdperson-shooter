using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEditor.Animations;
using UnityEngine.Animations;

public class ActiveWeapon : MonoBehaviour
{
    // Start is called before the first frame update
    public enum WeaponSlot{
        primary = 0,
        secondary = 1
    }
    public Transform crossHAirTarget1;
    public Transform [] weaponSlots;
    public Transform weaponleftGrip;
    public Transform weaponRightGrip;
    [Header("RayCast Weapon")]
    RayCastWeapon[] equipped_weapon =new RayCastWeapon[2];
    int activeWeaponIndex;
    bool isholstered =false;
    public Animator rigController;
    public AmmoWidget ammoWidget;
    
    void Start()
    {
        RayCastWeapon existingweapon = GetComponentInChildren<RayCastWeapon>();
        if(existingweapon){
            Equip(existingweapon);
        }
    }

    public RayCastWeapon GetActiveWeapon(){
        return GetWeapon(activeWeaponIndex);
    }

    RayCastWeapon GetWeapon(int index){
        if(index<0||index>=equipped_weapon.Length){
            return null;            
        }
        return equipped_weapon[index];
    }

    // Update is called once per frame

    // Handles weapon firing and updating bullet states
    void Update(){
        var weapon= GetWeapon(activeWeaponIndex);
        if (weapon&& !isholstered)
        {
            // Allow firing only when the character is aiming
            if (Input.GetMouseButtonDown(0))
            {
                weapon.StartFiring(); // Start firing
            }

            if (weapon.isFiring)
            {
                weapon.UpdateFiring(Time.deltaTime); // Update firing logic
            }

            weapon.UpdateBullets(Time.deltaTime); // Update bullet states

            if (Input.GetMouseButtonUp(0))
            {
                weapon.StopFiring(); // Stop firing
            }
        }
        if(Input.GetKeyDown(KeyCode.X)){
            ToggleActiveWeapon();
        }
        if(Input.GetKeyDown(KeyCode.Alpha1)){
             SetActiveWeapon(WeaponSlot.primary);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            SetActiveWeapon(WeaponSlot.secondary);
        }
    }
    public void Equip(RayCastWeapon newWeapon){
        int weaponSlotIndex =(int)newWeapon.weaponSlot;
        var weapon= GetWeapon(weaponSlotIndex);
        if (weapon){
            Destroy(weapon.gameObject);
        }
        weapon=newWeapon;
        weapon.CrosshairTraget=crossHAirTarget1;
        weapon.transform.SetParent(weaponSlots[weaponSlotIndex],false);
        equipped_weapon[weaponSlotIndex]=weapon;
        activeWeaponIndex=weaponSlotIndex;
        SetActiveWeapon(newWeapon.weaponSlot);
        ammoWidget.Refresh(weapon.ammoCount);
    }

    void ToggleActiveWeapon(){
        bool isHolstered=rigController.GetBool("holster_weapon");
        if(isHolstered){
            StartCoroutine(ActivateWeapon(activeWeaponIndex));
        }else{
            StartCoroutine(HolsterWeapon(activeWeaponIndex));
        }
    }

    void SetActiveWeapon(WeaponSlot weaponSlot){
        int holsterindex=activeWeaponIndex;
        int activateindex=(int)weaponSlot;
        if(holsterindex ==activateindex){
            holsterindex=-1;
        }
        StartCoroutine(SwitchWeapon(holsterindex,activateindex));
    }

    IEnumerator SwitchWeapon(int holsterindex,int activateindex){
        yield return StartCoroutine(HolsterWeapon(holsterindex));
        yield return StartCoroutine(ActivateWeapon(activateindex));
        activeWeaponIndex =activateindex;
    }

    IEnumerator HolsterWeapon(int index){
        isholstered=true;
        var weapon=GetWeapon(index);
        if (weapon){
            rigController.SetBool("holster_weapon",true);
            do{
                yield return new WaitForEndOfFrame();
            }while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime<1.0f);
        }
    }

    IEnumerator ActivateWeapon(int index){
        var weapon=GetWeapon(index);
        if (weapon){
            rigController.SetBool("holster_weapon",false);
            rigController.Play("equip_"+weapon.weaponName);
            do{
                yield return new WaitForEndOfFrame();
            }while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime<1.0f);
            isholstered=false;
        }
    }
}
