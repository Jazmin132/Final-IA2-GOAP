using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canteen : MonoBehaviour
{
    public float foodQuantity;
    public GameObject food;

    public void AddFood(float quantity)
    {
        if (foodQuantity > 0)
        {
            food.SetActive(true);
        }
        foodQuantity += quantity;
    }

    public void TakeFood(float quantity)
    {
        if (foodQuantity >= 0)
        {
            food.SetActive(false);
        }
        foodQuantity -= quantity;
    }
}
