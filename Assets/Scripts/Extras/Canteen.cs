using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Canteen : MonoBehaviour
{
    public float foodQuantity;
    //public GameObject foodView;
    [SerializeField] bool _appleAsked, _coconutAsked, _beanAsked;
    [SerializeField] GameObject[] _applesView, _coconutsView, _beansView;

    [SerializeField] Chef _chef;

    [SerializeField] Constructor _constructor;

    public CanteenFood prefabDesireCF;
    int HMApples;
    int HMBeans;
    int HMCoconuts;

    public List<Apple> appleListQuantity = new List<Apple>();
    public List<Coconut> coconutListQuantity = new List<Coconut>();
    public List<Bean> beanListQuantity = new List<Bean>();

    public List<NewFood> listNewFoodQuantity = new List<NewFood>();

    //public IEnumerable<Apple> appleIEnumerableQuantity;
    //public IEnumerable<Coconut> coconutIEnumerableQuantity;
    //public IEnumerable<Bean> beanIEnumerableQuantity;

    public int maxQuantityOfFood, maxTypesOfFood;

    int _randomNumFood;

    //public IEnumerable<NewFood> iEnumerableNewFoodQuantity;

    [SerializeField] GeneratorsScript _generatorsScript;

    void Start()
    {
        FoodIngredients();
        //PrefabDesire.transform.position = transform.position;
    }

    void FixedUpdate()
    {
        if(_chef != null && !_chef.chefAboutToEat)
        {
            if(_applesView[0].activeSelf)
            {
                _applesView[0].SetActive(false);
            }

            if (_coconutsView[0].activeSelf)
            {
                _coconutsView[0].SetActive(false);
            }

            if (_beansView[0].activeSelf)
            {
                _beansView[0].SetActive(false);
            }
        }

        if(_constructor != null && !_constructor.constructorEating)
        {
            if (_applesView[1].activeSelf)
            {
                _applesView[1].SetActive(false);
            }

            if (_coconutsView[1].activeSelf)
            {
                _coconutsView[1].SetActive(false);
            }

            if (_beansView[1].activeSelf)
            {
                _beansView[1].SetActive(false);
            }
        }
    }

    public void AddFood(float quantity)
    {
        //if (foodQuantity > 0)
        //{
        //    foodView.SetActive(true);
        //}
        foodQuantity += quantity;
    }

    public void TakeFood(float quantity)
    {
        //if (foodQuantity >= 0)
        //{
        //    foodView.SetActive(false);
        //}
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
                    HMApples = appleListQuantity.Count();
                    if (!_appleAsked)
                        _appleAsked = true;
                    break;
                case 1:
                    //Debug.Log("coconutListQuantity Capacity+");
                    coconutListQuantity.Add(null);
                    HMCoconuts = coconutListQuantity.Count();
                    if (!_coconutAsked)
                        _coconutAsked = true;
                    break;
                case 2:
                    //Debug.Log("beanListQuantity Capacity+");
                    beanListQuantity.Add(null);
                    HMBeans = beanListQuantity.Count();
                    if (!_beanAsked)
                        _beanAsked = true;
                    break;
                default:
                    //Debug.Log("ERROR, no se encuentra una comida de el número " + _randomNumFood);
                    break;
            }
            //Debug.Log(i);
        }
        prefabDesireCF.HowMuchFoodINeedCF(HMApples, HMBeans, HMCoconuts);
    }

    public void TransferFoodToCanteen(List<Apple> chefAppleList, List<Coconut> chefCoconutList, List<Bean> chefBeanList)
    {
        
        if (chefAppleList.Count > 0)
        {
            //appleListQuantityFP.Concat(chefAppleList);

            appleListQuantity.Clear();

            //for (int i = 0; i < chefAppleList.Count; i++)
            //{
            //    appleListQuantity.Add(chefAppleList[i]);
            //}

            //appleListQuantity = _generatorsScript.IEnumerableCollectionCreator(chefAppleList).ToList();

            appleListQuantity.AddRange(_generatorsScript.IEnumerableCollectionCreator(chefAppleList)); //IA2-LINQ
        }

        if (chefCoconutList.Count > 0)
        {
            //coconutListQuantityFP.Concat(chefCoconutList);

            coconutListQuantity.Clear();

            //for (int i = 0; i < chefCoconutList.Count; i++)
            //{
            //    coconutListQuantity.Add(chefCoconutList[i]);
            //}

            //coconutListQuantity = _generatorsScript.IEnumerableCollectionCreator(chefCoconutList).ToList();

            coconutListQuantity.AddRange(_generatorsScript.IEnumerableCollectionCreator(chefCoconutList)); //IA2-LINQ
        }

        if (chefBeanList.Count > 0)
        {
            //beanListQuantityFP.Concat(chefBeanList);

            beanListQuantity.Clear();

            //for (int i = 0; i < chefBeanList.Count; i++)
            //{
            //    beanListQuantity.Add(chefBeanList[i]);
            //}

            //beanListQuantity = _generatorsScript.IEnumerableCollectionCreator(chefBeanList).ToList();

            beanListQuantity.AddRange(_generatorsScript.IEnumerableCollectionCreator(chefBeanList)); //IA2-LINQ
        }

        CalculateFood();
    }

    public void CalculateFood() //IA2-LINQ
    {
        //Tema Concat + Aggregate para sumar el valor de food de cada tipo de comida

        #region Code_Test_foodQuantity
        //appleIEnumerableQuantity = appleListQuantity;
        //coconutIEnumerableQuantity = coconutListQuantity;
        //beanIEnumerableQuantity = beanListQuantity;

        //var listFood = FList.Create(iEnumerableNewFoodQuantity) + appleIEnumerableQuantity + coconutIEnumerableQuantity + beanIEnumerableQuantity;

        //foodQuantity = (FList.Create(iEnumerableNewFoodQuantity) + appleIEnumerableQuantity + coconutIEnumerableQuantity + beanIEnumerableQuantity).OfType<NewFood>().Aggregate);

        //foodQuantity = (FList.Create(iEnumerableNewFoodQuantity) + appleIEnumerableQuantity + coconutIEnumerableQuantity + beanIEnumerableQuantity).OfType<NewFood>().Select(x => x.foodValue).Aggregate(0f, (x, y) => x + y);

        //NewFoodValue = (FList.Create<NewFood>() + appleIEnumQuantity + coconutIEnumQuantity + beanIEnumQuantity).OfType<NewFood>().Select(x => x.foodValue).Aggregate(0f, (x, y) => x + y);

        //foodQuantity += (FList.Create<NewFood>() + appleListQuantity + coconutListQuantity + beanListQuantity).Select(x => x.foodValue).Aggregate(0f, (x, y) => x + y);
        #endregion

        //foodQuantity += (FList.Create<NewFood>() + appleListQuantity + coconutListQuantity + beanListQuantity).Select(x => x.foodValue).Sum(); //FLIST

        foodQuantity += listNewFoodQuantity.Concat(appleListQuantity).Concat(coconutListQuantity).Concat(beanListQuantity).Select(x => x.foodValue).Sum(); //CONCAT

        if (appleListQuantity.Count > 0)
        {
            for (int i = 0; i < appleListQuantity.Count; i++)
            {
                Destroy(appleListQuantity[i].gameObject);
            }

            appleListQuantity.Clear();
            HMApples = appleListQuantity.Count();
        }

        if (coconutListQuantity.Count > 0)
        {
            for (int i = 0; i < coconutListQuantity.Count; i++)
            {
                Destroy(coconutListQuantity[i].gameObject);
            }

            coconutListQuantity.Clear();
            HMCoconuts = coconutListQuantity.Count();
        }

        if (beanListQuantity.Count > 0)
        {
            for (int i = 0; i < beanListQuantity.Count; i++)
            {
                Destroy(beanListQuantity[i].gameObject);
            }

            beanListQuantity.Clear();
            HMBeans = beanListQuantity.Count();
        }

        listNewFoodQuantity.Clear();

        #region FoodAsked
        if (_appleAsked)
        {
            if (!_applesView[0].activeSelf && !_applesView[1].activeSelf)
            {
                _applesView[0].SetActive(true);
                _applesView[1].SetActive(true);
            }

            _appleAsked = false;
        }

        if (_coconutAsked)
        {
            if (!_coconutsView[0].activeSelf && !_coconutsView[1].activeSelf)
            {
                _coconutsView[0].SetActive(true);
                _coconutsView[1].SetActive(true);
            }

            _coconutAsked = false;
        }

        if (_beanAsked)
        {
            if (!_beansView[0].activeSelf && !_beansView[1].activeSelf)
            {
                _beansView[0].SetActive(true);
                _beansView[1].SetActive(true);
            }

            _beanAsked = false;
        }
        #endregion

        FoodIngredients();
    }
}
