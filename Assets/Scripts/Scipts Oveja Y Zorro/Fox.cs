using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Security.Cryptography;

public class Fox : GridEntity
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
    private Vector3 _velocity;

    public SpatialGrid targetGrid;
    private EventFSM<AgentStates> _eventFSM;
    private Transform _target;
    private Vector3 _destination;
    public float pursitSpeed;

    void Awake()
    {
        energy = maxEnergy;
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
        {
            Debug.Log("OnEnter IDLE");
            ChangeColor(Color.grey);

        };
        Idle.OnUpdate += () =>
        {
            MoveTest();
            energy += Time.deltaTime;
            if (energy >= maxEnergy)
                SendInputToSFSM(AgentStates.PATROL);
            return;
        };
        Idle.OnExit += x =>
        {
            Debug.Log("OnExit IDLE");
        };
        #endregion

        #region PATROL
        Patrol.OnEnter += x =>
        {
            Debug.Log("OnEnter Patrol");
            ChangeColor(Color.green);
        };
        Patrol.OnUpdate += () =>
        {
            MoveTest();
            energy -= Time.deltaTime;
            if (energy <= 0)
                SendInputToSFSM(AgentStates.IDLE);
            NowPatrol();
            foreach (GridEntity boid in Query().ToList())
            {
                if (boid != this && boid.GetComponent<Sheep>())
                {
                    Vector3 dist = boid.transform.position - transform.position;
                    if (dist.magnitude <= pursuitRadius)
                    {
                        _target = boid.transform;
                        SendInputToSFSM(AgentStates.PURSUIT);
                    }
                }
            }
            return;
        };
        #endregion

        #region PURSUIT

        Pursuit.OnEnter += x =>
        {
            Debug.Log("OnEnter Pursuit");
            ChangeColor(Color.magenta);
        };
        Pursuit.OnUpdate += () =>
        {
            energy -= Time.deltaTime;
            if (energy <= 0)
                SendInputToSFSM(AgentStates.IDLE);

            if (_target == null) SendInputToSFSM(AgentStates.PATROL);

            if (_target != null)
                if ((_target.position - transform.position).magnitude > pursuitRadius) SendInputToSFSM(AgentStates.PATROL);
            AddForce(NowPursuit());

            if (_target != null)
            {
                transform.LookAt(_target);
                transform.position = Vector3.MoveTowards(transform.position, _target.position, pursitSpeed * Time.fixedDeltaTime);
            }
            return;
        };
        #endregion

        _eventFSM = new EventFSM<AgentStates>(Idle);
    }

    public override void Update()
    {
        _eventFSM.Update();
    }
    public IEnumerable<GridEntity> Query()
    {
        return targetGrid.Query(
            transform.position + new Vector3(-pursuitRadius, 0, -pursuitRadius),
            transform.position + new Vector3(pursuitRadius, 0, pursuitRadius),
            x => {
                var position2d = x - transform.position;
                position2d.y = 0;
                return position2d.sqrMagnitude < pursuitRadius * pursuitRadius;
            });
    }

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
    Vector3 NowPursuit()
    {
        var boid = Query().Where(x => x is Sheep).Select(x => x as Sheep);

        foreach (var item in boid)
        {
            Vector3 futurePos = item.transform.position + item.GetVelocity2();
            Vector3 desired = futurePos - transform.position;
            desired.Normalize();
            desired *= maxSpeed;
            Vector3 steering = desired - _velocity;
            steering = Vector3.ClampMagnitude(steering, maxForce);
            return steering;
        }
        return Vector3.zero;
    }

    void AddForce(Vector3 force)
    {
        _velocity += force;

        if (_velocity.magnitude >= maxSpeed)
        {
            _velocity = _velocity.normalized * maxSpeed;
        }
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

