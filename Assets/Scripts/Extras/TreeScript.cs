using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour
{
    [SerializeField] float _woodHP, _woodHPValueToChangeToDamaged;
    [SerializeField] GameObject[] _viewModels;
    [SerializeField] LevelManager _levelManager;
    [SerializeField] Bee _Bee;

    void Awake()
    {
        if (_levelManager == null)
            _levelManager = GameObject.Find("LevelManager").GetComponent<LevelManager>();

        _levelManager.AddTree(this);
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

                if (Random.Range(0, 1) == 0)
                {
                    Instantiate(_Bee);
                    Debug.Log("CrearAbeja");
                }
                
                Destroy(gameObject);
            }
        }
    }
}
