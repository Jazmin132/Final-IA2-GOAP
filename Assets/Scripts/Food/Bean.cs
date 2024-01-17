using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bean : Legumes
{
    //public int foodValue;

    void Start()
    {
        AddThisFood();

        GameManager.instance.AddBean(this);

        AddThisLegume();

        UIManager.instance.UpdateFoodValue();
    }

    public void OnDeathBean()
    {
        GameManager.instance.RemoveBean(this);
    }
}
