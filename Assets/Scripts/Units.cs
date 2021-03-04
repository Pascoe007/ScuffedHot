using UnityEngine;
using System.Collections;

public class Units : MonoBehaviour
{
    FieldOfView fow;
    public Transform target;
    float speed = 10;
    Vector3[] path;
    int targetIndex;
    public float repeatRate;
    public bool hasWeapon = false;
    bool pathDone;


    void Start()
    {
        fow = GetComponent<FieldOfView>();
        InvokeRepeating("RunPath", 0.5f, repeatRate);

    }
    void RunPath()
    {
        fow.FindVisiableWeapons();
        Debug.Log(hasWeapon);
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
        Debug.Log("PlayerDis " + playerDist);
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
                Debug.Log("Player Angle " + playerAngle);
                if (!Physics.Raycast(transform.position, playerAngle, fow.dstToPlayer, fow.wallMask))
                {
                    Debug.Log("no move");
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
            if (transform.position == currentWaypoint)
            {
                pathDone = true;
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
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