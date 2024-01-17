using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewFood : MonoBehaviour
{
    public float foodValue;

    //void Start()
    //{
    //    GameManager.instance.AddFood(this);
    //}

    public void AddThisFood()
    {
        GameManager.instance.AddFood(this);
    }

    public void OnDeath()
    {
        GameManager.instance.RemoveFood(this);

        if (gameObject.GetComponent<Fruits>())
        {
            gameObject.GetComponent<Fruits>().OnDeathFruit();

            if (gameObject.GetComponent<Apple>())
            {
                gameObject.GetComponent<Apple>().OnDeathApple();
            }
            else if (gameObject.GetComponent<Coconut>())
            {
                gameObject.GetComponent<Coconut>().OnDeathCoconut();
            }
        }
        else if (gameObject.GetComponent<Legumes>())
        {
            gameObject.GetComponent<Legumes>().OnDeathLegume();

            if (gameObject.GetComponent<Bean>())
            {
                gameObject.GetComponent<Bean>().OnDeathBean();
            }
        }

        UIManager.instance.UpdateFoodValue();
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
