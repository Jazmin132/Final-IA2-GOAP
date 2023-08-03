using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    [Header("ASTAR Values")]
    public TimeSlicing timeSlicing;
    public List<Nodes> _AllNodes = new List<Nodes>();

    [Header("BOID Related Values")]
    public float boundWidth;
    public float boundHeight;

    public Food food;
    public GameObject[] FoodPoints;
    int IndeX = 0;
    public List<Boid> allBoids = new List<Boid>();
    public GameObject padreGrilla;

    [Header("FOX Related Values")]
    public List<Agent> allFoxes = new List<Agent>();
    List<Nodes> _pathToFollow = new List<Nodes>();

    public static GameManager instance;

    private void Awake() 
    { 
        if (instance == null) 
            instance = this; 
    }

    public List<Nodes> SetPath(Nodes StartingNode, Nodes GoalNode)
    { //x.GetNeighbours tiene que ser una tupla de nodos con sus distancias
        return _pathToFollow = TimeSlicing.AStar(StartingNode,
            (x) => x == GoalNode, x => x.GetNeighbours(), _StartingNode => 0).ToList();
    }

    public void PathToFollow(List<Nodes> pathToFollow, Transform EntityPos, float Speed)
    {//Pasar la posoción de la entidad y el speed
        //Esto tiene que ir un el otro lado
        var col = pathToFollow.Select(x => x.transform.position).OrderBy(x => x - EntityPos.position);

        if (col.First().magnitude > 0.1f)
        {//Toma el primer indice
            EntityPos.transform.forward = col.First();
            EntityPos.transform.position += EntityPos.forward * Speed * Time.deltaTime;
        }
        else
            pathToFollow.RemoveAt(0);//Se saltea el índice para luego pasar al siguiente(ojalá funcione)
    }

    public Nodes GetNode(Vector3 pos)
    {//Obtener nodo más cercano a la entidad dependiendo de su posición actual
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
    public void ChangeFoodPos(Food food)
    {
        food.transform.position = FoodPoints[IndeX].transform.position;
        IndeX++;
        if (IndeX >= FoodPoints.Length) IndeX = 0;
    }
    public void CallFoxes(Agent fox)
    {
        for (int i = 0; i < allFoxes.Count; i++)
        {
            if (fox != allFoxes[i])
            {
                allFoxes[i].WhereToGo= fox.transform;
                allFoxes[i].SendInputToSFSM(AgentStates.GOTODEST);
            }
        }
    }
    public Vector3 ChangeObjPosition(Vector3 pos)
    {
        if (pos.z > boundHeight / 2) pos.z = -boundHeight / 2;
        if (pos.z < -boundHeight / 2) pos.z = boundHeight / 2;
        if (pos.x < -boundWidth / 2) pos.x = boundWidth / 2;
        if (pos.x > boundWidth / 2) pos.x = -boundWidth / 2;
        return pos;
    }
    /*
    public void SpawnFood()
    {
        //actualSpawnPoint = foodSpawnPoints[UnityEngine.Random.Range(0, foodSpawnPoints.Count)];
        //StartCoroutine(SpawnTimer());
    }
    private IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(spawnTimer);
        //Instantiate(food, padreGrilla.transform);
        spawnPoint = new Vector3(UnityEngine.Random.Range(-boundWidth, boundWidth), 0.5f,
            UnityEngine.Random.Range(-boundHeight, boundHeight));
        Instantiate(food, spawnPoint, spawnRotation);
    }
    */
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