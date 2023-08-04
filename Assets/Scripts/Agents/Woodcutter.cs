using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Woodcutter : MonoBehaviour
{
    [Header("NormalValues")] //Lo siguiente se puede tener normalmente pero usarlo de esta manera es más óptimo
    [SerializeField] Transform _myTransform;
    [SerializeField] Rigidbody _myRgbd;

    [Header("Values")]

    //[SerializeField] float _randomValueForAngle;
    //[SerializeField] Vector3 _newVector3Rotation;

    [Header("ImportantValues")]
    [SerializeField] float _wood;
    [SerializeField] float _woodMaxCapacity, _woodToGain;
    [SerializeField] Store _storeToLoad;
    [SerializeField] GameObject _woodsObject;

    [SerializeField] float _speed, _speedOriginal;

    [SerializeField] float _shortestDistanceToTree;
    [SerializeField] TreeScript _treeToGoTo;
    [SerializeField] bool _doOnce;

    [SerializeField] bool _canCountTimeWood, _restartWoodTime;
    [SerializeField] float _timeWood, _timerWood;
    [SerializeField] TreeScript _treeToCut;

    [Header("State")] //Uso 'bool' en vez de 'estados', reemplazar los 'bool' por los estados que va a tener
    [SerializeField] bool _stateLookingForTree, _stateCut, _stateLoadWood;

    void Awake()
    {
        //Agregarlo a una lista antes de que haga algo? -> fijarse si poner código
    }

    void FixedUpdate()
    {
        if (_stateLookingForTree && !_stateCut && !_stateLoadWood)
        {
            LookingForTree();
        }
        else if (_stateCut && !_stateLookingForTree && !_stateLoadWood)
        {
            Cut();
        }
        else if (_stateLoadWood && !_stateLookingForTree && !_stateCut)
        {
            LoadWood();
        }
    }

    #region CountTimer
    void CountTimerWood()
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
            _treeToCut.RemoveWood(_woodToGain);

            _wood += _woodToGain;

            if (_wood >= _woodMaxCapacity)
            {
                if (!_woodsObject.activeSelf)
                    _woodsObject.SetActive(true);

                if (_stateCut)
                    _stateCut = false;

                if (!_stateLoadWood)
                    _stateLoadWood = true;
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
    void LookingForTree() //[STATE 1 - Inicial]
    {
        Debug.Log("LookingForTree");

        if (!_restartWoodTime)
            _restartWoodTime = true;

        if (LevelManager.instance.trees.Count > 0)
        {
            if (_doOnce)
            {
                if (_shortestDistanceToTree != 10000)
                    _shortestDistanceToTree = 10000;

                foreach (var tree in LevelManager.instance.trees)
                {
                    if (tree != null)
                    {
                        if ((tree.transform.position - _myTransform.position).sqrMagnitude < _shortestDistanceToTree)
                        {
                            _shortestDistanceToTree = (tree.transform.position - _myTransform.position).sqrMagnitude;

                            _treeToGoTo = tree;
                        }
                    }
                }

                _doOnce = false;
            }

            _myTransform.LookAt(new Vector3(_treeToGoTo.transform.position.x, 0, _treeToGoTo.transform.position.z));

            _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
        }
    }

    void Cut() //[STATE 2]
    {
        Debug.Log("Cut");

        if (_treeToCut == null)
        {
            if (!_doOnce)
                _doOnce = true;

            if (_stateCut)
                _stateCut = false;

            if (!_stateLookingForTree)
                _stateLookingForTree = true;
        }

        CountTimerWood();
    }

    void LoadWood() //[STATE 3]
    {
        Debug.Log("LoadWood");

        _myTransform.LookAt(new Vector3(_storeToLoad.transform.position.x, 0, _storeToLoad.transform.position.z));

        _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
    }
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 7)
        {
            if (collision.gameObject.GetComponent<TreeScript>())
            {
                _treeToCut = collision.gameObject.GetComponent<TreeScript>();

                if (_stateLookingForTree)
                    _stateLookingForTree = false;

                if (!_stateCut)
                    _stateCut = true;
            }
        }
        else if (collision.gameObject.layer == 8)
        {
            _storeToLoad.AddWood(_wood);

            _wood = 0;

            if (_woodsObject.activeSelf)
                _woodsObject.SetActive(false);

            if (!_doOnce)
                _doOnce = true;

            if (_stateLoadWood)
                _stateLoadWood = false;

            if (!_stateLookingForTree)
                _stateLookingForTree = true;
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.layer == 7)
    //    {
    //        if (_stateLookingForTree)
    //            _stateLookingForTree = false;
    //
    //        if (!_stateCut)
    //            _stateCut = true;
    //    }
    //    else if (other.gameObject.layer == 8)
    //    {
    //        if (_stateCut)
    //            _stateCut = false;
    //
    //        if (!_stateLookingForTree)
    //            _stateLookingForTree = true;
    //    }
    //}
}
