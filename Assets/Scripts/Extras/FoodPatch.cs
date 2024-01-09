using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FoodPatch : MonoBehaviour
{
    public float foodQuantity;

    //public List<NewFood> foodListToAdd = new List<NewFood>();
    //public NewFood food;

    public List<Apple> appleListQuantityFP = new List<Apple>();
    public List<Coconut> coconutListQuantityFP = new List<Coconut>();
    public List<Bean> beanListQuantityFP = new List<Bean>();

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    //public void AddFood(int foodNum, NewFood foodToAdd)
    //{
    //    food = foodToAdd;
    //
    //    TransferFood(foodNum, food);
    //}

    public void TransferFood(List<Apple> chefAppleList, List<Coconut> chefCoconutList, List<Bean> chefBeanList)
    {
        if(chefAppleList.Count > 0)
        {
            appleListQuantityFP.Concat(chefAppleList);
        }

        if(chefCoconutList.Count > 0)
        {
            coconutListQuantityFP.Concat(chefCoconutList);
        }

        if(chefBeanList.Count > 0)
        {
            beanListQuantityFP.Concat(chefBeanList);
        }
    }
}
