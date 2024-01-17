using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : Fruits
{
    //public int foodValue;

    void Start()
    {
        GameManager.instance.AddApple(this);
    }
    
    public void OnDeathApple()
    {
        GameManager.instance.RemoveApple(this);
    }
}
