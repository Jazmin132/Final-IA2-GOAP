using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum AgentStates { IDLE, PATROL, PURSUIT }
public class Agent : GridEntity
{
    public float pursuitRadius;
    public GameObject[] allWaypoints;
    public float speed = 5;
    public float maxSpeed;
    [Range(0, 0.1f)]
    public float maxForce;
    public float energy;
    public float maxEnergy;
    int _currentWaypoint;
    Vector3 _velocity;

    public SpatialGrid targetGrid;
    private EventFSM<AgentStates> _eventFSM;
    private Transform _target;
    Vector3 _destination;
    public float pursitSpeed;

    [Header("Visual Values")]
    public ParticleSystem particleTired;
    public ParticleSystem particleEnojo;

    void Awake()
    {
        energy = maxEnergy;
        //IA2 P3 -------------------------------------------------------
        var Idle = new State<AgentStates>("Idle");
        var Patrol = new State<AgentStates>("Patrol");
        var Pursuit = new State<AgentStates>("Pursuit");

        #region Transitions
        StateConfigurer.Create(Idle)
            .SetTransition(AgentStates.PATROL, Patrol)
            .Done();

        StateConfigurer.Create(Patrol)
            .SetTransition(AgentStates.IDLE, Idle)
            .SetTransition(AgentStates.PURSUIT, Pursuit).Done();

        StateConfigurer.Create(Pursuit)
           .SetTransition(AgentStates.IDLE, Idle)
           .SetTransition(AgentStates.PATROL, Patrol)
           .Done();
        #endregion

        #region IDLE
        Idle.OnEnter += x =>
        { particleTired.Play(); };
        Idle.OnUpdate += () =>
        {
            //OnMoveTest();
            energy += Time.deltaTime;
            if (energy >= maxEnergy)
                SendInputToSFSM(AgentStates.PATROL);
            return;
        };
        Idle.OnExit += x =>
        { particleTired.Stop(); };

        #endregion

        #region PATROL

       Patrol.OnUpdate += () =>//IA2-LINQ
        {
            energy -= Time.deltaTime;
            if (energy <= 0) SendInputToSFSM(AgentStates.IDLE);

            NowPatrol();

            var Num = Query().OfType<Boid>()
            .Select(x => x.transform)
            .OrderBy(x => x.position - transform.position);

            foreach (var boid in Num)
            {
                Vector3 dist = boid.transform.position - transform.position;
                if (dist.magnitude <= pursuitRadius)
                    SendInputToSFSM(AgentStates.PURSUIT);
            }
            return;
        };
        #endregion

        #region PURSUIT

        Pursuit.OnEnter += x =>//IA2-LINQ
        {
            particleEnojo.Play();
            var Num = Query()
            .OfType<Boid>()
            .Select(x => x.transform)
            .OrderBy(x => x.position - transform.position).First();

            _destination = Num.transform.position - transform.position;
             if(_destination.magnitude <= pursuitRadius) 
                _target = Num.transform;     
        };
        Pursuit.OnUpdate += () =>
        {

            energy -= Time.deltaTime;
            if (energy <= 0)
                SendInputToSFSM(AgentStates.IDLE);

            if (_target != null)
                if ((_target.position - transform.position).magnitude > pursuitRadius)
                    SendInputToSFSM(AgentStates.PATROL);

            AddForce(NowPursuit());

            if (_target != null)
            {
                transform.LookAt(_target);
                transform.position = Vector3.MoveTowards(transform.position, _target.position, pursitSpeed * Time.fixedDeltaTime);
            }
        };
        Pursuit.OnExit += x => { particleEnojo.Stop(); };

#endregion

        _eventFSM = new EventFSM<AgentStates>(Idle);
    }

    public void Update()
    {
        _eventFSM.Update();
    }
    public IEnumerable<GridEntity> Query() //IA-P2
    {       
            //creo una "caja" con las dimensiones deseadas, y luego filtro segun distancia para formar el círculo
            return targetGrid.Query(
                transform.position + new Vector3(-pursuitRadius, 0, -pursuitRadius),
                transform.position + new Vector3(pursuitRadius, 0, pursuitRadius),
                x => {
                    var position2d = x - transform.position;
                    position2d.y = 0;
                    return position2d.sqrMagnitude < pursuitRadius * pursuitRadius;
                });        
    }

    //IA2 P3 -----------------------------------------------------------
    public void NowPatrol()
    {
        GameObject waypoint = allWaypoints[_currentWaypoint];
        Vector3 dir = waypoint.transform.position - transform.position;
        dir.y = 0;
        transform.forward = dir;
        transform.position += transform.forward * speed * Time.deltaTime;
        if (dir.magnitude <= 0.3f)
        {
            _currentWaypoint++;
            if (_currentWaypoint >= allWaypoints.Length)
                _currentWaypoint = 0;
        }
    }

    void SendInputToSFSM(AgentStates agent)
    {
        _eventFSM.SendInput(agent);
    }

    public void ChangeColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }

    Vector3 NowPursuit()//IA2-LINQ
    {
        var boid = Query().OfType<Boid>();
        
        Vector3 Steering = boid.Aggregate(new Vector3(), (x, y) =>
        {
            Vector3 futurePos = y.transform.position + y.GetVelocity();
            Vector3 desired = futurePos - transform.position;
            desired.Normalize();
            desired *= maxSpeed;
            x = desired - _velocity;
            x = Vector3.ClampMagnitude(x, maxForce);
            return x;
        });

        return Vector3.zero;
    }

    void AddForce(Vector3 force)
    {
        _velocity += force;

        if (_velocity.magnitude >= maxSpeed)
            _velocity = _velocity.normalized * maxSpeed;
    }
    public Vector3 GetVelocity()
    {
        return _velocity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, pursuitRadius);
    }
}