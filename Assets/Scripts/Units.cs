﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Units : MonoBehaviour
{
    public GameObject ball;
    public Transform m_SpawnTransform;
    //public CharacterController controller;
    Rigidbody rb;
    FieldOfView fow;
    Grid grid;
    //public SpwanPoints spwanPoint;
    public Transform target;
    public float speed = 1;
    Vector3[] path;
    Vector3 velocity;
    int targetIndex;
    public float repeatRate;
    public bool hasWeapon = false;
    bool pathDone;

    public float health;


    void Start()
    {
        
        grid = GetComponent<Grid>();
        fow = GetComponent<FieldOfView>();
        rb = GetComponent<Rigidbody>();
        //spwanPoint = GetComponent <SpwanPoints>();
        InvokeRepeating("RunPath", 0.5f, repeatRate);
        SetKinematic(true);

        

    }
   
    void RunPath()
    {
        target = GameObject.Find("Player").transform;
        fow.FindVisiableWeapons();
        
        Transform tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Transform t in fow.visibleWeapons)
        {
            float dist = Vector3.Distance(t.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }
        
        ChoosePath(currentPos, minDist, tMin);
        
    }

    void ChoosePath(Vector3 currentPos, float minDist, Transform tMin)
    {
        
        
        float playerDist = Vector3.Distance(target.position, currentPos);
        while (tMin == null)
        {
            PathReqManager.RequestPath(transform.position, target.position, OnPathFound);
            break;
        }
        while(tMin != null)
        {
            while (!hasWeapon)
            {
                if (playerDist < Vector3.Distance(transform.position, tMin.transform.position))
                {
                    PathReqManager.RequestPath(transform.position, target.position, OnPathFound);
                    
                }
                else
                {
                    PathReqManager.RequestPath(transform.position, tMin.position, OnPathFound);
                    if(Vector3.Distance(transform.position, tMin.transform.position) < 3 && !hasWeapon)
                    {
                        StopCoroutine("FollowPath");
                        hasWeapon = true;
                        break;
                    }
                }
                
                
                break;
            }
            while (hasWeapon)
            {
                StartCoroutine(Shooting(target.transform.position));
                while (Vector3.Distance(transform.position, target.transform.position) < 20)
                {
                    Debug.Log("Stoped");
                    
                    break;
                }
                if(Vector3.Distance(transform.position, target.transform.position) > 20)
                {
                    PathReqManager.RequestPath(transform.position, target.position, OnPathFound);
                    break;
                }
                break;
            }
            break;
        }
        

        
            
        
    }



    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        
        Vector3 currentWaypoint = path[0];
        
        
        while (true)
        {
            float dst = Vector3.Distance(transform.position, currentWaypoint);
            //Debug.Log("dst" + dst);
            if (transform.position == currentWaypoint)
            {
                
                pathDone = true;
                targetIndex++;
                if (targetIndex <= path.Length)
                {
                    
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            currentWaypoint.y = transform.position.y;
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            //controller.Move(Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime));

            

            //rb.MovePosition(Vector3.Lerp(transform.position, currentWaypoint, ((Time.deltaTime - StartTime)*speed)/Vector3.Distance(transform.position, currentWaypoint)));

            
             
            transform.LookAt(currentWaypoint);


            yield return null;

        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
    public void TakeDamage(float amout)
    {
        health -= amout;
        if (health <= 0f)
        {
            SetKinematic(false);
            GetComponent<Animator>().enabled = false;
            Destroy(gameObject, 5);
            CancelInvoke("RunPath");
            //spwanPoint.enmeyCount--;
        }
    }
    void Die()
    {
        
       
    }
    
    void SetKinematic(bool state)
    {
        Rigidbody[] rb = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in rb)
        {
            rigidbody.isKinematic = state;
        }
    }
    IEnumerator Shooting(Vector3 Player)
    {
        m_SpawnTransform = this.gameObject.transform.GetChild(2);
        Player.y = transform.position.y;
        transform.LookAt(Player);
        RaycastHit hit;
        Vector3 shootDirection = transform.forward;
        shootDirection.x += Random.Range(-5, 5);
        shootDirection.y += Random.Range(-5, 5);
        if (Physics.Raycast(transform.position, shootDirection, out hit, 10))
        {
            Instantiate(ball, m_SpawnTransform.position, m_SpawnTransform.rotation);
        }
        yield return null;
    }
    
}