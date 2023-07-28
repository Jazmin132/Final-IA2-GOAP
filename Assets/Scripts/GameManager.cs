using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    [Header("ASTAR Values")]
    public TimeSlicing timeSlicing;
    public Nodes _StartingNode;
    public Nodes _GoalNode;
    public List<Nodes> _AllNodes = new List<Nodes>();
    List<Nodes> _pathToFollow = new List<Nodes>();

    [Header("BOID Related Values")]
    public float boundWidth;
    public float boundHeight;

    public int spawnTimer;
    public GameObject food;

    public List<Boid> allBoids = new List<Boid>();
    public List<Food> allFoods = new List<Food>();
    
    public List<Transform> foodSpawnPoints = new List<Transform>();
    public Transform actualSpawnPoint;
    public GameObject padreGrilla;
    public Vector3 spawnPoint;
    public Quaternion spawnRotation;

    public static GameManager instance;

    private void Awake() { if (instance == null) instance = this; }

    public void SetNodes(Nodes startingNode, Nodes finalNode)
    {
        _StartingNode = startingNode;
        _GoalNode = finalNode;
    }
    public void SetPath()
    { //x.GetNeighbours tiene que ser una tupla de nodos con sus distancias
        _pathToFollow = TimeSlicing.AStar(_StartingNode, (x) => x == _GoalNode, x => x.GetNeighbours(), _StartingNode => 0).ToList();
    }

    public void FollowPath(Transform Entity, float Speed)
    {//Pasar la posoci�n de la entidad y el speed
        var col = _pathToFollow.Select(x => x.transform.position).OrderBy(x => x - Entity.position);
        if (col.First().magnitude > 0.1f)
        {//Toma el primer indice
            Entity.transform.forward = col.First();
            Entity.transform.position += Entity.forward * Speed * Time.deltaTime;
        }
        else
            col.Skip(0);//Se saltea el �ndice para luego pasar al siguiente(ojal� funcione)
    }

    public Nodes GetNode(Vector3 pos)
    {//Obtener nodo m�s cercano a la entidad dependiendo de su posici�n actual
        Nodes currentFirstNode = _AllNodes[0];
        var MinDist = Vector3.Distance(_AllNodes[0].transform.position, pos);

        foreach (var CurrentNode in _AllNodes)
        {
            if (Vector3.Distance(CurrentNode.transform.position, pos) < MinDist)//&& VisibleDist(InLineOffSight)
            {
                MinDist = Vector3.Distance(CurrentNode.transform.position, pos);
                currentFirstNode = CurrentNode;
            }
        }
        return currentFirstNode;
    }

    public void AddBoid(Boid b)
    {
        if (!allBoids.Contains(b))
            allBoids.Add(b);
    }
    public void AddFood(Food f)
    {
        if (!allFoods.Contains(f))
            allFoods.Add(f);
        //allFoods.OrderBy(x => x);
    }
    public Vector3 ChangeObjPosition(Vector3 pos)
    {
        if (pos.z > boundHeight / 2) pos.z = -boundHeight / 2;
        if (pos.z < -boundHeight / 2) pos.z = boundHeight / 2;
        if (pos.x < -boundWidth / 2) pos.x = boundWidth / 2;
        if (pos.x > boundWidth / 2) pos.x = -boundWidth / 2;
        return pos;
    }
    public void SpawnFood()
    {
        actualSpawnPoint = foodSpawnPoints[UnityEngine.Random.Range(0, foodSpawnPoints.Count)];
        StartCoroutine(SpawnTimer());
    }

    private IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(spawnTimer);
        //Instantiate(food, padreGrilla.transform);
        spawnPoint = new Vector3(UnityEngine.Random.Range(-boundWidth, boundWidth), 0.5f,
            UnityEngine.Random.Range(-boundHeight, boundHeight));
        Instantiate(food, spawnPoint, spawnRotation);
    }

    private void OnDrawGizmos()
    {
        Vector3 topLeft = new Vector3(-boundWidth / 2, 0, boundHeight / 2);
        Vector3 topRight = new Vector3(boundWidth / 2, 0, boundHeight / 2);
        Vector3 botRight = new Vector3(boundWidth / 2, 0, -boundHeight / 2);
        Vector3 botLeft = new Vector3(-boundWidth / 2, 0, -boundHeight / 2);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, botRight);
        Gizmos.DrawLine(botRight, botLeft);
        Gizmos.DrawLine(botLeft, topLeft);
    }
}

//public void FollowPath(Transform Entity, float Speed)
//Vector3 nextP = _pathToFollow[0].transform.position;
//Vector3 dir = nextP - Entity.position;
//if (dir.magnitude > 0.1f)
//{
//    Entity.transform.forward = dir;
//    Entity.transform.position += Entity.forward * Speed * Time.deltaTime;
//}
//else
//    _pathToFollow.RemoveAt(0);