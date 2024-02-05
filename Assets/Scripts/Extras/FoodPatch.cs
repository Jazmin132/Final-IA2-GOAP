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

            appleListQuantityFP.AddRange(_generatorsScript.IEnumerableCollectionCreator(chefAppleList)); //IA-P2
        }

        if(chefCoconutList.Count > 0)
        {
            //coconutListQuantityFP.Concat(chefCoconutList);

            //for (int i = 0; i < chefCoconutList.Count; i++)
            //{
            //    coconutListQuantityFP.Add(chefCoconutList[i]);
            //}

            //coconutListQuantityFP = _generatorsScript.IEnumerableCollectionCreator(chefCoconutList).ToList();

            coconutListQuantityFP.AddRange(_generatorsScript.IEnumerableCollectionCreator(chefCoconutList)); //IA-P2
        }

        if(chefBeanList.Count > 0)
        {
            //beanListQuantityFP.Concat(chefBeanList);

            //for (int i = 0; i < chefBeanList.Count; i++)
            //{
            //    beanListQuantityFP.Add(chefBeanList[i]);
            //}

            //beanListQuantityFP = _generatorsScript.IEnumerableCollectionCreator(chefBeanList).ToList();

            beanListQuantityFP.AddRange(_generatorsScript.IEnumerableCollectionCreator(chefBeanList)); //IA-P2
        }
        //Debug.Log("Termina TransferFoodToFoodPatch");

        FoodPatchCountUI();
    }

    public void FoodPatchCountUI()
    {
        HMApples = appleListQuantityFP.Count();

        HMCoconuts = coconutListQuantityFP.Count();

        HMBeans = beanListQuantityFP.Count();

        prefabDesireFPF.HowMuchFoodINeedFPF(HMApples, HMBeans, HMCoconuts);
    }

}
