using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coconut : Fruits
{
    //public int foodValue;

    void Start()
    {
        AddThisFood();

        GameManager.instance.AddCoconut(this);

        AddThisFruit();

        UIManager.instance.UpdateFoodValue();
    }

    public void OnDeathCoconut()
    {
        GameManager.instance.RemoveCoconut(this);
    }
}
