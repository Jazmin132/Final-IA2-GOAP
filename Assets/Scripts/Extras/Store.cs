using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    public float woodQuantity;

    public GameObject wood1;
    public GameObject wood2;

    [SerializeField] Text _woodText;

    public void AddWood(float quantity)
    {
        woodQuantity += quantity;
        if (woodQuantity >= 100)
        {
            wood1.SetActive(true);
        }

        if(woodQuantity >= 200)
        {
            wood2.SetActive(true);
        }

        _woodText.text = woodQuantity.ToString();
    }

    public void TakeWood(float quantity)
    {
        woodQuantity -= quantity;

        if (woodQuantity <= 100)
        {
            wood1.SetActive(false);
        }

        if (woodQuantity <= 200)
        {
            wood2.SetActive(false);
        }

        _woodText.text = woodQuantity.ToString();
    }
}
