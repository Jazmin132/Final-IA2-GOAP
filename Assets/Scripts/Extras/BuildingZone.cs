using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingZone : MonoBehaviour
{
    public float materialsQuantity;

    public void AddMaterials(float quantity)
    {
        materialsQuantity += quantity;
    }

    public void TakeMaterials(float quantity)
    {
        materialsQuantity -= quantity;
    }
}
