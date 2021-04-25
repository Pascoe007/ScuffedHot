using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanPoints : MonoBehaviour
{
    public GameObject Enmey;
    public int xPos;
    public int zPos;
    public int enmeyCount = 0;

    private void Awake()
    {
        
    }
    public void LowerCount()
    {
        
    }
    private void Update()
    {
        while (enmeyCount < 5)
        {
            xPos = Random.Range(-50, 50);
            zPos = Random.Range(-50, 50);
            Instantiate(Enmey, new Vector3(xPos, 0, zPos), Quaternion.identity);
            enmeyCount += 1;
            Debug.Log(enmeyCount);
        }
        

    }
}
