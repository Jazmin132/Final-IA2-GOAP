using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : GridEntity
{
    private void Start()
    {
        GameManager.instance.ChangeFoodPos(this);
    }
    public void OnDeath()
    {
        Debug.Log("Teletrasportar");
        GameManager.instance.ChangeFoodPos(this);
    }
}
