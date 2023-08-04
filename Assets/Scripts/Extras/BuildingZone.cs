using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingZone : MonoBehaviour
{
    public float materialsQuantity;
    public GameObject columns1;
    public GameObject columns2;
    public GameObject roof;

    public void AddMaterials(float quantity)
    {
        materialsQuantity += quantity;
        if (materialsQuantity > 500)
        {
            columns1.SetActive(true);
        }
        if (materialsQuantity > 1000)
        {
            columns2.SetActive(true);
        }
        if (materialsQuantity > 2000)
        {
            roof.SetActive(true);
        }
    }

    public void TakeMaterials(float quantity)
    {
        materialsQuantity -= quantity;
    }
}
