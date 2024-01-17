using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : Fruits
{
    //public int foodValue;

    void Start()
    {
        GameManager.instance.AddApple(this);

        AddThisFruit();
    }
    
    public void OnDeathApple()
    {
        GameManager.instance.RemoveApple(this);
    }
}
