using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coconut : Fruits
{
    //public int foodValue;

    void Start()
    {
        GameManager.instance.AddCoconut(this);
    }

    public void OnDeathCoconut()
    {
        GameManager.instance.RemoveCoconut(this);
    }
}
