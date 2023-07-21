using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [Header("NormalValues")] //Lo siguiente se puede tener normalmente pero usarlo de esta manera es más óptimo
    [SerializeField] Transform _myTransform;
    [SerializeField] Rigidbody _myRgbd;

    [Header("Values")]

    [SerializeField] float _randomValueForAngle;
    [SerializeField] Vector3 _newVector3Rotation;

    [Header("ImportantValues")]
    [SerializeField] float _life;
    [SerializeField] float _lifeOriginal, _lifeToLose;

    [SerializeField] float _speed, _speedOriginal;

    [SerializeField] bool _canCountTimeLife, _restartLifeTime;
    [SerializeField] float _timeLife, _timerLife;

    [SerializeField] bool _canCountTimeMove, _restartMoveTime;
    [SerializeField] float _timeMove, _timerMove;
    [SerializeField] float _timerToMove;
    [SerializeField] bool _doOnce;


    [Header("State")] //Uso 'bool' en vez de 'estados', reemplazar los 'bool' por los estados que va a tener
    [SerializeField] bool _stateIdle, _stateMoving, _stateDeath;

    void Awake()
    {
        //Agregarlo a una lista antes de que haga algo? -> fijarse si poner código
    }

    void FixedUpdate()
    {
        if (_life > 0)
        {
            CountTimerLife();

            CountTimerMoving();

            if (_stateIdle && !_stateMoving && !_stateDeath)
            {
                Idle();
            }
            else if (_stateMoving && !_stateIdle && !_stateDeath)
            {
                Moving();
            }
        }
        else
        {
            _stateIdle = false;

            _stateMoving = false;

            _stateDeath = true;

            if (_stateDeath && !_stateMoving && !_stateMoving)
                Death();
        }
    }

    #region CountTimer
    void CountTimerLife()
    {
        if (!_canCountTimeLife)
        {
            if (!_restartLifeTime)
                _restartLifeTime = true;

            return;
        }

        if (_timeLife != 0 && _restartLifeTime)
        {
            _timeLife = 0;

            _restartLifeTime = false;
        }

        _timeLife += Time.fixedDeltaTime;

        if (_timeLife >= _timerLife)
        {
            _life -= _lifeToLose;

            _timeLife = 0;
        }
    }

    void CountTimerMoving()
    {
        if (!_canCountTimeMove)
        {
            if (!_restartMoveTime)
                _restartMoveTime = true;

            return;
        }

        if (_timeMove != 0 && _restartMoveTime)
        {
            _timeMove = 0;

            _restartMoveTime = false;
        }

        _timeMove += Time.fixedDeltaTime;

        if (_timeMove >= _timerMove)
        {
            if(_timeMove < _timerToMove)
            {
                if(_stateIdle)
                    _stateIdle = false;

                if(!_stateMoving)
                    _stateMoving = true;
            }
            else
            {
                if (!_stateIdle)
                    _stateIdle = true;

                if (_stateMoving)
                    _stateMoving = false;

                _timeMove = 0;
            }
        }
    }
    #endregion

    //void SetValueRandom(float valueToRandom, float min, float max)
    //{
    //    Debug.Log("SetValueRandom");
    //
    //    valueToRandom = Random.Range(min, max);
    //}

    void ChangeTransformRotationY(float value)
    {
        Debug.Log("ChangeTransformRotationY");

        _newVector3Rotation = new Vector3(_myTransform.rotation.eulerAngles.x, value, _myTransform.rotation.eulerAngles.z);

        _myTransform.localEulerAngles = _newVector3Rotation;
    }

    void DoTransformRotationYWithRandomValue()
    {
        if (!_doOnce)
            return;

        //SetValueRandom(_randomValueForAngle, 0, 361);

        _randomValueForAngle = Random.Range(0, 361);

        ChangeTransformRotationY(_randomValueForAngle);

        _doOnce = false;
    }

    #region States
    void Idle() //[STATE 1 - Inicial]
    {
        Debug.Log("Idle");

        if (!_doOnce)
            _doOnce = true;
    }

    void Moving() //[STATE 2]
    {
        Debug.Log("Moving");

        DoTransformRotationYWithRandomValue();

        _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
    }

    void Death() //[STATE 3]
    {
        Debug.Log("Death");

        Destroy(gameObject);
    }
    #endregion
}
