using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legumes : NewFood
{
    //void Start()
    //{
    //    GameManager.instance.AddLegume(this);
    //}

    public void AddThisLegume()
    {
        GameManager.instance.AddLegume(this);
    }

    public void OnDeathLegume()
    {
        GameManager.instance.RemoveLegume(this);
    }
}
