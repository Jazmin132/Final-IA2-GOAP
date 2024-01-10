using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewFood : MonoBehaviour
{
    public int foodValue;

    public int foodNum;

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
        var chef = collision.gameObject.GetComponent<Chef>();

        if (chef != null && collision.gameObject.layer == 14)
        {
            Debug.Log("Chef has collided");

            chef.finalDest = Vector3.zero;

            if (gameObject.GetComponent<Apple>())
            {

            }
            else if (gameObject.GetComponent<Coconut>())
            {

            }
            else if (gameObject.GetComponent<Bean>())
            {

            }
    
            OnDeath();
    
            Destroy(gameObject);
        }
    }
}
