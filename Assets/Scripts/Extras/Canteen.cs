using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canteen : MonoBehaviour
{
    public float foodQuantity;
    public GameObject food;

    public List<Apple> appleListQuantity = new List<Apple>();
    public List<Coconut> coconutListQuantity = new List<Coconut>();
    public List<Bean> beanListQuantity = new List<Bean>();

    public int maxQuantityOfFood, maxTypesOfFood;

    int _randomNumFood;

    void Start()
    {
        FoodIngredients();
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

    public void FoodIngredients()
    {
        for (int i = 0; i < maxQuantityOfFood; i++)
        {
            _randomNumFood = Random.Range(0, maxTypesOfFood);

            switch (_randomNumFood)
            {
                case 0:
                    Debug.Log("appleListQuantity Capacity+");
                    appleListQuantity.Add(null);
                    break;
                case 1:
                    Debug.Log("coconutListQuantity Capacity+");
                    coconutListQuantity.Add(null);
                    break;
                case 2:
                    Debug.Log("beanListQuantity Capacity+");
                    beanListQuantity.Add(null);
                    break;
                default:
                    Debug.Log("ERROR, no se encuentra una comida de el número " + _randomNumFood);
                    break;
            }

            Debug.Log(i);
        }
    }
}
