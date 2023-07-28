using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : GridEntity
{
    public GameObject[] wp;
    private int _index;

    private void Start()
    {
        _index = 0;

        GameManager.instance.AddFood(this);
        if(GameManager.instance.actualSpawnPoint != null)transform.position = GameManager.instance.actualSpawnPoint.position;
        //OnMoveTest();
        //SpatialGrid.Instance.UpdateEntity(this);
    }

    //public override void Update()
    //{
    //}

    public void Teleportation()
    {
        if (_index < wp.Length - 1)
        {
            _index++;
            transform.position = wp[_index].transform.position - transform.position;
        }
        else _index = 0;
    }
    //Esto reemplazaria teleportation y se me ocurre una idea, de que al ser destruido la comida, al mismo tiempo se llama un temporizador a un gameObject con los WP
    //Los WP serian los posibles spawns de la comida, el GameObject/Empty hara un Random.Range con el Length del Array donde aparecera una comida
    //El Enumerator podria estar en el GameManager
    public void OnDeath()//IA-P2
    {
       // GameManager.instance.SpawnFood();
       // GameManager.instance.allFoods.Remove(this);
        Destroy(gameObject);
    }
}
