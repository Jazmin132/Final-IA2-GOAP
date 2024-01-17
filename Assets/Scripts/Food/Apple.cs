using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : Fruits
{
    //public int foodValue;

    void Start()
    {
        AddThisFood();

        GameManager.instance.AddApple(this);

        AddThisFruit();

        UIManager.instance.UpdateFoodValue();
    }
    
    public void OnDeathApple()
    {
        GameManager.instance.RemoveApple(this);
    }
}
