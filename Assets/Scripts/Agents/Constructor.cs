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
    [SerializeField] bool _stateGrabingWoodForWork, _stateConstruct, _stateEat, _stateDeath;

    void Awake()
    {
        //Agregarlo a una lista antes de que haga algo? -> fijarse si poner código
    }

    void FixedUpdate()
    {
        if (_hungerActive)
        {
            CountTimerHunger();
        }

        if (_isEating)
        {
            CountTimerEatFood();
        }

        if (_isWorking)
        {
            CountTimerWork();
        }

        if (_hunger < _hungerMaxCapacity)
        {
            if (_stateGrabingWoodForWork && !_stateConstruct && !_stateEat && !_stateDeath)
            {
                GrabingWoodForWork();
            }
            else if (_stateConstruct && !_stateGrabingWoodForWork && !_stateEat && !_stateDeath)
            {
                Construct();
            }
            else if (_stateEat && !_stateGrabingWoodForWork && !_stateConstruct && !_stateDeath)
            {
                Eat();
            }
        }
        else
        {
            if (_stateDeath && !_stateGrabingWoodForWork && !_stateConstruct && !_stateEat)
            {
                Death();
            }
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
                if (_hunger < 0)
                {
                    _hunger = 0;
                }

                if (_stateEat)
                    _stateEat = false;

                if (_isEating)
                    _isEating = false;

                if (!_restartHungerTime)
                    _restartHungerTime = true;

                if (!_hungerActive)
                    _hungerActive = true;

                if (_wood <= 0)
                {
                    if (!_stateGrabingWoodForWork)
                        _stateGrabingWoodForWork = true;
                }
                else
                {
                    if (!_stateConstruct)
                        _stateConstruct = true;
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
            _hunger += _hungerToGain;

            if (_hunger > _hungerMinCapacity && _hunger < _hungerMaxCapacity)
            {
                if (_stateGrabingWoodForWork)
                    _stateGrabingWoodForWork = false;

                if (_stateConstruct)
                    _stateConstruct = false;

                if (!_stateEat)
                    _stateEat = true;

                if (_isWorking)
                    _isWorking = false;
            }
            else if (_hunger >= _hungerMaxCapacity)
            {
                if (_stateGrabingWoodForWork)
                    _stateGrabingWoodForWork = false;

                if (_stateConstruct)
                    _stateConstruct = false;

                if (_stateEat)
                    _stateEat = false;

                if (!_stateDeath)
                    _stateDeath = true;
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
                if (_stateConstruct)
                    _stateConstruct = false;

                if (!_stateGrabingWoodForWork)
                    _stateGrabingWoodForWork = true;

                if (_isWorking)
                    _isWorking = false;
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
    void GrabingWoodForWork() //[STATE 1]
    {
        Debug.Log("GrabingWoodForWork");

        _myTransform.LookAt(new Vector3(_storeToGrabWood.transform.position.x, 0, _storeToGrabWood.transform.position.z));

        _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
    }

    void Construct() //[STATE 2 - Inicial]
    {
        Debug.Log("Construct");

        _myTransform.LookAt(new Vector3(_buildingZoneToBuild.transform.position.x, 0, _buildingZoneToBuild.transform.position.z));

        _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
    }

    void Eat() //[STATE 3]
    {
        Debug.Log("Eat");

        _myTransform.LookAt(new Vector3(_canteenToEat.transform.position.x, 0, _canteenToEat.transform.position.z));

        _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
    }

    void Death() //[STATE 4]
    {
        Debug.Log("Death");

        Destroy(gameObject);
    }
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 12)
        {
            if (!_isWorking)
                _isWorking = true;
        }
        else if (collision.gameObject.layer == 10)
        {
            if (_stateEat)
            {
                if (_hungerActive)
                    _hungerActive = false;

                if (!_isEating)
                    _isEating = true;
            }
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

            if (_stateGrabingWoodForWork)
                _stateGrabingWoodForWork = false;

            if (!_stateConstruct)
                _stateConstruct = true;
        }
    }
}
