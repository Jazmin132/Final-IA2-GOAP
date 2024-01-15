using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chef : MonoBehaviour
{
    [Header("NormalValues")] //Lo siguiente se puede tener normalmente pero usarlo de esta manera es más óptimo
    [SerializeField] Transform _myTransform;
    [SerializeField] Rigidbody _myRgbd;

    [Header("Values")]

    //[SerializeField] float _randomValueForAngle;
    //[SerializeField] Vector3 _newVector3Rotation;

    [Header("ImportantValues")]
    //[SerializeField] float _food;
    //[SerializeField] float _foodMaxCapacity, _foodToGain;
    [SerializeField] Canteen _canteenToLoad;

    [SerializeField] float _speed, _speedOriginal;

    [SerializeField] FoodPatch _foodPatch;

    [SerializeField] bool _canCountTimeFood, _restartFoodTime;
    [SerializeField] float _timeFood, _timerFood;

    [SerializeField] float _hunger;
    [SerializeField] float _hungerMaxCapacity, _hungerMinCapacity, _hungerToLose, _hungerToGain;
    [SerializeField] bool _canCountTimeHunger, _restartHungerTime;
    [SerializeField] float _timeHunger, _timerHunger;

    [SerializeField] bool _canCountTimeEatFood, _restartEatFoodTime;

    [SerializeField] bool _hungerActive, _isEating;

    [Header("State")] //Uso 'bool' en vez de 'estados', reemplazar los 'bool' por los estados que va a tener
    [SerializeField] bool _stateLookingForFood, _stateCollect, _stateLoadFood, _stateEat;

    public ParticleSystem particleCooking;
    public ParticleSystem particleHungry;
    public ParticleSystem Death;
    public ParticleSystem particleHarvest;

    [SerializeField] GameObject _particleDeathObject;

    //[SerializeField] NewFood[] _foodCollected;

    public List<Apple> appleQuantity = new List<Apple>();
    public List<Coconut> coconutQuantity = new List<Coconut>();
    public List<Bean> beanQuantity = new List<Bean>();

    [SerializeField] GameManager _gameManager;

    public Vector3 finalDest;

    public int maxQuantityFoodCarried;

    [SerializeField] Constructor _constructor;

    [SerializeField] bool _maxCapacityFoodReached, _foodForCanteenReady;

    #region ChefStates
    public enum ChefStates
    {
        LookingForFood,
        GoToTable,
        LoadFood,
        WaitForCompany,
        Eat,
        stateDeath
    }
    public EventFSM<ChefStates> _MyFSM;

    void Awake()
    {
        var _LookingForFood = new State<ChefStates>("LookingForFood");
        var _goToTable = new State<ChefStates>("GoToTable");
        var _LoadFood = new State<ChefStates>("LoadFood");
        var _WaitForCompany = new State<ChefStates>("WaitForCompany");
        var _Eat = new State<ChefStates>("Eat");
        var _stateDeath = new State<ChefStates>("stateDeath");

        StateConfigurer.Create(_LookingForFood)
            .SetTransition(ChefStates.GoToTable, _goToTable)
            .SetTransition(ChefStates.Eat, _Eat)
            .SetTransition(ChefStates.LoadFood, _LoadFood)
            .SetTransition(ChefStates.stateDeath, _stateDeath).Done();

        StateConfigurer.Create(_goToTable)
            .SetTransition(ChefStates.Eat, _Eat)
            .SetTransition(ChefStates.LoadFood, _LoadFood)
            .SetTransition(ChefStates.stateDeath, _stateDeath)
            .SetTransition(ChefStates.LookingForFood, _LookingForFood)
            .SetTransition(ChefStates.WaitForCompany, _WaitForCompany).Done();

        StateConfigurer.Create(_LoadFood)
            .SetTransition(ChefStates.Eat, _Eat)
            .SetTransition(ChefStates.GoToTable, _goToTable)
            .SetTransition(ChefStates.stateDeath, _stateDeath)
            .SetTransition(ChefStates.LookingForFood, _LookingForFood).Done();

        StateConfigurer.Create(_WaitForCompany)
            .SetTransition(ChefStates.Eat, _Eat)
            .SetTransition(ChefStates.LoadFood, _LoadFood)
            .SetTransition(ChefStates.stateDeath, _stateDeath).Done();

        StateConfigurer.Create(_Eat)
            .SetTransition(ChefStates.GoToTable, _goToTable)
            .SetTransition(ChefStates.LoadFood, _LoadFood)
            .SetTransition(ChefStates.stateDeath, _stateDeath)
            .SetTransition(ChefStates.LookingForFood, _LookingForFood).Done();

        StateConfigurer.Create(_stateDeath).Done();

        _LookingForFood.OnEnter += x => 
        { 
            _stateLoadFood = true;
        }; 
        _LookingForFood.OnFixedUpdate += () => 
        {
            //if (!_restartFoodTime)
            //    _restartFoodTime = true;
            //
            //_myTransform.LookAt(new Vector3(_vegetablePatchToGoTo.transform.position.x, 0, _vegetablePatchToGoTo.transform.position.z));
            //
            //_myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);

            if (!_restartFoodTime)
                _restartFoodTime = true;

            if (_gameManager.allFood.Count > 0)
            {
                if (finalDest == Vector3.zero)
                {
                    finalDest = _gameManager.allFood[0].transform.position;
                }
            
                if (_gameManager.allFood.Count > 1)
                {
                    for (int i = 0; i < _gameManager.allFood.Count; i++)
                    {
                        Vector3 dist = _gameManager.allFood[i].transform.position;
                
                        if ((dist - _myTransform.position).sqrMagnitude < (finalDest - _myTransform.position).sqrMagnitude)
                        {
                            finalDest = dist;
                        }
                    }
                }

                _myTransform.LookAt(new Vector3(finalDest.x, 0, finalDest.z));

                _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
            }
        };
        _LookingForFood.OnExit += x => 
        {
            finalDest = Vector3.zero;

            _stateLoadFood = false; 
        };

        //_Collect.OnEnter += x => 
        //{
        //    particleHarvest.Play();
        //    _stateLookingForFood = true; 
        //};
        //_Collect.OnFixedUpdate += () => 
        //{
        //    //CountTimerCollectFood();
        //};
        //_Collect.OnExit += x =>
        //{
        //    particleHarvest.Stop();
        //    _stateLookingForFood = false;
        //};

        _goToTable.OnEnter += x =>
        {

        };
        _goToTable.OnFixedUpdate += () => 
        {
            _myTransform.LookAt(new Vector3(_canteenToLoad.transform.position.x, 0, _canteenToLoad.transform.position.z));

            _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
        };
        _goToTable.OnExit += x =>
        {

        };

        _LoadFood.OnEnter += x => 
        {
            particleCooking.Play();
            _stateLoadFood = true; 
        };
        _LoadFood.OnFixedUpdate += () => 
        {
            //_myTransform.LookAt(new Vector3(_canteenToLoad.transform.position.x, 0, _canteenToLoad.transform.position.z));

            _myTransform.LookAt(new Vector3(_foodPatch.transform.position.x, 0, _foodPatch.transform.position.z));

            _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
        };
        _LoadFood.OnExit += x => 
        {
            particleCooking.Stop();
            _stateLoadFood = false; 
        };

        _WaitForCompany.OnEnter += x =>
        {
            //Partículas de enojo ON
        };
        _WaitForCompany.OnFixedUpdate += () =>
        {
            if (_constructor.isReadyToEat)
            {
                SentToFSM(ChefStates.Eat);
            }
        };
        _WaitForCompany.OnExit += x =>
        {
            //Partículas de enojo OFF
        };

        _Eat.OnEnter += x =>
        {
            particleHungry.Play();
            _stateEat = true;

            if (_stateEat)
            {
                if (_hungerActive)
                    _hungerActive = false;
                
                if (!_isEating)
                    _isEating = true;
            }
        };
        _Eat.OnFixedUpdate += () => 
        {
            //_myTransform.LookAt(new Vector3(_canteenToLoad.transform.position.x, 0, _canteenToLoad.transform.position.z));
            //
            //_myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
        };
        _Eat.OnExit += x =>
        {
            particleHungry.Stop();
            _stateEat = false;
            _isEating = false;
        };

        _stateDeath.OnEnter += x => { Death.Play(); };
        _stateDeath.OnFixedUpdate += () => 
        {
            if (_hunger > _hungerMaxCapacity)
            {
                GameObject effect = Instantiate(_particleDeathObject, transform.position, Quaternion.identity);
                Destroy(effect, 3f);

                Destroy(gameObject);
            }
        };

        _MyFSM = new EventFSM<ChefStates>(_LookingForFood);
    }
    #endregion

    void FixedUpdate()
    {
        _MyFSM.FixedUpdate();

        if (_hungerActive)
            CountTimerHunger();

        if (_isEating)
            CountTimerEatFood();

        if(appleQuantity.Count + coconutQuantity.Count + beanQuantity.Count >= maxQuantityFoodCarried && !_foodForCanteenReady)
        {
            _maxCapacityFoodReached = true;

            SentToFSM(ChefStates.LoadFood);
        }

    #region Before
        /*
        if (_hunger < _hungerMaxCapacity)
        {
            if (_stateLookingForFood && !_stateCollect && !_stateLoadFood && !_stateEat && !_stateDeath)
            {
                //LookingForFood();
            }
            else if (_stateCollect && !_stateLookingForFood && !_stateLoadFood && !_stateEat && !_stateDeath)
            {
                //Collect();
            }
            else if (_stateLoadFood && !_stateLookingForFood && !_stateCollect && !_stateEat && !_stateDeath)
            {
                //LoadFood();
            }
            else if (_stateEat && !_stateLookingForFood && !_stateCollect && !_stateLoadFood && !_stateDeath)
            {
                //Eat();
            }
        }
        else
        {
            //if (_stateDeath && !_stateLookingForFood && !_stateCollect && !_stateLoadFood && !_stateEat)
            //{
            //    Death();
            //}
        }
        */
        #endregion
    }

    public void TransferFoodToChef(List<Apple> FPAppleList, List<Coconut> FPCoconutList, List<Bean> FPBeanList)
    {
        if(FPAppleList.Count > 0)
        {
            if(_canteenToLoad.appleListQuantity.Count > 0)
            {
                for (int i = 0; i < _canteenToLoad.appleListQuantity.Count; i++)
                {
                    appleQuantity.Add(FPAppleList[i]);
                }
            }
        }

        if (FPCoconutList.Count > 0)
        {
            if (_canteenToLoad.coconutListQuantity.Count > 0)
            {
                for (int i = 0; i < _canteenToLoad.coconutListQuantity.Count; i++)
                {
                    coconutQuantity.Add(FPCoconutList[i]);
                }
            }
        }

        if (FPBeanList.Count > 0)
        {
            if (_canteenToLoad.beanListQuantity.Count > 0)
            {
                for (int i = 0; i < _canteenToLoad.beanListQuantity.Count; i++)
                {
                    beanQuantity.Add(FPBeanList[i]);
                }
            }
        }
    }

    #region CountTimer
    /*
    void CountTimerCollectFood()
    {
        if (!_canCountTimeFood)
        {
            if (!_restartFoodTime)
                _restartFoodTime = true;
        }

        if (_timeFood != 0 && _restartFoodTime)
        {
            _timeFood = 0;
            _restartFoodTime = false;
        }

        _timeFood += Time.fixedDeltaTime;

        if (_timeFood >= _timerFood)
        {
            _food += _foodToGain;

            if (_food >= _foodMaxCapacity)
            {
                //if (_stateCollect)
                //    _stateCollect = false;
                //if (!_stateLoadFood)
                //    _stateLoadFood = true;

                SentToFSM(ChefStates.LoadFood);
            }
            _timeFood = 0;
        }
    }
    */

    void CountTimerEatFood()
    {
        if (!_canCountTimeEatFood)
        {
            if (!_restartEatFoodTime)
                _restartEatFoodTime = true;

            return;
        }

        if (_timeHunger != 0 && _restartEatFoodTime)
        {
            _timeHunger = 0;

            _restartEatFoodTime = false;
        }

        _timeHunger += Time.fixedDeltaTime;

        if (_timeHunger >= _timerHunger)
        {
            //_treeToCut.RemoveWood(_woodToGain);

            float value = _hungerToLose - _hunger;

            _hunger -= _hungerToLose;

            if (_hungerToLose <= _canteenToLoad.foodQuantity)
            {
                if (_hunger >= 0)
                {
                    _canteenToLoad.TakeFood(_hungerToLose);
                }
                else
                {
                    _canteenToLoad.TakeFood(value);
                }
            }
            else
            {
                _canteenToLoad.TakeFood(_canteenToLoad.foodQuantity);

                _hunger = 0;
            }

            //if (_hunger <= _canteenToLoad.foodQuantity)
            //{
            //    _canteenToLoad.TakeFood(_hunger);
            //
            //    _hunger = 0;
            //}
            //else
            //{
            //    _hunger -= _canteenToLoad.foodQuantity;
            //
            //    _canteenToLoad.TakeFood(_canteenToLoad.foodQuantity);
            //}

            if (_hunger <= 0)
            {
                if (_hunger < 0)
                {
                    _hunger = 0;
                }

                //if (_stateEat)
                //    _stateEat = false;
                //
                //if (_isEating)
                //    _isEating = false;

                if (!_restartFoodTime)
                    _restartFoodTime = true;

                if (!_restartHungerTime)
                    _restartHungerTime = true;

                if (!_hungerActive)
                    _hungerActive = true;

                //if (_food < _foodMaxCapacity)
                //{
                //    SentToFSM(ChefStates.LookingForFood);
                //}
                //else
                //{
                //    SentToFSM(ChefStates.LoadFood);
                //}

                SentToFSM(ChefStates.LookingForFood);

                if (!_restartEatFoodTime)
                    _restartEatFoodTime = true;
            }

            _timeHunger = 0;
        }
    }

    void CountTimerHunger()
    {
        if (!_canCountTimeHunger)
        {
            if (!_restartHungerTime)
                _restartHungerTime = true;

            return;
        }

        if (_timeHunger != 0 && _restartHungerTime)
        {
            _timeHunger = 0;

            _restartHungerTime = false;
        }

        _timeHunger += Time.fixedDeltaTime;

        if (_timeHunger >= _timerHunger)
        {
            //_treeToCut.RemoveWood(_woodToGain);

            _hunger += _hungerToGain;

            if (_hunger > _hungerMinCapacity && _hunger < _hungerMaxCapacity)
            {
                SentToFSM(ChefStates.Eat);
            }
            else if (_hunger >= _hungerMaxCapacity)
            {
                SentToFSM(ChefStates.stateDeath);
            }

            _timeHunger = 0;
        }
    }
    #endregion

    //void SetValueRandom(float valueToRandom, float min, float max)
    //{
    //    Debug.Log("SetValueRandom");
    //
    //    valueToRandom = Random.Range(min, max);
    //}

    //void ChangeTransformRotationY(float value)
    //{
    //    Debug.Log("ChangeTransformRotationY");
    //
    //    _newVector3Rotation = new Vector3(_myTransform.rotation.eulerAngles.x, value, _myTransform.rotation.eulerAngles.z);
    //
    //    _myTransform.localEulerAngles = _newVector3Rotation;
    //}

    //void DoTransformRotationYWithRandomValue()
    //{
    //    if (!_doOnce)
    //        return;
    //
    //    //SetValueRandom(_randomValueForAngle, 0, 361);
    //
    //    _randomValueForAngle = Random.Range(0, 361);
    //
    //    ChangeTransformRotationY(_randomValueForAngle);
    //
    //    _doOnce = false;
    //}

    #region States
    //void LookingForFood() //[STATE 1 - Inicial]
    //{
    //    Debug.Log("LookingForFood");
    //
    //    if (!_restartFoodTime)
    //        _restartFoodTime = true;
    //
    //    _myTransform.LookAt(new Vector3(_vegetablePatchToGoTo.transform.position.x, 0, _vegetablePatchToGoTo.transform.position.z));
    //
    //    _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
    //}

    //void Collect() //[STATE 2]
    //{
    //    Debug.Log("Collect");
    //    CountTimerCollectFood();
    //}

    //void LoadFood() //[STATE 3]
    //{
    //    Debug.Log("LoadFood");
    //
    //    _myTransform.LookAt(new Vector3(_canteenToLoad.transform.position.x, 0, _canteenToLoad.transform.position.z));
    //
    //    _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
    //}

    //void Eat() //[STATE 4]
    //{
    //    Debug.Log("Eat");
    //
    //    _myTransform.LookAt(new Vector3(_canteenToLoad.transform.position.x, 0, _canteenToLoad.transform.position.z));
    //
    //    _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
    //}

    //void Death() //[STATE 5]
    //{
    //    Debug.Log("Death");
    //
    //    Destroy(gameObject);
    //}
    #endregion

    void SentToFSM(ChefStates states)
    {
        _MyFSM.SendInput(states);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 9 && _maxCapacityFoodReached)
        {
            //Debug.Log("Colisiona con FoodPatch");

            var foodpatch = collision.gameObject.GetComponent<FoodPatch>();

            if(foodpatch != null)
            {
                //Debug.Log("Chequea que FoodPatch tenga código");

                foodpatch.TransferFoodToFoodPatch(appleQuantity, coconutQuantity, beanQuantity);

                appleQuantity.Clear();
                coconutQuantity.Clear();
                beanQuantity.Clear();

                if (_foodPatch.appleListQuantityFP.Count >= _canteenToLoad.appleListQuantity.Count && _foodPatch.coconutListQuantityFP.Count >= _canteenToLoad.coconutListQuantity.Count && _foodPatch.beanListQuantityFP.Count >= _canteenToLoad.beanListQuantity.Count)
                {
                    TransferFoodToChef(_foodPatch.appleListQuantityFP, _foodPatch.coconutListQuantityFP, _foodPatch.beanListQuantityFP);

                    _foodPatch.appleListQuantityFP.RemoveRange(0, _canteenToLoad.appleListQuantity.Count);
                    _foodPatch.coconutListQuantityFP.RemoveRange(0, _canteenToLoad.coconutListQuantity.Count);
                    _foodPatch.beanListQuantityFP.RemoveRange(0, _canteenToLoad.beanListQuantity.Count);

                    _foodForCanteenReady = true;

                    SentToFSM(ChefStates.GoToTable);
                }
                else
                    SentToFSM(ChefStates.LookingForFood);
            }

            _maxCapacityFoodReached = false;

            //SentToFSM(ChefStates.Collect);
        }
        else if (collision.gameObject.layer == 10 && _foodForCanteenReady)
        {
            var canteen = collision.gameObject.GetComponent<Canteen>();

            if(canteen != null)
            {
                canteen.TransferFoodToCanteen(appleQuantity, coconutQuantity, beanQuantity);

                appleQuantity.Clear();
                coconutQuantity.Clear();
                beanQuantity.Clear();

                SentToFSM(ChefStates.WaitForCompany);
            }

            _foodForCanteenReady = false;

            //SentToFSM(ChefStates.WaitForCompany);

            //var canteen = collision.gameObject.GetComponent<Canteen>();
            //
            //if(canteen != null)
            //{
            //
            //}

            //if (_stateLoadFood)
            //{
            //    _canteenToLoad.AddFood(_food);
            //
            //    _food = 0;
            //    SentToFSM(ChefStates.LookingForFood);
            //}
            //else if (_stateEat)
            //{
            //    if (_hungerActive)
            //        _hungerActive = false;
            //    
            //    if (!_isEating)
            //        _isEating = true;
            //}
        }
    }
}
