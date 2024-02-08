using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    public float woodQuantity;

    //public GameObject wood1;
    //public GameObject wood2;

    [SerializeField] GameObject[] _woods;

    [SerializeField] Text _woodText;

    public void AddWood(float quantity)
    {
        woodQuantity += quantity;
        //if (woodQuantity >= 100)
        //{
        //    wood1.SetActive(true);
        //}
        //
        //if(woodQuantity >= 200)
        //{
        //    wood2.SetActive(true);
        //}

        #region woodQuantity_If_True

        if (woodQuantity >= 100)
        {
            _woods[0].SetActive(true);
        }

        if (woodQuantity >= 200)
        {
            _woods[1].SetActive(true);
        }

        if (woodQuantity >= 300)
        {
            _woods[2].SetActive(true);
        }

        if (woodQuantity >= 400)
        {
            _woods[3].SetActive(true);
        }

        if (woodQuantity >= 500)
        {
            _woods[4].SetActive(true);
        }

        if (woodQuantity >= 600)
        {
            _woods[5].SetActive(true);
        }

        if (woodQuantity >= 700)
        {
            _woods[6].SetActive(true);
        }

        if (woodQuantity >= 800)
        {
            _woods[7].SetActive(true);
        }

        if (woodQuantity >= 900)
        {
            _woods[8].SetActive(true);
        }

        if (woodQuantity >= 1000)
        {
            _woods[9].SetActive(true);
        }

        if (woodQuantity >= 1100)
        {
            _woods[10].SetActive(true);
        }

        if (woodQuantity >= 1200)
        {
            _woods[11].SetActive(true);
        }

        if (woodQuantity >= 1300)
        {
            _woods[12].SetActive(true);
        }

        if (woodQuantity >= 1400)
        {
            _woods[13].SetActive(true);
        }

        if (woodQuantity >= 1500)
        {
            _woods[14].SetActive(true);
        }

        if (woodQuantity >= 1600)
        {
            _woods[15].SetActive(true);
        }

        if (woodQuantity >= 1700)
        {
            _woods[16].SetActive(true);
        }

        if (woodQuantity >= 1800)
        {
            _woods[17].SetActive(true);
        }

        if (woodQuantity >= 1900)
        {
            _woods[18].SetActive(true);
        }

        if (woodQuantity >= 2000)
        {
            _woods[19].SetActive(true);
        }

        #endregion

        _woodText.text = woodQuantity.ToString();
    }

    public void TakeWood(float quantity)
    {
        woodQuantity -= quantity;

        //if (woodQuantity <= 100)
        //{
        //    wood1.SetActive(false);
        //}
        //
        //if (woodQuantity <= 200)
        //{
        //    wood2.SetActive(false);
        //}

        #region woodQuantity_If_False

        if (woodQuantity < 100)
        {
            _woods[0].SetActive(false);
        }

        if (woodQuantity < 200)
        {
            _woods[1].SetActive(false);
        }

        if (woodQuantity < 300)
        {
            _woods[2].SetActive(false);
        }

        if (woodQuantity < 400)
        {
            _woods[3].SetActive(false);
        }

        if (woodQuantity < 500)
        {
            _woods[4].SetActive(false);
        }

        if (woodQuantity < 600)
        {
            _woods[5].SetActive(false);
        }

        if (woodQuantity < 700)
        {
            _woods[6].SetActive(false);
        }

        if (woodQuantity < 800)
        {
            _woods[7].SetActive(false);
        }

        if (woodQuantity < 900)
        {
            _woods[8].SetActive(false);
        }

        if (woodQuantity < 1000)
        {
            _woods[9].SetActive(false);
        }

        if (woodQuantity < 1100)
        {
            _woods[10].SetActive(false);
        }

        if (woodQuantity < 1200)
        {
            _woods[11].SetActive(false);
        }

        if (woodQuantity < 1300)
        {
            _woods[12].SetActive(false);
        }

        if (woodQuantity < 1400)
        {
            _woods[13].SetActive(false);
        }

        if (woodQuantity < 1500)
        {
            _woods[14].SetActive(false);
        }

        if (woodQuantity < 1600)
        {
            _woods[15].SetActive(false);
        }

        if (woodQuantity < 1700)
        {
            _woods[16].SetActive(false);
        }

        if (woodQuantity < 1800)
        {
            _woods[17].SetActive(false);
        }

        if (woodQuantity < 1900)
        {
            _woods[18].SetActive(false);
        }

        if (woodQuantity < 2000)
        {
            _woods[19].SetActive(false);
        }

        #endregion

        _woodText.text = woodQuantity.ToString();
    }
}
