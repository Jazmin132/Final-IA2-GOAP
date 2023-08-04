using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum AgentStates { IDLE, PATROL, PURSUIT, GOTODEST, RETURN }
public class Agent : GridEntity
{
    public float pursuitRadius;
    public float speed = 5;
    public float maxSpeed;
    public Nodes[] PatrolWaypoints;

    [Range(0, 0.1f)]
    public float maxForce;

    public float energy;
    public float maxEnergy;
    int _currentWaypoint;
    Vector3 _velocity;

    public SpatialGrid targetGrid;
    private Transform _target;
    Vector3 _destination;
    public float pursitSpeed;
    public LayerMask obstacleLayer;
    List<Nodes> _pathToFollow = new List<Nodes>();
    public Transform WhereToGo;
    private Nodes _NodoFinal;
    private Nodes _NodoInicial;
    public float Angle;

    [Header("Visual Values")]
    public ParticleSystem particleTired;
    public ParticleSystem particleEnojo;

    private EventFSM<AgentStates> _eventFSM;

    public bool _Idle, _Patrol, _Pursuit, _GotoDest, _Return;
    void Awake()
    {
        energy = maxEnergy;

        var Idle = new State<AgentStates>("Idle");
        var Patrol = new State<AgentStates>("Patrol");
        var Pursuit = new State<AgentStates>("Pursuit");
        var GotoDest = new State<AgentStates>("GotoDest");
        var Return = new State<AgentStates>("Return");

   #region Transitions
        StateConfigurer.Create(Idle)
            .SetTransition(AgentStates.PATROL, Patrol)
            .SetTransition(AgentStates.GOTODEST, GotoDest)
            .Done();

        StateConfigurer.Create(GotoDest)
            .SetTransition(AgentStates.PATROL, Patrol)//Si no ve a las ovejas
            .SetTransition(AgentStates.RETURN, Return)
            .SetTransition(AgentStates.PURSUIT, Pursuit).Done();//Si ve a las ovejas

        StateConfigurer.Create(Patrol)
            .SetTransition(AgentStates.IDLE, Idle)
            .SetTransition(AgentStates.GOTODEST, GotoDest)
            .SetTransition(AgentStates.PURSUIT, Pursuit).Done();

        StateConfigurer.Create(Pursuit)
            .SetTransition(AgentStates.RETURN, Return)
           .SetTransition(AgentStates.IDLE, Idle)
           .Done();

        StateConfigurer.Create(Return)
            .SetTransition(AgentStates.PATROL, Patrol).Done();//Si no ve a las ovejas

        #endregion

   #region IDLE
        Idle.OnEnter += x => { particleTired.Play(); _Idle = true; };
        Idle.OnUpdate += () =>
        {
            energy += Time.deltaTime;
            if (energy >= maxEnergy)
                SendInputToSFSM(AgentStates.PATROL);
            return;
        };
        Idle.OnExit += x => { particleTired.Stop(); _Idle = false; };

        #endregion

   #region GOTODEST
        GotoDest.OnEnter += x =>
        {
            _GotoDest = true;
            if (_pathToFollow.Count == 0)
            {
                _NodoInicial = PatrolWaypoints[0];
                _NodoFinal = GameManager.instance.GetNode(WhereToGo.position);
                _pathToFollow = GameManager.instance.CreatePath(_NodoInicial, _NodoFinal);
            }
        };
        GotoDest.OnUpdate += () =>
        {
            if (_pathToFollow.Count != 0)
            {
                PathToFollow(7);
                CheckForOveja();
            }
            else if (_pathToFollow.Count <= 0)
                SendInputToSFSM(AgentStates.RETURN);
        };
        GotoDest.OnExit += x => { _pathToFollow.Clear(); _GotoDest = false; };
   #endregion

   #region RETURN
        Return.OnEnter += x => 
        {
            _Return = true;
            transform.position += new Vector3(3.5f, 0, 2);
            if (_pathToFollow.Count == 0)
            {
                _NodoFinal = PatrolWaypoints[0];
                _NodoInicial = GameManager.instance.GetNode(transform.position);
                _pathToFollow = GameManager.instance.CreatePath(_NodoInicial, _NodoFinal);
            }
        };
        Return.OnUpdate += () =>
        {
            if (_pathToFollow.Count != 0)
            {
                PathToFollow(5);
                CheckForOveja();
            }
            else if (_pathToFollow.Count <= 0)
            {
                SendInputToSFSM(AgentStates.PATROL);
            }
        };
        Return.OnExit += x => { _pathToFollow.Clear(); _Return = false; };
        #endregion

   #region PATROL
        Patrol.OnEnter += x => { _Patrol = true; };
        Patrol.OnUpdate += () =>//IA2-LINQ
        {
            energy -= Time.deltaTime;
            if (energy <= 0) SendInputToSFSM(AgentStates.IDLE);

            NowPatrol();
            CheckForOveja();
        };
        Patrol.OnExit += x => { _Patrol = false; };
        #endregion

   #region PURSUIT

        Pursuit.OnEnter += x =>//IA2-LINQ
        {
            _Pursuit = true;
            AlertFoxes(this);
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
                    SendInputToSFSM(AgentStates.RETURN);

            AddForce(NowPursuit());

            if (_target != null)
            {
                transform.LookAt(_target);
                transform.position = Vector3.MoveTowards(transform.position, _target.position, pursitSpeed * Time.fixedDeltaTime);
            }
        };
        Pursuit.OnExit += x => { particleEnojo.Stop(); _target = null; _Pursuit = false; };

#endregion

        _eventFSM = new EventFSM<AgentStates>(Idle);
    }

