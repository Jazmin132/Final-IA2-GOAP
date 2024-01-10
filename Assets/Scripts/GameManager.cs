using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
//using System; System no deja usar el random

public class GameManager : MonoBehaviour
{
    [Header("ASTAR Values")]
    public TimeSlicing timeSlicing;
    public List<Nodes> _AllNodes = new List<Nodes>();

    [Header("BOID Related Values")]
    public float boundWidth;
    public float boundHeight;
    public Boid BoidPrefab;
    public Food food;
    public GameObject[] FoodPoints;
    int IndeX = 0;
    public List<Boid> allBoids = new List<Boid>();
    public GameObject padreGrilla;

    [Header("FOOD Related Values")]
    public List<NewFood> allFood = new List<NewFood>();

    [Header("FOX Related Values")]
    public List<Agent> allFoxes = new List<Agent>();
    List<Nodes> _pathToFollow = new List<Nodes>();

    public static GameManager instance;
    public GameObject boidPrefab;//CAMBIO JULI

    public int respawnTime;//CAMBIO JULI
    public Quaternion spawnRotation;//CAMBIO JULI
    public UIManager UI;

    public bool sheepAlive;

    private void Awake() 
    { 
        if (instance == null) 
            instance = this; 
    }

    public List<Nodes> CreatePath(Nodes StartingNode, Nodes GoalNode)
    { //x.GetNeighbours tiene que ser una tupla de nodos con sus distancias
        return _pathToFollow = TimeSlicing.AStar(StartingNode,
            (x) => x == GoalNode, x => x.GetNeighbours(), _StartingNode => 0).ToList();
    }

    public Nodes GetNode(Vector3 pos)//IA2-LINQ
    {//Obtener nodo más cercano a la entidad dependiendo de su posición actual
        Nodes currentFirstNode = _AllNodes[0];
        var MinDist = Vector3.Distance(_AllNodes[0].transform.position, pos);

        var Nodes = _AllNodes.Where(x => (pos - x.transform.position).magnitude < MinDist);

        foreach (var CurrentNode in Nodes)
        {
            MinDist = Vector3.Distance(CurrentNode.transform.position, pos);
            currentFirstNode = CurrentNode;
        }
        return currentFirstNode;
    }

    public void AddBoid(Boid b)
    {
        if (!allBoids.Contains(b)) allBoids.Add(b);
        UI.ChangeText(1, "sheep");
    }

    public void RemoveBoid(Boid b)
    {
        allBoids.Remove(b);//CAMBIO JULI
        StartCoroutine(RespawnBoid(respawnTime));//CAMBIO JULI

        UI.ChangeText(-1, "sheep");
    }

    public void AddFood(NewFood b)
    {
        if (!allFood.Contains(b)) allFood.Add(b);
    }

    public void RemoveFood(NewFood b)
    {
        allFood.Remove(b);
    }

    private IEnumerator RespawnBoid(float delay)// Se usa para aparecer en aleatorio adentro de la zona de contencion CAMBIO JULI
    {
        yield return new WaitForSeconds(delay);

        Vector3 respawnPosition = new Vector3(Random.Range(-boundWidth / 2, boundWidth / 2), 0, Random.Range(-boundHeight / 2, boundHeight / 2));

        GameObject newBoidObject = Instantiate(boidPrefab, respawnPosition, spawnRotation);
        Boid newBoid = newBoidObject.GetComponent<Boid>();

        allBoids.Add(newBoid);
    }

    public void InstaBoid()
    {
        Instantiate(BoidPrefab.gameObject, FoodPoints[0].transform);
        Debug.Log("Instanciar Oveja");
    }

    public void ChangeFoodPos(Food food)
    {
        food.transform.position = FoodPoints[IndeX].transform.position;
        IndeX++;
        if (IndeX >= FoodPoints.Length) IndeX = 0;
    }

    public void CallFoxes(Agent fox)//IA2 LINQ
    {
        //var Foxes = allFoxes.Where(x => x != fox).ToList(); // Original
        var Foxes = allFoxes.TakeWhile(x => x != fox).ToList();
        for (int i = 0; i < Foxes.Count; i++)
        {
            //allFoxes[i].WhereToGo= fox.transform;
            allFoxes[i].target = fox.transform;
            allFoxes[i].SendInputToSFSM(AgentStates.GOTODEST);
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