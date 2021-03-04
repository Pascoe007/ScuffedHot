using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask weaponMask;
    public LayerMask wallMask;
    public LayerMask playerMask;

    public List<Transform> visibleWeapons = new List<Transform>();
    public List<Transform> Player = new List<Transform>();

    public float dstToPlayer;

    private void Start()
    {
        StartCoroutine("FindTargetsWithDelay", 0.2);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisiableWeapons();
            FindPlayer();
        }
    }
    public void FindVisiableWeapons()
    {
        visibleWeapons.Clear();
        Collider[] weaponsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, weaponMask);
        for (int i = 0; i < weaponsInViewRadius.Length; i++)
        {
            Transform weapon = weaponsInViewRadius[i].transform;
            Vector3 dirToWeapon = (weapon.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToWeapon) < viewAngle / 2)
            {
                float dstToWeapon = Vector3.Distance(transform.position, weapon.position);

                if (!Physics.Raycast(transform.position, dirToWeapon, dstToWeapon, wallMask))
                {
                    visibleWeapons.Add(weapon);
                }
            }
        }        
    }

    public void FindPlayer()
    {
        Collider[] player = Physics.OverlapSphere(transform.position, viewRadius, playerMask);
        for (int i = 0; i < player.Length; i++)
        {
            Transform _player = player[i].transform;
            Vector3 dirToPlayer = (_player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                dstToPlayer = Vector3.Distance(transform.position, _player.position);

                if(!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, wallMask))
                {
                    Player.Add(_player);
                    
                }

            }
        }
    }

    public Vector3 DirFromAngle(float angleinDegress, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleinDegress += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleinDegress * Mathf.Deg2Rad), 0, Mathf.Cos(angleinDegress * Mathf.Deg2Rad));
    }
}
