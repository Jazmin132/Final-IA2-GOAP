using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Canteen : MonoBehaviour
{
    public float foodQuantity;
    public GameObject foodView;

    public List<Apple> appleListQuantity = new List<Apple>();
    public List<Coconut> coconutListQuantity = new List<Coconut>();
    public List<Bean> beanListQuantity = new List<Bean>();

    public IEnumerable<Apple> appleIEnumerableQuantity;
    public IEnumerable<Coconut> coconutIEnumerableQuantity;
    public IEnumerable<Bean> beanIEnumerableQuantity;

    public int maxQuantityOfFood, maxTypesOfFood;

    int _randomNumFood;

    //public List<NewFood> listNewFoodQuantity = new List<NewFood>();
    //
    //public IEnumerable<NewFood> iEnumerableNewFoodQuantity;

    void Start()
    {
        FoodIngredients();
    }

    public void AddFood(float quantity)
    {
        if (foodQuantity > 0)
        {
            foodView.SetActive(true);
        }
        foodQuantity += quantity;
    }

    public void TakeFood(float quantity)
    {
        if (foodQuantity >= 0)
        {
            foodView.SetActive(false);
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
                    //Debug.Log("appleListQuantity Capacity+");
                    appleListQuantity.Add(null);
                    break;
                case 1:
                    //Debug.Log("coconutListQuantity Capacity+");
                    coconutListQuantity.Add(null);
                    break;
                case 2:
                    //Debug.Log("beanListQuantity Capacity+");
                    beanListQuantity.Add(null);
                    break;
                default:
                    //Debug.Log("ERROR, no se encuentra una comida de el número " + _randomNumFood);
                    break;
            }

            //Debug.Log(i);
        }
    }

    public void TransferFoodToCanteen(List<Apple> chefAppleList, List<Coconut> chefCoconutList, List<Bean> chefBeanList)
    {
        if (chefAppleList.Count > 0)
        {
            //appleListQuantityFP.Concat(chefAppleList);

            appleListQuantity.Clear();

            for (int i = 0; i < chefAppleList.Count; i++)
            {
                appleListQuantity.Add(chefAppleList[i]);
            }
        }

        if (chefCoconutList.Count > 0)
        {
            //coconutListQuantityFP.Concat(chefCoconutList);

            coconutListQuantity.Clear();

            for (int i = 0; i < chefCoconutList.Count; i++)
            {
                coconutListQuantity.Add(chefCoconutList[i]);
            }
        }

        if (chefBeanList.Count > 0)
        {
            //beanListQuantityFP.Concat(chefBeanList);

            beanListQuantity.Clear();

            for (int i = 0; i < chefBeanList.Count; i++)
            {
                beanListQuantity.Add(chefBeanList[i]);
            }
        }

        CalculateFood();
    }

    public void CalculateFood() //IA2-LINQ
    {
        //Tema Concat + Aggregate para sumar el valor de food de cada tipo de comida

        appleIEnumerableQuantity = appleListQuantity;
        coconutIEnumerableQuantity = coconutListQuantity;
        beanIEnumerableQuantity = beanListQuantity;

        //var listFood = FList.Create(iEnumerableNewFoodQuantity) + appleIEnumerableQuantity + coconutIEnumerableQuantity + beanIEnumerableQuantity;

        //foodQuantity = (FList.Create(iEnumerableNewFoodQuantity) + appleIEnumerableQuantity + coconutIEnumerableQuantity + beanIEnumerableQuantity).OfType<NewFood>().Aggregate);

        //foodQuantity = (FList.Create(iEnumerableNewFoodQuantity) + appleIEnumerableQuantity + coconutIEnumerableQuantity + beanIEnumerableQuantity).OfType<NewFood>().Select(x => x.foodValue).Aggregate(0f, (x, y) => x + y);

        //NewFoodValue = (FList.Create<NewFood>() + appleIEnumQuantity + coconutIEnumQuantity + beanIEnumQuantity).OfType<NewFood>().Select(x => x.foodValue).Aggregate(0f, (x, y) => x + y);

        foodQuantity += (FList.Create<NewFood>() + appleIEnumerableQuantity + coconutIEnumerableQuantity + beanIEnumerableQuantity).Select(x => x.foodValue).Aggregate(0f, (x, y) => x + y);

        //var listFood = FList.Create(listNewFoodQuantity) + appleListQuantity + coconutListQuantity + beanListQuantity;

        appleListQuantity.Clear();
        coconutListQuantity.Clear();
        beanListQuantity.Clear();

        //listNewFoodQuantity.Clear();

        FoodIngredients();
    }
}
