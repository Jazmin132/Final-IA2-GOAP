using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewFood : MonoBehaviour
{
    public int foodValue;

    void Start()
    {
        GameManager.instance.AddFood(this);
    }

    public void OnDeath()
    {
        GameManager.instance.RemoveFood(this);
    }
}
