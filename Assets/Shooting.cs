using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject SpawnPoint;
    public GameObject bullet;
    public float speed = 100f;

    private void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            GameObject instBullet = Instantiate(bullet, SpawnPoint.transform.position, Quaternion.identity) as GameObject;
            Rigidbody instBulletRB = instBullet.AddComponent<Rigidbody>();
            instBulletRB.AddForce( * speed);
            instBulletRB.useGravity = false;
            
        }
    }

}
