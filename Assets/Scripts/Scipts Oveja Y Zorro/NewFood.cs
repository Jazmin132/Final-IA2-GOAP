using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewFood : MonoBehaviour
{
    public float foodValue;

    void Start()
    {
        if (gameObject.GetComponent<Apple>())
        {
            GameManager.instance.AllFruits++;
            GameManager.instance.AllApples++;
        }
        else if (gameObject.GetComponent<Coconut>())
        {
            GameManager.instance.AllFruits++;
            GameManager.instance.AllCoconuts++;
        }
        else if (gameObject.GetComponent<Bean>())
        {
            GameManager.instance.AllLegumes++;
            GameManager.instance.AllBeans++;
        }

        GameManager.instance.AddFood(this);
    }

    public void OnDeath()
    {
        if (gameObject.GetComponent<Apple>())
        {
            GameManager.instance.AllFruits--;
            GameManager.instance.AllApples--;
        }
        else if (gameObject.GetComponent<Coconut>())
        {
            GameManager.instance.AllFruits--;
            GameManager.instance.AllCoconuts--;
        }
        else if (gameObject.GetComponent<Bean>())
        {
            GameManager.instance.AllLegumes--;
            GameManager.instance.AllBeans--;
        }

        GameManager.instance.RemoveFood(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var chef = collision.gameObject.GetComponent<Chef>();

        if (chef != null && collision.gameObject.layer == 14)
        {
            //Debug.Log("Chef has collided");

            if (chef.appleQuantity.Count + chef.coconutQuantity.Count + chef.beanQuantity.Count < chef.maxQuantityFoodCarried)
            {
                chef.finalDest = Vector3.zero;

                if (gameObject.GetComponent<Apple>())
                {
                    chef.appleQuantity.Add(gameObject.GetComponent<Apple>());
                }
                else if (gameObject.GetComponent<Coconut>())
                {
                    chef.coconutQuantity.Add(gameObject.GetComponent<Coconut>());
                }
                else if (gameObject.GetComponent<Bean>())
                {
                    chef.beanQuantity.Add(gameObject.GetComponent<Bean>());
                }

                OnDeath();

                //Destroy(gameObject);

                gameObject.SetActive(false);
            }
        }
    }
}
