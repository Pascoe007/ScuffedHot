using UnityEngine;
using System.Collections;

public class Units : MonoBehaviour
{
    //public CharacterController controller;
    Rigidbody rb;
    FieldOfView fow;
    public Transform target;
    public float speed = 1;
    Vector3[] path;
    Vector3 velocity;
    int targetIndex;
    public float repeatRate;
    public bool hasWeapon = false;
    bool pathDone;
    public float StartTime;


    void Start()
    {
        fow = GetComponent<FieldOfView>();
        rb = GetComponent<Rigidbody>();
        InvokeRepeating("RunPath", 0.5f, repeatRate);
        StartTime = Time.time;

    }
    void tUpdate()
    {
        //velocity.y += -9.81f * Time.deltaTime;
        //controller.Move(velocity * Time.deltaTime);
        //if (controller.isGrounded)
        {
            //Debug.Log("Grounded");
        }
    }
    void RunPath()
    {
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
        
        Test(currentPos, minDist, tMin);
        
    }

    void Test(Vector3 currentPos, float minDist, Transform tMin)
    {
        float playerDist = Vector3.Distance(target.position, currentPos);
        
        if (minDist < playerDist && !hasWeapon)
        {
            
            if (pathDone)
            {
                
                hasWeapon = true;
                return;
            }
            PathReqManager.RequestPath(transform.position, tMin.position, OnPathFound);
        }
        else if (hasWeapon)
        {
            if (playerDist <= 11)
            {
                Vector3 playerAngle = (target.position - transform.position).normalized;
                
                if (!Physics.Raycast(transform.position, playerAngle, fow.dstToPlayer, fow.wallMask))
                {
                    StopCoroutine("FollowPath");
                }
                
            }
            else if (playerDist > 11)
            {
                PathReqManager.RequestPath(transform.position, target.position, OnPathFound);
                return;
            }
            else
            {
                PathReqManager.RequestPath(transform.position, target.position, OnPathFound);
            }
        }
        else
        {
            PathReqManager.RequestPath(transform.position, target.position, OnPathFound);
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
            Debug.Log("dst" + dst);
            if (currentWaypoint == transform.position)
            {
                
                pathDone = true;
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            //transform.position = Vector2.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            //controller.Move(Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime));

            

            rb.MovePosition(Vector3.Lerp(transform.position, currentWaypoint, ((Time.deltaTime - StartTime)*speed)/Vector3.Distance(transform.position, currentWaypoint)));

            
            //transform.LookAt(currentWaypoint);


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
}