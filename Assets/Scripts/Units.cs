using UnityEngine;
using System.Collections;

public class Units : MonoBehaviour
{
    //public CharacterController controller;
    Rigidbody rb;
    FieldOfView fow;
    Grid grid;
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
        grid = GetComponent<Grid>();
        fow = GetComponent<FieldOfView>();
        rb = GetComponent<Rigidbody>();
        InvokeRepeating("RunPath", 0.5f, repeatRate);
        StartTime = Time.time;
        

    }
    void tUpdate()
    {
        velocity.y += -9.81f * Time.deltaTime;
        rb.MovePosition(velocity * Time.deltaTime);
        
        
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
        
        Debug.Log(tMin);
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
                    if(Vector3.Distance(transform.position, tMin.transform.position) < 5 && !hasWeapon)
                    {
                        hasWeapon = true;
                        break;
                    }
                }
                
                
                break;
            }
            while (hasWeapon)
            {
                while (Vector3.Distance(transform.position, target.transform.position) < 10)
                {
                    StopCoroutine("FollowPath");
                    break;
                }
                if(Vector3.Distance(transform.position, target.transform.position) > 10)
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
                Debug.Log("hello");
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
}