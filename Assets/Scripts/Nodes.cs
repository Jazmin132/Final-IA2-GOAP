using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nodes : MonoBehaviour
{
    public List<Nodes> Vecinos = new List<Nodes>();
    public float radious;

    void Start()
    {
        GameManager.instance._AllNodes.Add(this);

        foreach (var Node in GameManager.instance._AllNodes)
        {
            if (radious < Vector3.Distance(transform.position,Node.transform.position))
            {
                Vecinos.Add(Node);
            }
        }
    }

    public void GetNeighbours()
    {
        //Distancias entre los vecinos
    }
}
