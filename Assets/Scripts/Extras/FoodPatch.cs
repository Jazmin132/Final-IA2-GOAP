using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class FoodPatch : MonoBehaviour
{
    //public float foodQuantity;

    //public List<NewFood> foodListToAdd = new List<NewFood>();
    //public NewFood food;

    public FoodPatchFood prefabDesireFPF;
    int HMApples;
    int HMBeans;
    int HMCoconuts;

    public List<Apple> appleListQuantityFP = new List<Apple>();
    public List<Coconut> coconutListQuantityFP = new List<Coconut>();
    public List<Bean> beanListQuantityFP = new List<Bean>();

    [SerializeField] GameObject[] _applesView, _coconutsView, _beansView;
    [SerializeField] int _numToView1, _numToView2, _numToView3;

    [SerializeField] GeneratorsScript _generatorsScript;

    //public void AddFood(int foodNum, NewFood foodToAdd)
    //{
    //    food = foodToAdd;
    //
    //    TransferFood(foodNum, food);
    //}

    public void TransferFoodToFoodPatch(List<Apple> chefAppleList, List<Coconut> chefCoconutList, List<Bean> chefBeanList)
    {
        if(chefAppleList.Count > 0)
        {
            //appleListQuantityFP.Concat(chefAppleList);

            //for (int i = 0; i < chefAppleList.Count; i++)
            //{
            //    appleListQuantityFP.Add(chefAppleList[i]);
            //}

            //appleListQuantityFP = _generatorsScript.IEnumerableCollectionCreator(chefAppleList).ToList();

            appleListQuantityFP.AddRange(_generatorsScript.IEnumerableCollectionCreator(chefAppleList)); //IA2-LINQ
        }

        if(chefCoconutList.Count > 0)
        {
            //coconutListQuantityFP.Concat(chefCoconutList);

            //for (int i = 0; i < chefCoconutList.Count; i++)
            //{
            //    coconutListQuantityFP.Add(chefCoconutList[i]);
            //}

            //coconutListQuantityFP = _generatorsScript.IEnumerableCollectionCreator(chefCoconutList).ToList();

            coconutListQuantityFP.AddRange(_generatorsScript.IEnumerableCollectionCreator(chefCoconutList)); //IA2-LINQ
        }

        if(chefBeanList.Count > 0)
        {
            //beanListQuantityFP.Concat(chefBeanList);

            //for (int i = 0; i < chefBeanList.Count; i++)
            //{
            //    beanListQuantityFP.Add(chefBeanList[i]);
            //}

            //beanListQuantityFP = _generatorsScript.IEnumerableCollectionCreator(chefBeanList).ToList();

            beanListQuantityFP.AddRange(_generatorsScript.IEnumerableCollectionCreator(chefBeanList)); //IA2-LINQ
        }
        //Debug.Log("Termina TransferFoodToFoodPatch");

        FoodPatchCountUI();

        FoodPatchViewFood();
    }

    public void FoodPatchCountUI()
    {
        HMApples = appleListQuantityFP.Count();

        HMCoconuts = coconutListQuantityFP.Count();

        HMBeans = beanListQuantityFP.Count();

        prefabDesireFPF.HowMuchFoodINeedFPF(HMApples, HMBeans, HMCoconuts);
    }

    public void FoodPatchViewFood()
    {
        #region ApplesView
        if (appleListQuantityFP.Count >= _numToView1)
        {
            if (!_applesView[0].activeSelf)
                _applesView[0].SetActive(true);
        }
        else
        {
            if (_applesView[0].activeSelf)
                _applesView[0].SetActive(false);
        }

        if (appleListQuantityFP.Count >= _numToView2)
        {
            if (!_applesView[1].activeSelf)
                _applesView[1].SetActive(true);
        }
        else
        {
            if (_applesView[1].activeSelf)
                _applesView[1].SetActive(false);
        }

        if (appleListQuantityFP.Count >= _numToView3)
        {
            if (!_applesView[2].activeSelf)
                _applesView[2].SetActive(true);
        }
        else
        {
            if (_applesView[2].activeSelf)
                _applesView[2].SetActive(false);
        }
        #endregion

        #region CoconutsView
        if (coconutListQuantityFP.Count >= _numToView1)
        {
            if (!_coconutsView[0].activeSelf)
                _coconutsView[0].SetActive(true);
        }
        else
        {
            if (_coconutsView[0].activeSelf)
                _coconutsView[0].SetActive(false);
        }

        if (coconutListQuantityFP.Count >= _numToView2)
        {
            if (!_coconutsView[1].activeSelf)
                _coconutsView[1].SetActive(true);
        }
        else
        {
            if (_coconutsView[1].activeSelf)
                _coconutsView[1].SetActive(false);
        }

        if (coconutListQuantityFP.Count >= _numToView3)
        {
            if (!_coconutsView[2].activeSelf)
                _coconutsView[2].SetActive(true);
        }
        else
        {
            if (_coconutsView[2].activeSelf)
                _coconutsView[2].SetActive(false);
        }
        #endregion

        #region BeansView
        if (beanListQuantityFP.Count >= _numToView1)
        {
            if (!_beansView[0].activeSelf)
                _beansView[0].SetActive(true);
        }
        else
        {
            if (_beansView[0].activeSelf)
                _beansView[0].SetActive(false);
        }

        if (beanListQuantityFP.Count >= _numToView2)
        {
            if (!_beansView[1].activeSelf)
                _beansView[1].SetActive(true);
        }
        else
        {
            if (_beansView[1].activeSelf)
                _beansView[1].SetActive(false);
        }

        if (beanListQuantityFP.Count >= _numToView3)
        {
            if (!_beansView[2].activeSelf)
                _beansView[2].SetActive(true);
        }
        else
        {
            if (_beansView[2].activeSelf)
                _beansView[2].SetActive(false);
        }
        #endregion
    }
}
