using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canteen : MonoBehaviour
{
    public float foodQuantity;
    public GameObject food;

    public List<Apple> appleQuantity = new List<Apple>();
    public List<Coconut> coconutQuantity = new List<Coconut>();
    public List<Bean> beanQuantity = new List<Bean>();

    public int maxQuantityOfFood, maxTypesOfFood, randomNumFood;

    void Start()
    {
        for (int i = 0; i < maxQuantityOfFood; i++)
        {
            randomNumFood = Random.Range(0, maxTypesOfFood);

            switch (randomNumFood)
            {
                case 0:

                    break;
                case 1:

                    break;
                case 2:

                    break;
                default:

                    Debug.Log("ERROR, no se encuentra una comida de el número" + randomNumFood);

                    break;
            }
        }
    }

    public void AddFood(float quantity)
    {
        if (foodQuantity > 0)
        {
            food.SetActive(true);
        }
        foodQuantity += quantity;
    }

    public void TakeFood(float quantity)
    {
        if (foodQuantity >= 0)
        {
            food.SetActive(false);
        }
        foodQuantity -= quantity;
    }


}
