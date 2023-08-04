using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public List<TreeScript> trees = new List<TreeScript>();

    [SerializeField]
    float _limitMapRL = 25f;

    [SerializeField]
    float _limitMapUD = 25f;

    private void Awake()
    {
        instance = this;
    }

    public void AddTree(TreeScript t)
    {
        //Añade un objeto que contenga el script Tree a la lista trees
        if (!trees.Contains(t))
            trees.Add(t);
    }

    public void RemoveTree(TreeScript t)
    {
        //Remueve un objeto que contenga el script Tree de la lista trees
        if (trees.Contains(t))
            trees.Remove(t);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        //Calcula los 4 vertices que forman el area
        Vector3 topLeft = new Vector3(-_limitMapRL, 0, _limitMapUD);
        Vector3 topRight = new Vector3(_limitMapRL, 0, _limitMapUD);
        Vector3 bottomRight = new Vector3(_limitMapRL, 0, -_limitMapUD);
        Vector3 bottomLeft = new Vector3(-_limitMapRL, 0, -_limitMapUD);

        //Dibuja las lineas del area a partir de los vertices
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
