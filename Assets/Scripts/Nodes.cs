using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Nodes : MonoBehaviour
{
    public List<Nodes> Vecinos = new List<Nodes>();
    public float radious;

    void Start()
    {
        GameManager.instance._AllNodes.Add(this);
    }
    /*
    public void AddVecinos() 
    {
        foreach (var Node in GameManager.instance._AllNodes)
        {
            if (radious < Vector3.Distance(transform.position,Node.transform.position))
                Vecinos.Add(Node);
        }
    }*/

    public Tuple<Nodes, float>[] GetNeighbours()//IA2-LINQ
    {// new Tuple<Nodes, float>[] { Tuple.Create(_StartingNode, 1f), Tuple.Create(_StartingNode, 1f) }
        var ListNodosDist = new List<Tuple<Nodes, float>>();
        foreach (var Node in GameManager.instance._AllNodes)
        {
            if (radious < Vector3.Distance(transform.position, Node.transform.position))
            {
                var Distance = Vector3.Distance(transform.position, Node.transform.position);
                ListNodosDist.Add(Tuple.Create(Node, Distance));
            }
        }
        var ArrayTupla = new Tuple<Nodes, float>[ListNodosDist.Count];

        for (int i = 0; i < ArrayTupla.Length; i++)
            ArrayTupla[i] = ListNodosDist[i];

        return ArrayTupla;
    }
}
