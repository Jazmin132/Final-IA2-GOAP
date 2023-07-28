using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEditor.Progress;

public class Boid : GridEntity
{
    public List<Food> Foods;
    public GameObject hunter;
    public Agent agent;

    private Vector3 _velocity;
    public float maxSpeed;
    [Range(0f, 0.5f)]
    public float maxForce;
    public float viewRadius;
    public float separationRadius;
    public float arriveRadius;
    public float eatRadius;

    [Range(0f, 3f)]
    public float separationWeight;
    [Range(0f, 3f)]
    public float alignmentWeight;
    [Range(0f, 3f)]
    public float cohesionWeight;
    [Range(0f, 3f)]
    public float arriveWeight;
    [Range(0f, 3f)]
    public float evadeWeight;


    void Start()
    {
        GameManager.instance.AddBoid(this);

        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        randomDir.Normalize();
        randomDir *= maxSpeed;
        AddForce(randomDir);
    }

    public void Update()
    {
        //base.Update();//TP2 IA2
        AddForce(Separation() * separationWeight);
        AddForce(Alignment() * alignmentWeight);
        AddForce(Cohesion() * cohesionWeight);
        AddForce(Evade() * evadeWeight); //Evade es lo contrario a pursuit
        AddForce(Arrive() * arriveWeight);

        transform.position += _velocity * Time.deltaTime;
        transform.forward = _velocity;        
        CheckBounds();
        transform.position = new Vector3(transform.position.x,0, transform.position.z);
        //FakeUpdate();//TP2 IA2
    }

    public Vector3 GetVelocity()
    {
        return _velocity;
    }

    #region Separation
    Vector3 Separation()
    {
        Vector3 desired = Vector3.zero;
        foreach (Boid boid in GameManager.instance.allBoids)
        {
            Vector3 dist = boid.transform.position - transform.position;
            if (dist.magnitude <= separationRadius)
            {
                desired += dist;
            }
        }
        if (desired == Vector3.zero) return desired; 
        desired *= -1;
        return CalculateSteering(desired);
    }
    #endregion

    #region Alignment
    Vector3 Alignment()
    {
        Vector3 desired = Vector3.zero;
        int count = 0;
        foreach (var item in GameManager.instance.allBoids)
        {
            if (item == this) continue;
            if (Vector3.Distance(transform.position, item.transform.position) <= viewRadius)
            {
                desired += item._velocity;
                count++;
            }
        }
        if (count == 0) return desired;
        desired /= (float)count;
        return CalculateSteering(desired);
        //var closestBoid = GameManager.instance.allBoids.Where(x => Vector3.Distance(transform.position, x.gameObject.transform.position) <= viewRadius).OrderBy(x => x).Take(1);
    }
    #endregion

    #region Cohesion
    Vector3 Cohesion()
    {
        Vector3 desired = Vector3.zero;
        int count = 0;
        foreach (var item in GameManager.instance.allBoids)
        {
            if (item == this) continue;

            if (Vector3.Distance(transform.position, item.transform.position) <= viewRadius)
            {
                desired += item.transform.position;
                count++;
            }
        }
        if (count == 0) return desired;

        desired /= (float)count;
        desired -= transform.position;

        return CalculateSteering(desired);
    }
    #endregion

    #region Arrive
    Vector3 Arrive()
    {
        if (GameManager.instance.allFoods == null) return Vector3.zero;
        var closestFood = GameManager.instance.allFoods.Where(x => (x.gameObject.transform.position - transform.position).magnitude <= arriveRadius)
            .OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).Take(1).ToList(); //IA 2 P1

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
                //item.OnDeath();
            }
            return CalculateSteering(desired);
        }

        //foreach (var item in GameManager.instance.allFoods)
        //{
        //    if ((item.gameObject.transform.position - transform.position).magnitude <= arriveRadius) // ACA HACER QUE LO HAGA CUANDO DETECTE COMIDA EN LA CELDA DE LA GRILLA EN LA QUE ESTA POSICIONADO
        //    {
        //        Vector3 desired = item.gameObject.transform.position - transform.position;
        //        float dist = desired.magnitude;
        //        if (dist <= arriveRadius)
        //        {
        //            desired.Normalize();
        //            desired *= maxSpeed * (dist / arriveRadius);
        //        }
        //        if ((item.gameObject.transform.position - transform.position).magnitude <= eatRadius)
        //        {
        //            //f.Teleportation();
        //            EatFood(item);
        //            //item.OnDeath();
        //        }
        //        return CalculateSteering(desired);
        //    }
        //}
            return Vector3.zero;
    }
    #endregion

    Vector3 Evade()
    {
        if (Vector3.Distance(transform.position, hunter.transform.position) <= viewRadius)
        {
            Vector3 futurePos = hunter.transform.position + agent.GetVelocity();
            Vector3 desired = futurePos + hunter.transform.position;
            desired.Normalize();
            desired *= agent.maxSpeed;
            return CalculateSteering(desired);
        }
        return Vector3.zero;
    }

    Vector3 CalculateSteering(Vector3 desired)
    {
        return Vector3.ClampMagnitude((desired.normalized * maxSpeed) - _velocity, maxForce);
    }

    void CheckBounds()
    {
        transform.position = GameManager.instance.ChangeObjPosition(transform.position);
    }

    void AddForce(Vector3 force)
    {
        _velocity = Vector3.ClampMagnitude(_velocity + force, maxSpeed);
    }

    public void EatFood(Food food)
    {
        food.OnDeath();
        //if (GameManager.instance.allFoods.Count <= 0) return;
        //
        //foreach (var item in GameManager.instance.allFoods)
        //{
        //    //if ((item.gameObject.transform.position - transform.position).magnitude <= eatRadius)
        //    //{
        //    //    //f.Teleportation();
        //    //    item.OnDeath();
        //    //}
        //}
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
