using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    [SerializeField]
    float _treeSpawnerLimitMapRL;

    [SerializeField]
    float _treeSpawnerLimitMapUD;

    [SerializeField] Vector3 spawnPoint;
    public Quaternion spawnRotation;

    [SerializeField] bool _canCountTimeSpawn, _restartSpawnTime;
    [SerializeField] float _timeSpawn, _timerSpawn;
    //[SerializeField] TreeScript _treeToSpawn;
    [SerializeField] GameObject[] _AllFood;

    //[SerializeField] CountScript _countScript;

    [SerializeField] Transform[] _foodTypeTransformObjects;

    //void Start()
    //{
    //    SpawnTree();
    //}

    void FixedUpdate()
    {
        CountTimerSpawner();

        //SpawnTree();
    }

    #region CountTimer
    void CountTimerSpawner()
    {
        if (!_canCountTimeSpawn)
        {
            if (!_restartSpawnTime)
                _restartSpawnTime = true;

            return;
        }

        if (_timeSpawn != 0 && _restartSpawnTime)
        {
            _timeSpawn = 0;

            _restartSpawnTime = false;
        }

        _timeSpawn += Time.fixedDeltaTime;

        if (_timeSpawn >= _timerSpawn)
        {
            //Instantiate(_treeToSpawn, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            SpawnTree();

            _timeSpawn = 0;
        }
    }
    #endregion

    public void SpawnTree() //TP IA2 comienza el temporizador y al terminar hace un random el cual es un spawn aleatorio
    {
        spawnPoint = new Vector3(Random.Range(-_treeSpawnerLimitMapRL, _treeSpawnerLimitMapRL), 0, Random.Range(-_treeSpawnerLimitMapUD, _treeSpawnerLimitMapUD));
        spawnRotation = new Quaternion(0, Random.Range(0, 361), 0, 0);

        var RN = Random.Range(0,3);

        //StartCoroutine(_countScript.WaitCounter(_timerSpawn, "SpawnTree"));

        Instantiate(_AllFood[RN], spawnPoint, spawnRotation, _foodTypeTransformObjects[RN]);
        //Instantiate(_treeToSpawn, spawnPoint, spawnRotation);

        //Debug.Log("bruh");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        //Calcula los 4 vertices que forman el area
        Vector3 topLeft = new Vector3(-_treeSpawnerLimitMapRL, 0, _treeSpawnerLimitMapUD);
        Vector3 topRight = new Vector3(_treeSpawnerLimitMapRL, 0, _treeSpawnerLimitMapUD);
        Vector3 bottomRight = new Vector3(_treeSpawnerLimitMapRL, 0, -_treeSpawnerLimitMapUD);
        Vector3 bottomLeft = new Vector3(-_treeSpawnerLimitMapRL, 0, -_treeSpawnerLimitMapUD);

        //Dibuja las lineas del area a partir de los vertices
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
