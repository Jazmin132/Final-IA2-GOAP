using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Boid : GridEntity
{
    //public List<Food> Foods;
    List<Food> _closestFood;
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
    [SerializeField] bool _state_Alignment, _state_Evade, _state_Arrive;
    public enum BoidStates
    {
        ALIGNMENT,
        EVADE,
        ARRIVE
    }
    public EventFSM<BoidStates> _MyFSM;
    private void Awake()
    {
        var _Alignment = new State<BoidStates>("Idle");
        var _Evade = new State<BoidStates>("Move");
        var _Arrive = new State<BoidStates>("DIE");

        StateConfigurer.Create(_Alignment)
            .SetTransition(BoidStates.EVADE, _Evade)
            .SetTransition(BoidStates.ARRIVE, _Arrive).Done();

        StateConfigurer.Create(_Evade)
            .SetTransition(BoidStates.ALIGNMENT, _Alignment)
            .Done();//.SetTransition(BoidStates.ARRIVE, _Arrive)

        StateConfigurer.Create(_Arrive)
            .SetTransition(BoidStates.EVADE, _Evade)
            .SetTransition(BoidStates.ALIGNMENT, _Alignment)
            .Done();

        _Alignment.OnEnter += x => { _state_Alignment = true; };
        _Alignment.OnFixedUpdate += () => 
        {
            AddForce(Alignment() * alignmentWeight);

            if (Vector3.Distance(transform.position, hunter.transform.position) <= viewRadius)
                SentToFSM(BoidStates.EVADE);

            CheckForFood();
        };
        _Alignment.OnExit += x => { _state_Alignment = false;};

        _Evade.OnEnter += x => { _state_Evade = true; };
        _Evade.OnFixedUpdate += () => 
        {
            AddForce(Evade() * evadeWeight);
            if (Vector3.Distance(transform.position, hunter.transform.position) > viewRadius)
                SentToFSM(BoidStates.ALIGNMENT);
        };
        _Evade.OnExit += x => { _state_Evade = false; };

        _Arrive.OnEnter += x =>
        {
            _state_Arrive = true;
            //Foods = _closestFood;
        };
        _Arrive.OnFixedUpdate += () =>
        {
            AddForce(Arrive(_closestFood[0]) * arriveWeight);

            if (Vector3.Distance(transform.position, hunter.transform.position) <= viewRadius)
                SentToFSM(BoidStates.EVADE);
            else if(_closestFood.Count >= 0)
                SentToFSM(BoidStates.ALIGNMENT);

            //    SentToFSM(BoidStates.ALIGNMENT);
            //else if (Vector3.Distance(transform.position, hunter.transform.position) <= viewRadius)
            //    SentToFSM(BoidStates.EVADE);
        };
        _Arrive.OnExit += x => { _state_Arrive = false; };

       _MyFSM = new EventFSM<BoidStates>(_Alignment);
    }
    void Start()
    {
        GameManager.instance.AddBoid(this);

        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        randomDir.Normalize();
        randomDir *= maxSpeed;
        AddForce(randomDir);
    }

    public void FixedUpdate()
    {
        _MyFSM.FixedUpdate();

        AddForce(Separation() * separationWeight);
        AddForce(Cohesion() * cohesionWeight);
       // AddForce(Alignment() * alignmentWeight);
        //AddForce(Evade() * evadeWeight); //Evade es lo contrario a pursuit
        //AddForce(Arrive() * arriveWeight);

        transform.position += _velocity * Time.deltaTime;
        transform.forward = _velocity;        
        CheckBounds();
        transform.position = new Vector3(transform.position.x,0, transform.position.z);
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

    public void CheckForFood()
    {
        _closestFood = GameManager.instance.allFoods.Where(x => (x.gameObject.transform.position - transform.position).magnitude <= arriveRadius)
           .OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).Take(1).ToList();
        
        //Debug.Log((GameManager.instance.allFoods[0].gameObject.transform.position - transform.position).magnitude <= arriveRadius);
        //Debug.Log(_closestFood.Count);
        if (_closestFood.Count > 0)
            SentToFSM(BoidStates.ARRIVE);
    }
    #region Arrive
    Vector3 Arrive(Food food)
    {
        if (GameManager.instance.allFoods == null) return Vector3.zero;
        //var closestFood = GameManager.instance.allFoods.Where(x => (x.gameObject.transform.position - transform.position).magnitude <= arriveRadius)
        //    .OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).Take(1).ToList(); //IA 2 P1
        //Debug.Log(closestFood);
        //Foods = closestFood;
        //if (closestFood.Count > 0)
        //{

        Vector3 desired = food.gameObject.transform.position - transform.position;//closestFood[0]
         float dist = desired.magnitude;
        if (dist <= arriveRadius)
        {
            desired.Normalize();
            desired *= maxSpeed * (dist / arriveRadius);
        }//closestFood[0]
        if ((food.gameObject.transform.position - transform.position).magnitude <= eatRadius)
            EatFood(food);//closestFood[0] //item.OnDeath();

        return CalculateSteering(desired);
        //}
    #region what
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
        // return Vector3.zero;
        #endregion
    }
    #endregion

    Vector3 Evade()
    {
        //if (Vector3.Distance(transform.position, hunter.transform.position) <= viewRadius)
        //{
        Debug.Log("Evade");
            Vector3 futurePos = hunter.transform.position + agent.GetVelocity();
            Vector3 desired = futurePos + hunter.transform.position;
            desired.Normalize();
            desired *= agent.maxSpeed;
            return CalculateSteering(desired);
        //}
        //return Vector3.zero;
    }

    public Vector3 GetVelocity()
    {
        return _velocity;
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
    }

    void SentToFSM(BoidStates states)
    {
        _MyFSM.SendInput(states);
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
