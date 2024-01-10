using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewFood : MonoBehaviour
{
    public int foodValue;

    void Start()
    {
        GameManager.instance.AddFood(this);
    }

    public void OnDeath()
    {
        GameManager.instance.RemoveFood(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 14)
        {
            Debug.Log("Chef has collided");
    
            OnDeath();
    
            Destroy(gameObject);
        }
    }
}
