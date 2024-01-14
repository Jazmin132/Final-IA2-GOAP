using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Food : GridEntity
{
    private void Awake()
    {
        if(gameObject.transform.parent != GameManager.instance.gridObject.transform)
        {
            gameObject.transform.parent = GameManager.instance.gridObject.transform;
        }
    }

    private void Start()
    {
        GameManager.instance.ChangeFoodPos(this);
    }

    public void OnDeath()
    {
        GameManager.instance.ChangeFoodPos(this);
    }
}
