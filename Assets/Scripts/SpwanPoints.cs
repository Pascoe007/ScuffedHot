using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanPoints : MonoBehaviour
{
    public GameObject Enmey;
    public int xPos;
    public int zPos;
    public int enmeyCount;

    void Start()
    {
        StartCoroutine(EmemySpwan());
    }
    public void DecreasedCount()
    {
        enmeyCount--;
    }
    IEnumerator EmemySpwan()
    {
        while(enmeyCount< 10)
        {
            xPos = Random.Range(-50, 50);
            zPos = Random.Range(-50, 50);
            Instantiate(Enmey, new Vector3(xPos, 0, zPos), Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            enmeyCount += 1;
        }
    }
}
