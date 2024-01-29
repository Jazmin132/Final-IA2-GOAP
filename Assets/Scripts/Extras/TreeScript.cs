using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour
{
    [SerializeField] float _woodHP, _woodHPValueToChangeToDamaged;
    [SerializeField] GameObject[] _viewModels;
    [SerializeField] LevelManager _levelManager;
    [SerializeField] Bee _Bee;

    [SerializeField] Vector3 _beeSpawnPosition;
    [SerializeField] Quaternion _beeSpawnRotation;

    [SerializeField] float _beeSpawnPositionYValueToAdd;

    [SerializeField] float _treeRotationY;

    [SerializeField] Transform _beesTransformObject;

    void Awake()
    {
        if (_levelManager == null)
            _levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();

        _levelManager.AddTree(this);

        _treeRotationY = Random.Range(0, 361);

        transform.eulerAngles = new Vector3 (transform.rotation.x, _treeRotationY, transform.rotation.z);
    }

    void Start()
    {
        if(_beesTransformObject == null)
        {
            _beesTransformObject = GameManager.instance.beesTransformObject;
        }
    }

    public void RemoveWood(float quantity)
    {
        _woodHP -= quantity;

        if (_woodHP <= _woodHPValueToChangeToDamaged)
        {
            if (_viewModels[0].activeSelf)
                _viewModels[0].SetActive(false);

            if (!_viewModels[1].activeSelf)
                _viewModels[1].SetActive(true);

            if (_woodHP <= 0)
            {
                _levelManager.RemoveTree(this);

                if (Random.Range(0, 2) == 0)
                {
                    //gameObject.SetActive(false);

                    _beeSpawnPosition = new Vector3 (transform.position.x, transform.position.y + _beeSpawnPositionYValueToAdd, transform.position.z);

                    //Destroy(gameObject);

                    Instantiate(_Bee, _beeSpawnPosition, _beeSpawnRotation, _beesTransformObject);
                    //Debug.Log("CrearAbeja");
                }
                
                Destroy(gameObject);
            }
        }
    }
}
