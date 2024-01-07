using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrazynTheInfinite : MonoBehaviour
{
    public static TrazynTheInfinite instance;

    public float boundWidth;
    public float boundHeight;

    public int spawnTimer;
    public GameObject food;
    public GameObject boidPrefab;

    public List<Sheep> allBoids = new List<Sheep>();
    public List<Food> allFoods = new List<Food>();

    public List<Transform> foodSpawnPoints = new List<Transform>();
    public Transform actualSpawnPoint;
    public GameObject padreGrilla;
    public Vector3 spawnPoint;
    public Quaternion spawnRotation;


    public int respawnTime = 5;



    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddBoid(Sheep b)
    {
        if (!allBoids.Contains(b))
            allBoids.Add(b);
    }

    public void AddFood(Food f)
    {
        if (!allFoods.Contains(f))
            allFoods.Add(f);

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
        actualSpawnPoint = foodSpawnPoints[Random.Range(0, foodSpawnPoints.Count)];
        StartCoroutine(SpawnTimer());
    }

    private IEnumerator SpawnTimer()
    {
        yield return new WaitForSeconds(spawnTimer);

        spawnPoint = new Vector3(Random.Range(-boundWidth, boundWidth), 0.5f, Random.Range(-boundHeight, boundHeight));
        Instantiate(food, spawnPoint, spawnRotation);

    }

    public void SheepDestroyed(Sheep boid)
    {
        allBoids.Remove(boid);
        StartCoroutine(RespawnBoid(respawnTime));
    }

    private IEnumerator RespawnBoid(float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 respawnPosition = new Vector3(Random.Range(-boundWidth / 2, boundWidth / 2), 0, Random.Range(-boundHeight / 2, boundHeight / 2));

        GameObject newBoidObject = Instantiate(boidPrefab, respawnPosition, spawnRotation);
        Sheep newBoid = newBoidObject.GetComponent<Sheep>();

        allBoids.Add(newBoid);

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
