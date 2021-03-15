using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform SpawnPoint;
    public GameObject bullet;
    public Camera Camera;
    public float speed = 100f;
    public float fireRate;
    public float nextFire;
    public float coolDown;
    public float coolDownSeconds;
    public float ammo;
    public float maxAmmo;
    public bool IsFullAuto;

    private void Update()
    {
        while (!IsFullAuto)
        {
            if (fireRate == 0 && Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
            else
            {
                if (Input.GetButtonDown("Fire1") && Time.time > nextFire && fireRate > 0)
                {
                    if (ammo > 0)
                    {
                        nextFire = Time.time + fireRate;
                        Shoot();
                        ammo--;
                    }
                    else if (ammo == 0)
                    {
                        if (coolDown > Time.time)
                        {
                            coolDown = Time.time + coolDownSeconds;
                        }
                    }


                }
            }
            if (Input.GetButton("Reload"))
            {
                ammo = maxAmmo;
            }
            break;
        }
        while (IsFullAuto)
        {
            if (fireRate == 0 && Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
            else
            {
                if (Input.GetButton("Fire1") && Time.time > nextFire && fireRate > 0)
                {
                    if (ammo > 0)
                    {
                        nextFire = Time.time + fireRate;
                        Shoot();
                        ammo--;
                    }
                    else if (ammo == 0)
                    {
                        if (coolDown > Time.time)
                        {
                            coolDown = Time.time + coolDownSeconds;
                        }
                    }


                }
            }
            if (Input.GetButton("Reload"))
            {
                ammo = maxAmmo;
            }
            break;
        }

    }

    void Shoot()
    {
        Ray ray = Camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(1000);
        }
        var instBullet = Instantiate(bullet, SpawnPoint.transform.position, SpawnPoint.transform.rotation) as GameObject;
        instBullet.GetComponent<Rigidbody>().velocity = (targetPoint - SpawnPoint.transform.position).normalized * 10;


    }

}
