using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Health : MonoBehaviour
{
    public int health;
    public int numOfHearts;

    public Image[] Hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private void Update()
    {
        if (health > numOfHearts)
        {
            health = numOfHearts;
        }
        for (int i = 0; i < Hearts.Length; i++)
        {
            if (i< health)
            {
                Hearts[i].sprite = fullHeart;
            }
            else
            {
                Hearts[i].sprite = emptyHeart;
            }
            if (i<numOfHearts)
            {
                Hearts[i].enabled = true;
            }
            else
            {
                Hearts[i].enabled = false;
            }
        }
    }
    public void Damage()
    {
        health--;
    }

}
