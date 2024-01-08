using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodPatch : MonoBehaviour
{
    public float foodQuantity;

    //public List<NewFood> foodListToAdd = new List<NewFood>();
    public NewFood food;

    public List<Apple> appleListQuantityFP = new List<Apple>();
    public List<Coconut> coconutListQuantityFP = new List<Coconut>();
    public List<Bean> beanListQuantityFP = new List<Bean>();

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void AddFood(int foodNum, NewFood foodToAdd)
    {
        food = foodToAdd;

        TransferFood(foodNum, food);
    }

    public void TransferFood(int foodNum, NewFood foodToAdd)
    {


        switch (foodNum)
        {
            case 0:
                
                break;
            case 1:

                break;
            case 2:

                break;
            default:
                Debug.Log("ERROR, el número " + foodNum + " no tiene un tipo food.");
                break;
        }
    }
}
