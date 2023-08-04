using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
    public float woodQuantity;

    public void AddWood(float quantity)
    {
        woodQuantity += quantity;
    }

    public void TakeWood(float quantity)
    {
        woodQuantity -= quantity;
    }
}
