using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingZone : MonoBehaviour
{
    public float materialsQuantity;
    //public GameObject columns1;
    //public GameObject columns2;
    //public GameObject roof;

    [SerializeField] GameObject[] _columns;
    [SerializeField] GameObject[] _walls;
    [SerializeField] GameObject _entrance;
    [SerializeField] GameObject _roof;

    [SerializeField] Text _materialsText;

    public void AddMaterials(float quantity)
    {
        materialsQuantity += quantity;

        //if (materialsQuantity > 250)
        //{
        //    columns1.SetActive(true);
        //}
        //else if (materialsQuantity > 500)
        //{
        //    columns2.SetActive(true);
        //}
        //else if (materialsQuantity > 1000)
        //{
        //    roof.SetActive(true);
        //}

        if(materialsQuantity >= 50)
        {
            _columns[0].SetActive(true);
        }
        else if (materialsQuantity >= 100)
        {
            _walls[0].SetActive(true);
        }
        else if (materialsQuantity >= 150)
        {
            _columns[1].SetActive(true);
        }
        else if (materialsQuantity >= 200)
        {
            _walls[1].SetActive(true);
        }
        else if (materialsQuantity >= 250)
        {
            _columns[2].SetActive(true);
        }
        else if (materialsQuantity >= 300)
        {
            _walls[2].SetActive(true);
        }
        else if (materialsQuantity >= 350)
        {
            _columns[3].SetActive(true);
        }
        else if (materialsQuantity >= 400)
        {
            _entrance.SetActive(true);
        }
        else if (materialsQuantity >= 500)
        {
            _roof.SetActive(true);
        }

        _materialsText.text = materialsQuantity.ToString();
    }

    public void TakeMaterials(float quantity)
    {
        materialsQuantity -= quantity;

        _materialsText.text = materialsQuantity.ToString();
    }
}
