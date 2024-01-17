using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bean : Legumes
{
    //public int foodValue;

    void Start()
    {
        GameManager.instance.AddBean(this);
    }

    public void OnDeathBean()
    {
        GameManager.instance.RemoveBean(this);
    }
}
