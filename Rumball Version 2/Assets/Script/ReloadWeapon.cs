using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadWeapon : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator rigcontroller;
    public WeaponAnimationEvents animationEvents;
    public ActiveWeapon activeWeapon;
    public Transform lefthand;
    public AmmoWidget ammoWidget;

    GameObject magazinehand;
    void Start()
    {
        animationEvents.WeaponAnimationEvent.AddListener(OnAnimationEvent);
    }

    // Update is called once per frame
    void Update()
    {
        RayCastWeapon weapon =activeWeapon.GetActiveWeapon();
        if(weapon){
            if(Input.GetKeyDown(KeyCode.R)||weapon.ammoCount<=0){
                rigcontroller.SetTrigger("reload_weapon");  

            }
            if(weapon.isFiring){
                ammoWidget.Refresh(weapon.ammoCount);
            }
        }
    }

    void OnAnimationEvent(string eventName){
        switch(eventName){
            case "detach_magazine":
                DetachMagazine();
                break;
            case "dropping_magazine":
                DropMagazine();
                break;
            case "refilling_magazine":
                RefillMagazine();
                break;
            case "attach_magazine":
                AttachMagazine();
                break;
        }
    }

    void DetachMagazine(){
        RayCastWeapon weapon =activeWeapon.GetActiveWeapon();
        magazinehand =Instantiate(weapon.magazine,lefthand,true);
        weapon.magazine.SetActive(false);
    }

    void DropMagazine(){
        GameObject droppedMagazine =Instantiate(magazinehand,magazinehand.transform.position,magazinehand.transform.rotation);
        droppedMagazine.AddComponent<Rigidbody>();
        droppedMagazine.AddComponent<BoxCollider>();
        magazinehand.SetActive(false);
    }

    void RefillMagazine(){
        magazinehand.SetActive(true);
    }

    void AttachMagazine(){
        RayCastWeapon weapon= activeWeapon.GetActiveWeapon();
        weapon.magazine.SetActive(true);
        Destroy(magazinehand);
        weapon.ammoCount=weapon.clipSize;
        rigcontroller.ResetTrigger("reload_weapon");
        ammoWidget.Refresh(weapon.ammoCount);
    }
}
