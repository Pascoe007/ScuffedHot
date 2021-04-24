using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public float damage;
    public float range;
    public Camera camera;
    public ParticleSystem muzzelFlash;

   

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        muzzelFlash.Play();
        RaycastHit hit;
        if(Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, range))
        {
           Units units = hit.transform.GetComponent<Units>();
            if(units != null)
            {
                units.TakeDamage(damage);
            }
        }
    }
}
