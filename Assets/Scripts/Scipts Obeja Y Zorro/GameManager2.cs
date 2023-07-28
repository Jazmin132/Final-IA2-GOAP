using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;//TP IA2 P1

public class GameManager2 : MonoBehaviour
{
    public static GameManager2 instance;

    public float boundWidth;
    public float boundHeight;

    public int spawnTimer; //TP IA2
    public GameObject food; //TP IA2

    public List<Boid> allBoids = new List<Boid>();
    public List<Food> allFoods = new List<Food>();
    
    public List<Transform> foodSpawnPoints = new List<Transform>();//Array o Lista, cual sera mejor? TP IA 2
    public Transform actualSpawnPoint; //TP IA 2
    public GameObject padreGrilla;
    public Vector3 spawnPoint;
    public Quaternion spawnRotation;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddBoid(Boid b)
    {
        if (!allBoids.Contains(b))
            allBoids.Add(b);
    }

    public void AddFood(Food f)
    {
        if(!allFoods.Contains(f))
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

    public void SpawnFood() //TP IA2 comienza el temporizador y al terminar hace un random el cual es un spawn aleatorio
    {
        actualSpawnPoint = foodSpawnPoints[Random.Range(0, foodSpawnPoints.Count)];
        StartCoroutine(SpawnTimer());        
    }

    private IEnumerator SpawnTimer() //TP IA2-P2
    {
        yield return new WaitForSeconds(spawnTimer);
        //Instantiate(food, padreGrilla.transform);
        spawnPoint = new Vector3(Random.Range(-boundWidth, boundWidth), 0.5f, Random.Range(-boundHeight, boundHeight));
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
