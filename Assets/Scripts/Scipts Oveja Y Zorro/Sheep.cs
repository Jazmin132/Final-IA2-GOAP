using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Sheep : GridEntity
{
    public List<Food> Foods;
    public GameObject hunter;
    public Agent agent2;
    public SpatialGrid targetGrid2; //

    private Vector3 _velocity2;
    public float maxSpeed2;
    [Range(0f, 0.5f)]
    public float maxForce2;
    public float viewRadius2;
    public float separationRadius2;
    public float arriveRadius2;
    public float eatRadius2;

    [Range(0f, 3f)]
    public float separationWeight2;
    [Range(0f, 3f)]
    public float alignmentWeight2;
    [Range(0f, 3f)]
    public float cohesionWeight2;
    [Range(0f, 3f)]
    public float arriveWeight2;
    [Range(0f, 3f)]
    public float evadeWeight2;

    public float hunterRadius2 = 6;
    void Start()
    {
        GameManager.instance.AddBoid(this);

        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        randomDir.Normalize();
        randomDir *= maxSpeed2;
        AddForce2(randomDir);


        //Aca
        SpatialGrid spatialGrid = FindObjectOfType<SpatialGrid>();

        if (spatialGrid != null)
        {
            targetGrid2 = spatialGrid;
            UpdateGrid2();
        }
        else
        {
            Debug.LogWarning("No se encontr� una instancia de SpatialGrid");
        }

    }

    public override void Update()
    {
        base.Update();
        AddForce2(Separation2() * separationWeight);
        AddForce2(Alignment2() * alignmentWeight2);
        AddForce2(Cohesion2() * cohesionWeight);
        AddForce2(Evade2() * evadeWeight);
        AddForce2(Arrive2() * arriveWeight);

        if (Vector3.Distance(transform.position, hunter.transform.position) <= hunterRadius)
        {
            TrazynTheInfinite.instance.SheepDestroyed(this);
            Destroy(gameObject);
            return;
        }

        transform.position += _velocity * Time.deltaTime;
        transform.forward = _velocity;
        CheckBounds2();
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);

    }
    void UpdateGrid2()
    {
        SpatialGrid spatialGrid = FindObjectOfType<SpatialGrid>();

        if (spatialGrid != null)
        {
            spatialGrid.UpdateEntity(this);
        }
        else
        {
            Debug.LogWarning("No se encontr� una instancia de SpatialGrid");
        }
    }

    public IEnumerable<GridEntity> Query()
    {
        return targetGrid.Query(
            transform.position + new Vector3(-viewRadius, 0, -viewRadius),
            transform.position + new Vector3(viewRadius, 0, viewRadius),
            x => {
                var position2d = x - transform.position;
                position2d.y = 0;
                return position2d.sqrMagnitude < viewRadius * viewRadius;
            });
    }

    public Vector3 GetVelocity2()
    {
        return _velocity;
    }

    #region Separation
    Vector3 Separation2()
    {
        Vector3 desired = Vector3.zero;
        foreach (GridEntity boid in Query().ToList())
        {
            Vector3 dist = boid.transform.position - transform.position;
            if (dist.magnitude <= separationRadius)
            {
                desired += dist;
            }
        }
        if (desired == Vector3.zero) return desired;
        desired *= -1;
        return CalculateSteering2(desired);
    }
    #endregion

    #region Alignment
    Vector3 Alignment2()
    {
        Vector3 desired = Vector3.zero;
        int count = 0;
        foreach (GridEntity boid in Query().ToList())
        {
            if (boid != this && boid.GetComponent<Sheep>())
            {
                if (Vector3.Distance(transform.position, boid.transform.position) <= viewRadius)
                {
                    desired += boid.GetComponent<Sheep>()._velocity;
                    count++;
                }
            }
        }
        if (count == 0) return desired;
        desired /= (float)count;
        return CalculateSteering2(desired);
    }
    #endregion

    #region Cohesion
    Vector3 Cohesion2()
    {
        Vector3 desired = Vector3.zero;
        int count = 0;
        foreach (GridEntity boid in Query().ToList())
        {
            if (boid == this) continue;

            if (Vector3.Distance(transform.position, boid.transform.position) <= viewRadius)
            {
                desired += boid.transform.position;
                count++;
            }
        }
        if (count == 0) return desired;

        desired /= (float)count;
        desired -= transform.position;

        return CalculateSteering2(desired);
    }
    #endregion

    #region Arrive
    Vector3 Arrive2()
    {
        if (TrazynTheInfinite.instance.allFoods == null) return Vector3.zero;
        var closestFood = TrazynTheInfinite.instance.allFoods.Where(x => (x.gameObject.transform.position - transform.position).magnitude <= arriveRadius)
            .OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).Take(1).ToList();

        Debug.Log(closestFood);
        Foods = closestFood;
        if (closestFood.Count > 0)
        {
            Vector3 desired = closestFood[0].gameObject.transform.position - transform.position;
            float dist = desired.magnitude;
            if (dist <= arriveRadius)
            {
                desired.Normalize();
                desired *= maxSpeed * (dist / arriveRadius);
            }
            if ((closestFood[0].gameObject.transform.position - transform.position).magnitude <= eatRadius)
            {
                EatFood(closestFood[0]);
            }
            return CalculateSteering2(desired);
        }

        return Vector3.zero;
    }
    #endregion

    Vector3 Evade2()
    {
        if (Vector3.Distance(transform.position, hunter.transform.position) <= viewRadius)
        {
            Vector3 futurePos = hunter.transform.position + agent.GetVelocity();
            Vector3 desired = futurePos + hunter.transform.position;
            desired.Normalize();
            desired *= agent.maxSpeed;
            return CalculateSteering2(desired);
        }
        return Vector3.zero;
    }

    Vector3 CalculateSteering2(Vector3 desired)
    {
        return Vector3.ClampMagnitude((desired.normalized * maxSpeed) - _velocity, maxForce);
    }

    void CheckBounds2()
    {
        transform.position = GameManager.instance.ChangeObjPosition(transform.position);
    }

    void AddForce2(Vector3 force)
    {
        _velocity = Vector3.ClampMagnitude(_velocity + force, maxSpeed);
    }

    public void EatFood2(Food food)
    {
        food.OnDeath();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, arriveRadius);
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, eatRadius);

    }
}