    private void Start()
    {
        GameManager.instance.allFoxes.Add(this);
    }
    public void Update()
    {
        _eventFSM.Update();
        //AddForce(ObstacleAvoidance() * 5);
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

    public void NowPatrol()
    {
        Nodes waypoint = PatrolWaypoints[_currentWaypoint];
        Vector3 dir = waypoint.transform.position - transform.position;
        dir.y = 0;
        transform.forward = dir;
        transform.position += transform.forward * speed * Time.deltaTime;
        if (dir.magnitude <= 0.3f)
        {
            _currentWaypoint++;
            if (_currentWaypoint >= PatrolWaypoints.Length)
                _currentWaypoint = 0;
        }
    }
    void CheckForOveja()//IA2-LINQ
    {
        var Num = Query().OfType<Boid>()
        .Select(x => x.transform)
        .OrderBy(x => x.position - transform.position);

        foreach (var boid in Num)
        {
            Vector3 dist = boid.transform.position - transform.position;
            if (dist.magnitude <= pursuitRadius)
                SendInputToSFSM(AgentStates.PURSUIT);
        }
    }
    public void AlertFoxes(Agent fox)
    {
        GameManager.instance.CallFoxes(fox);
    }
    void PathToFollow(float Speed)
    {
        Vector3 nextP = _pathToFollow[0].transform.position;
        Vector3 dir = nextP - transform.position;
        if (dir.magnitude > 0.1f)
        {
            transform.forward = dir;
            transform.position += transform.forward * Speed * Time.fixedDeltaTime;
        }
        else
            _pathToFollow.RemoveAt(0);
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
    /*
    Vector3 ObstacleAvoidance()
    {
        Vector3 desired = default;
        Collider[] obstacles = Physics.OverlapSphere(transform.position, pursuitRadius, obstacleLayer);

        foreach (var item in obstacles)
        {
            if (Vector3.Angle(item.transform.position - transform.position, transform.forward) <= Angle / 2)
                return CalculateSteering(-transform.right);
        }
        if (desired != Vector3.zero) desired /= obstacles.Length;
        return (desired);
    }
    */
    Vector3 CalculateSteering(Vector3 desired)
    {
        return Vector3.ClampMagnitude((desired.normalized * speed) - _velocity, maxForce);
    }
    public void SendInputToSFSM(AgentStates agent)
    {
        _eventFSM.SendInput(agent);
    }

    Vector3 GetDirFromAngle(float Angle)
    {
        return new Vector3(Mathf.Sin(Angle * Mathf.Deg2Rad), 0, Mathf.Cos(Angle * Mathf.Deg2Rad));
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, pursuitRadius);

        Vector3 LineaA = GetDirFromAngle(-Angle / 2 + transform.eulerAngles.y);
        Vector3 LineaB = GetDirFromAngle(Angle / 2 + transform.eulerAngles.y);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + LineaA * pursuitRadius);
        Gizmos.DrawLine(transform.position, transform.position + LineaB * pursuitRadius);
    }
}