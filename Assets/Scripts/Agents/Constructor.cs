using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constructor : MonoBehaviour
{
    [Header("NormalValues")] //Lo siguiente se puede tener normalmente pero usarlo de esta manera es más óptimo
    [SerializeField] Transform _myTransform;
    [SerializeField] Rigidbody _myRgbd;

    [Header("Values")]

    //[SerializeField] float _randomValueForAngle;
    //[SerializeField] Vector3 _newVector3Rotation;

    [Header("ImportantValues")]
    [SerializeField] Canteen _canteenToEat;

    [SerializeField] float _speed, _speedOriginal;

    [SerializeField] float _wood;
    [SerializeField] float _woodMaxCapacity, _woodToLose;
    [SerializeField] bool _canCountTimeWood, _restartWoodTime;
    [SerializeField] float _timeWood, _timerWood;
    [SerializeField] Store _storeToGrabWood;
    [SerializeField] BuildingZone _buildingZoneToBuild;

    [SerializeField] float _hunger;
    [SerializeField] float _hungerMaxCapacity, _hungerMinCapacity, _hungerToLose, _hungerToGain;
    [SerializeField] bool _canCountTimeHunger, _restartHungerTime;
    [SerializeField] float _timeHunger, _timerHunger;

    [SerializeField] bool _canCountTimeEatFood, _restartEatFoodTime;

    [SerializeField] bool _hungerActive, _isEating, _isWorking;

    [Header("State")] //Uso 'bool' en vez de 'estados', reemplazar los 'bool' por los estados que va a tener
    [SerializeField] bool _stateGrabingWood, _stateConstruct, _stateEat;

    public ParticleSystem particleBuilding;
    public ParticleSystem particleHunger;
    public ParticleSystem particleDeath;
    public ParticleSystem particleAnger;

    [SerializeField] GameObject _particleDeathObject;

    public bool isReadyToEat;

    [SerializeField] bool _goingToEat;

    [SerializeField] FlowerManager _flowerManager;

    [SerializeField] bool _notScared;
    [SerializeField] float _speedScared;

    [SerializeField] float _beeCheckRadius;

    //[SerializeField] Bee _beeTarget;

    [SerializeField] int _beesNotInRadius;

    #region ConstructorStates
    public enum ConstructorStates
    {
        GrabingWood,
        Construct,
        GoToTable,
        WaitForFood,
        Eat,
        stateDeath
    }
    public EventFSM<ConstructorStates> _MyFSM;

    void Awake()
    {
        var _GrabingWood = new State<ConstructorStates>("GrabingWood");
        var _Construct = new State<ConstructorStates>("Construct");
        var _goToTable = new State<ConstructorStates>("GoToTable");
        var _waitForFood = new State<ConstructorStates>("WaitForFood");
        var _Eat = new State<ConstructorStates>("Eat");
        var _stateDeath = new State<ConstructorStates>("stateDeath");

        StateConfigurer.Create(_GrabingWood)
            .SetTransition(ConstructorStates.Eat, _Eat)
            .SetTransition(ConstructorStates.Construct, _Construct)
            .SetTransition(ConstructorStates.GoToTable, _goToTable)
            .SetTransition(ConstructorStates.stateDeath, _stateDeath).Done();

        StateConfigurer.Create(_Construct)
            .SetTransition(ConstructorStates.Eat, _Eat)
            .SetTransition(ConstructorStates.GoToTable, _goToTable)
            .SetTransition(ConstructorStates.stateDeath, _stateDeath)
            .SetTransition(ConstructorStates.GrabingWood, _GrabingWood).Done();

        StateConfigurer.Create(_goToTable)
            .SetTransition(ConstructorStates.Eat, _Eat)
            .SetTransition(ConstructorStates.WaitForFood, _waitForFood)
            .SetTransition(ConstructorStates.stateDeath, _stateDeath)
            .SetTransition(ConstructorStates.GrabingWood, _GrabingWood).Done();

        StateConfigurer.Create(_waitForFood)
            .SetTransition(ConstructorStates.Eat, _Eat)
            .SetTransition(ConstructorStates.stateDeath, _stateDeath)
            .SetTransition(ConstructorStates.GrabingWood, _GrabingWood).Done();

        StateConfigurer.Create(_Eat)
            .SetTransition(ConstructorStates.Construct, _Construct)
            .SetTransition(ConstructorStates.stateDeath, _stateDeath)
            .SetTransition(ConstructorStates.GrabingWood, _GrabingWood).Done();


        StateConfigurer.Create(_stateDeath).Done();

        _GrabingWood.OnEnter += x => { _stateGrabingWood = true; };
        _GrabingWood.OnFixedUpdate += () => 
        {
            _myTransform.LookAt(new Vector3(_storeToGrabWood.transform.position.x, 0, _storeToGrabWood.transform.position.z));
            _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
        };
        _GrabingWood.OnExit += x => { _stateGrabingWood = false; };

        _Construct.OnEnter += x =>
        {
            _stateConstruct = true;
            particleBuilding.Play();
        };
        _Construct.OnFixedUpdate += () => 
        {
            _myTransform.LookAt(new Vector3(_buildingZoneToBuild.transform.position.x, 0, _buildingZoneToBuild.transform.position.z));
            _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
        };
        _Construct.OnExit += x =>
        { 
            _stateConstruct = false;
            _isWorking = false;
            particleBuilding.Stop();
        };

        _goToTable.OnEnter += x =>
        {
            _goingToEat = true;
            //Debug.Log("Entre en GoToTable");
            //Partículas de yendo a la mesa? ON
        };
        _goToTable.OnFixedUpdate += () =>
        {
            _myTransform.LookAt(new Vector3(_canteenToEat.transform.position.x, 0, _canteenToEat.transform.position.z));
            _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
        };
        _goToTable.OnExit += x =>
        {
            //Partículas de yendo a la mesa? OFF
        };

        _waitForFood.OnEnter += x =>
        {
            //Partículas de enojo ON
            //Debug.Log("WaitForFood ON");
            particleAnger.Play();

        };
        _waitForFood.OnFixedUpdate += () =>
        {
            if(_canteenToEat.foodQuantity > _hunger)
            {
                SentToFSM(ConstructorStates.Eat);
            }
        };
        _waitForFood.OnExit += x =>
        {
            //Partículas de enojo OFF
            particleAnger.Stop();
        };

        _Eat.OnEnter += x => 
        { 
            _stateEat = true;
            isReadyToEat = true;
            particleHunger.Play();
        };
        _Eat.OnFixedUpdate += () =>
        {
            if(_isEating == true)CountTimerEatFood();
            //_myTransform.LookAt(new Vector3(_canteenToEat.transform.position.x, 0, _canteenToEat.transform.position.z));
            //_myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
        };
        _Eat.OnExit += x => 
        { 
            _isEating = false;
            _stateEat = false;
            isReadyToEat = false;
            _goingToEat = false;
            particleHunger.Stop();
        };

        _stateDeath.OnFixedUpdate += () =>
        {
            GameObject effect = Instantiate(_particleDeathObject, transform.position, Quaternion.identity);
            Destroy(effect, 3f);

            Destroy(gameObject);
        };

        _MyFSM = new EventFSM<ConstructorStates>(_GrabingWood);
    }
    #endregion

    void FixedUpdate()
    {
        _MyFSM.FixedUpdate();

        if (_hungerActive)
        {
            CountTimerHunger();
        }
        //if (_isEating)
        //{
        //    CountTimerEatFood();
        //}
        if (_isWorking)
        {
            CountTimerWork();
        }
        #region BEFORE
        /*if (_hunger < _hungerMaxCapacity)
        {
            if (_stateGrabingWoodForWork && !_stateConstruct && !_stateEat && !_stateDeath)
            {
                //GrabingWoodForWork();Cambiar a _GrabingWood
            }
            else if (_stateConstruct && !_stateGrabingWoodForWork && !_stateEat && !_stateDeath)
            {
                //Construct();
            }
            else if (_stateEat && !_stateGrabingWoodForWork && !_stateConstruct && !_stateDeath)
            {
                //Eat();
            }
        else
        {
            if (_stateDeath && !_stateGrabingWoodForWork && !_stateConstruct && !_stateEat)
            {
                //Death();
            }
        }
        }*/
        #endregion

        if (_flowerManager.BeeTotal.Count > 0)
        {
            for (int i = 0; i < _flowerManager.BeeTotal.Count; i++)
            {
                Vector3 dist = _flowerManager.BeeTotal[i].transform.position - transform.position;
                if (dist.magnitude <= _beeCheckRadius)
                {
                    if (_beesNotInRadius > 0)
                        _beesNotInRadius--;

                    //_beeTarget = _flowerManager.BeeTotal[i];

                    if (_notScared)
                    {
                        //Mostrar asustado de este chef/ Scared

                        _speed = _speedScared;

                        _notScared = false;
                    }
                }
                else
                {
                    if (_beesNotInRadius < _flowerManager.BeeTotal.Count)
                        _beesNotInRadius++;
                }
            }
        }

        if (_beesNotInRadius == _flowerManager.BeeTotal.Count)
        {
            if (!_notScared)
            {
                //Esconder asustado de este chef

                _speed = _speedOriginal;

                _notScared = true;
            }

            _beesNotInRadius = 0;
        }
    }

    #region CountTimer
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
            float value = _hungerToLose - _hunger;

            _hunger -= _hungerToLose;

            if (_hungerToLose <= _canteenToEat.foodQuantity)
            {
                if (_hunger >= 0)
                {
                    _canteenToEat.TakeFood(_hungerToLose);
                }
                else
                {
                    _canteenToEat.TakeFood(value);
                }
            }
            else
            {
                _canteenToEat.TakeFood(_canteenToEat.foodQuantity);

                _hunger = 0;
            }

            if (_hunger <= 0)
            {
                if (_hunger < 0) _hunger = 0;

                if (_stateEat) _stateEat = false;

                if (_isEating) _isEating = false;

                if (!_restartHungerTime)
                    _restartHungerTime = true;

                if (!_hungerActive)
                    _hungerActive = true;

                if (_wood <= 0)
                {
                    //if (!_stateGrabingWoodForWork)
                    //    _stateGrabingWoodForWork = true;
                    SentToFSM(ConstructorStates.GrabingWood);
                }
                else
                {
                    //if (!_stateConstruct)
                    //    _stateConstruct = true;
                    SentToFSM(ConstructorStates.Construct);
                }

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
        }

        if (_timeHunger != 0 && _restartHungerTime)
        {
            _timeHunger = 0;
            _restartHungerTime = false;
        }

        _timeHunger += Time.fixedDeltaTime;

        if (_timeHunger >= _timerHunger)
        {
            _hunger += _hungerToGain;

            if (_hunger >= _hungerMinCapacity && _hunger < _hungerMaxCapacity)
            {
                //if (_stateGrabingWoodForWork)
                //    _stateGrabingWoodForWork = false;

                if (_stateConstruct)
                    _stateConstruct = false;
                
                if (!_stateEat)
                    _stateEat = true;
                
                if (_isWorking)
                    _isWorking = false;

                //SentToFSM(ContructorStates.Eat);

                //Debug.Log("Voy a prender el estado GoToTable");

                SentToFSM(ConstructorStates.GoToTable);
            }
            else if (_hunger >= _hungerMaxCapacity)
            {
                //if (_stateGrabingWoodForWork)
                //    _stateGrabingWoodForWork = false;
                
                if (_stateConstruct)
                    _stateConstruct = false;
                
                if (_stateEat)
                    _stateEat = false;
                
                //if (!_stateDeath)
                //    _stateDeath = true;
                SentToFSM(ConstructorStates.stateDeath);
            }

            _timeHunger = 0;
        }
    }

    void CountTimerWork()
    {
        if (!_canCountTimeWood)
        {
            if (!_restartWoodTime)
                _restartWoodTime = true;

            return;
        }

        if (_timeWood != 0 && _restartWoodTime)
        {
            _timeWood = 0;

            _restartWoodTime = false;
        }

        _timeWood += Time.fixedDeltaTime;

        if (_timeWood >= _timerWood)
        {
            float value = _woodToLose - _wood;

            _wood -= _woodToLose;

            if (value <= _woodToLose)
            {
                _buildingZoneToBuild.AddMaterials(_woodToLose);
            }
            else
            {
                _buildingZoneToBuild.AddMaterials(value);
            }

            //if (_woodToLose > _buildingZoneToBuild.materialsQuantity)
            //{
            //    if (_wood >= 0)
            //    {
            //        _buildingZoneToBuild.AddMaterials(_woodToLose);
            //    }
            //    else
            //    {
            //        _buildingZoneToBuild.AddMaterials(value);
            //    }
            //}
            //else
            //{
            //    _buildingZoneToBuild.AddMaterials(_buildingZoneToBuild.materialsQuantity);
            //
            //    _wood = 0;
            //}

            if (_wood <= 0)
            {
                //if (_stateConstruct)
                //    _stateConstruct = false;
                //
                //if (!_stateGrabingWoodForWork)
                //    _stateGrabingWoodForWork = true;
                //
                //if (_isWorking)
                //    _isWorking = false;
                SentToFSM(ConstructorStates.GrabingWood);
            }

            _timeWood = 0;
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
    //void GrabingWoodForWork() //[STATE 1]
    //{
    //    Debug.Log("GrabingWoodForWork");
    //
    //    _myTransform.LookAt(new Vector3(_storeToGrabWood.transform.position.x, 0, _storeToGrabWood.transform.position.z));
    //    _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
    //}

    //void Construct() //[STATE 2 - Inicial]
    //{
    //    Debug.Log("Construct");
    //
    //    _myTransform.LookAt(new Vector3(_buildingZoneToBuild.transform.position.x, 0, _buildingZoneToBuild.transform.position.z));
    //    _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
    //}
   
    //void Eat() //[STATE 3]
    //{
    //    Debug.Log("Eat");
    //
    //    _myTransform.LookAt(new Vector3(_canteenToEat.transform.position.x, 0, _canteenToEat.transform.position.z));
    //    _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
    //}
    #endregion

    void SentToFSM(ConstructorStates states)
    {
        _MyFSM.SendInput(states);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 12)
        {
            if (!_isWorking)
                _isWorking = true;
        }
        else if (collision.gameObject.layer == 10 && _goingToEat)
        {
            //var canteen = collision.gameObject.GetComponent<Canteen>();
            //
            //if(canteen != null)
            //{
            //
            //}

            if (_stateEat)
            {
                if (_hungerActive)
                    _hungerActive = false;

                if (!_isEating)
                    _isEating = true;
            }

            SentToFSM(ConstructorStates.WaitForFood);
        }
        else if (collision.gameObject.layer == 8)
        {
            if (_storeToGrabWood.woodQuantity >= _woodMaxCapacity)
            {
                _storeToGrabWood.TakeWood(_woodMaxCapacity);

                _wood = _woodMaxCapacity;
            }
            else
            {
                _wood = _storeToGrabWood.woodQuantity;

                _storeToGrabWood.TakeWood(_storeToGrabWood.woodQuantity);
            }

            //if (_stateGrabingWoodForWork)
            //    _stateGrabingWoodForWork = false;
            if (!_stateConstruct)
                _stateConstruct = true;
            SentToFSM(ConstructorStates.Construct);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _beeCheckRadius);
    }
}
