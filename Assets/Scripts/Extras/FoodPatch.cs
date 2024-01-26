using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FoodPatch : MonoBehaviour
{
    //public float foodQuantity;

    //public List<NewFood> foodListToAdd = new List<NewFood>();
    //public NewFood food;

    public List<Apple> appleListQuantityFP = new List<Apple>();
    public List<Coconut> coconutListQuantityFP = new List<Coconut>();
    public List<Bean> beanListQuantityFP = new List<Bean>();

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

            for (int i = 0; i < chefAppleList.Count; i++)
            {
                appleListQuantityFP.Add(chefAppleList[i]);
            }
        }

        if(chefCoconutList.Count > 0)
        {
            //coconutListQuantityFP.Concat(chefCoconutList);

            for (int i = 0; i < chefCoconutList.Count; i++)
            {
                coconutListQuantityFP.Add(chefCoconutList[i]);
            }
        }

        if(chefBeanList.Count > 0)
        {
            //beanListQuantityFP.Concat(chefBeanList);

            for (int i = 0; i < chefBeanList.Count; i++)
            {
                beanListQuantityFP.Add(chefBeanList[i]);
            }
        }

        //Debug.Log("Termina TransferFoodToFoodPatch");
    }

    //public void TransferFoodToChef()
    //{
    //
    //}
}
