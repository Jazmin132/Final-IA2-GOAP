using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruits : NewFood
{
    //void Start()
    //{
    //    GameManager.instance.AddFruit(this);
    //}

    public void AddThisFruit()
    {
        GameManager.instance.AddFruit(this);
    }

    public void OnDeathFruit()
    {
        GameManager.instance.RemoveFruit(this);
    }
}
