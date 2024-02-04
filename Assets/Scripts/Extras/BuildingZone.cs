using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingZone : MonoBehaviour
{
    public float materialsQuantity;
    public GameObject columns1;
    public GameObject columns2;
    public GameObject roof;

    [SerializeField] Text _materialsText;

    public void AddMaterials(float quantity)
    {
        materialsQuantity += quantity;
        if (materialsQuantity > 500)
        {
            columns1.SetActive(true);
        }
        else if (materialsQuantity > 1000)
        {
            columns2.SetActive(true);
        }
        else if (materialsQuantity > 2000)
        {
            roof.SetActive(true);
        }

        _materialsText.text = materialsQuantity.ToString();
    }

    public void TakeMaterials(float quantity)
    {
        materialsQuantity -= quantity;

        _materialsText.text = materialsQuantity.ToString();
    }
}
