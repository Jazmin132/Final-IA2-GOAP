using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Food : GridEntity
{
    private void Start()
    {
        GameManager.instance.ChangeFoodPos(this);
    }

    public void OnDeath()
    {
        GameManager.instance.ChangeFoodPos(this);
    }
}
