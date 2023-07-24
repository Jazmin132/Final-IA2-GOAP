using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TimeSlicing timeSlicing;
    public Nodes _StartingNode;
    public Nodes _GoalNode;
    public List<Nodes> _AllNodes = new List<Nodes>();
    List<Nodes> _pathToFollow = new List<Nodes>();

    public void SetNodes(Nodes startingNode, Nodes finalNode)
    {
        _StartingNode = startingNode;
        _GoalNode = finalNode;
    }
    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void SetPath()
    { //x.GetNeighbours tiene que ser una tupla de nodos con sus distancias
        _pathToFollow = TimeSlicing.AStar(_StartingNode, (x) => x == _GoalNode, x => x.GetNeighbours(), _StartingNode => 0).ToList();
    }

    public void FollowPath(Transform Entity, float Speed)
    {//Pasar la posoción de la entidad y el speed
        var col = _pathToFollow.Select(x => x.transform.position).OrderBy(x => x - Entity.position);
        if (col.First().magnitude > 0.1f)
        {//Toma el primer indice
            Entity.transform.forward = col.First();
            Entity.transform.position += Entity.forward * Speed * Time.deltaTime;
        }
        else
            col.Skip(0);//Se saltea el índice para luego pasar al siguiente(ojalá funcione)
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