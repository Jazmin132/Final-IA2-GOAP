using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canteen : MonoBehaviour
{
    public float foodQuantity;

    public void AddFood(float quantity)
    {
        foodQuantity += quantity;
    }

    public void TakeFood(float quantity)
    {
        foodQuantity -= quantity;
    }
}
