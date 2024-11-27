using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

public class RayCastWeapon : MonoBehaviour
{
    // Represents an individual bullet's data
    class Bullet
    {
        public float time;                    // Time since the bullet was fired
        public Vector3 initialPosition;       // Starting position of the bullet
        public Vector3 initialVelocity;       // Initial velocity of the bullet
        public TrailRenderer tracer;          // Visual tracer for the bullet
    }

    List<Bullet> bullets = new List<Bullet>(); // List to store all active bullets

    public ActiveWeapon.WeaponSlot weaponSlot;

    [Header("Bullet Status")]
    public bool isFiring = false;           // Tracks whether the weapon is currently firing
    public FloatValue fireRate;               // Number of bullets fired per second
    public FloatValue bulletSpeed ;     // Speed of the bullets
    public FloatValue bulletDrop ;         // Simulated gravity affecting bullets

    [Header("BulletLifeTime")]
    public FloatValue accumulatedTime;          // Time accumulator for handling fire rate
    public FloatValue maxlifetime;      // Maximum lifetime of a bullet before it is destroyed

    [Header("Bullet's Start and End Points")]
    public Transform raycastorigin;        // Origin point of the bullet
    public Transform CrosshairTraget;      // Target point where bullets are aimed

    [Header("Effects")]
    public ParticleSystem[] muzzleFlash;   // Array of muzzle flash particle effects
    public ParticleSystem hitEffect;       // Particle effect for bullet impacts
    public TrailRenderer tracerEffect;  
    public string weaponName;// Trail effect for visualizing bullets

    [Header("RayCast")]
    Ray ray;                               // Ray for bullet trajectory
    RaycastHit hitInfo;                    // Information about a raycast hit
    public GameObject magazine;
    public int ammoCount;
    public int clipSize;

    // PUBLIC CONTROL FUNCTIONS

    // Starts firing the weapon
    public void StartFiring()
    {
        accumulatedTime.value= 0f;
        isFiring=true;
        FireBullet();        
    }

    // Stops firing the weapon
    public void StopFiring()
    {
        isFiring = false;
    }

    // Updates firing logic to ensure bullets are fired at the specified rate
    public void UpdateFiring(float deltaTime)
    {
        accumulatedTime.value += deltaTime;
        float fireInterval = 1.0f / fireRate.value;
        while (accumulatedTime.value >= 0)
        {
            FireBullet();
            accumulatedTime.value -= fireInterval;
        }
    }

    // CORE UPDATE FUNCTIONS

    // Updates all active bullets
    public void UpdateBullets(float deltaTime)
    {
        SimulateBullets(deltaTime);         // Simulates bullet physics and movement
        DestroyBullets();                   // Removes bullets that exceed their lifetime
    }

    // BULLET SIMULATION FUNCTIONS

    // Simulates the movement of all bullets
    void SimulateBullets(float deltaTime)
    {
        bullets.ForEach(bullet =>
        {
            Vector3 p0 = GetPosition(bullet);  // Current position
            bullet.time += deltaTime;
            Vector3 p1 = GetPosition(bullet);  // New position after deltaTime
            RayCastSegment(p0, p1, bullet);    // Handle collisions and movement
        });
    }

    // Removes bullets that have exceeded their maximum lifetime
    void DestroyBullets()
    {
        bullets.RemoveAll(bullet => bullet.time >= maxlifetime.value);
    }

    // Handles raycasting for a bullet's trajectory
void RayCastSegment(Vector3 start, Vector3 end, Bullet bullet)
{
    Vector3 direction = end - start;
    float distance = direction.magnitude;
    ray.origin = start;
    ray.direction = direction;

    if (Physics.Raycast(ray, out hitInfo, distance))
    {
        // Handle bullet hitting a surface
        if (hitEffect != null) // Check if hitEffect exists
        {
            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);
        }

        // Check if the tracer exists before accessing it
        if (bullet.tracer != null)
        {
            bullet.tracer.transform.position = hitInfo.point;
        }

        bullet.time = maxlifetime.value;  // End the bullet's lifetime
        end = hitInfo.point;

        var rb2d = hitInfo.collider.GetComponent<Rigidbody>();
        if (rb2d)
        {
            rb2d.AddForceAtPosition(ray.direction * 20, hitInfo.point, ForceMode.Impulse);
        }
    }
    else
    {
        // Move the bullet to its end position
        if (bullet.tracer != null)
        {
            bullet.tracer.transform.position = end;
        }
    }
}


    // UTILITY FUNCTIONS

    // Fires a single bullet and triggers muzzle flash
    private void FireBullet()
    {  
        if(ammoCount <=0){
            return;
        }
        ammoCount--;
        // Tracks whether the weapon is currently firing
        foreach (var particle in muzzleFlash)
        {
            particle.Emit(1); // Emit muzzle flash particles
        }

        // Calculate direction from raycast origin to crosshair target
        Vector3 targetPosition = CrosshairTraget.position;

        // Adjust velocity to avoid misalignment
        Vector3 direction = (targetPosition - raycastorigin.position).normalized;
        Vector3 velocity = direction * bulletSpeed.value;

        var bullet = CreateBullet(raycastorigin.position, velocity);
        bullets.Add(bullet);
    }


    // Creates a new bullet with specified position and velocity
    Bullet CreateBullet(Vector3 position, Vector3 velocity)
    {
        Bullet bullet = new Bullet();
        bullet.initialPosition = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0.0f;
        bullet.tracer = Instantiate(tracerEffect, position, Quaternion.identity);
        bullet.tracer.AddPosition(position);
        Color color =Random.ColorHSV(0.46f,0.61f);
        float intensity =20.0f;
        Color rgb =new Color(color.r*intensity,color.g*intensity,color.b*intensity,color.a*intensity);
        bullet.tracer.material.SetColor("_EmissionColor",rgb);
        return bullet;
    }

    // Calculates the position of a bullet based on time and velocity
    Vector3 GetPosition(Bullet bullet)
    {
        Vector3 gravity = Vector3.down * bulletDrop.value;
        return bullet.initialPosition + (bullet.initialVelocity * bullet.time) +
               (0.5f * gravity * bullet.time * bullet.time);
    }
}
