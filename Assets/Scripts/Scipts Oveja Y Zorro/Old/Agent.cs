using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

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
    public Transform target;
    
    Vector3 _destination;
    public float pursitSpeed;
    public LayerMask obstacleLayer;
    List<Nodes> _pathToFollow = new List<Nodes>();
    //public Transform WhereToGo;
    private Nodes _NodoFinal;
    private Nodes _NodoInicial;
    public float Angle;

    [Header("Visual Values")]
    public ParticleSystem particleTired;
    public ParticleSystem particleEnojo;

    private EventFSM<AgentStates> _eventFSM;

    public List<Transform> _listTransforms;

    [SerializeField] bool _canChangeToSleepyFace;

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
        Idle.OnEnter += x => 
        {
            if(_canChangeToSleepyFace)
                UIManager.instance.AssignFoxesFaces(this, "Dormido");

            particleTired.Play(); _Idle = true;

            //Debug.Log("Entra en IDLE");
        };
        Idle.OnUpdate += () =>
        {
            energy += Time.deltaTime;
            if (energy >= maxEnergy)
                SendInputToSFSM(AgentStates.PATROL);
            return;
        };
        Idle.OnExit += x => { particleTired.Stop(); _Idle = false; _canChangeToSleepyFace = true; };

        #endregion

   #region GOTODEST
        GotoDest.OnEnter += x =>
        {
            UIManager.instance.AssignFoxesFaces(this, "Yendo");
            _GotoDest = true;
            if (_pathToFollow.Count == 0)
            {
                _NodoInicial = PatrolWaypoints[0];
                _NodoFinal = GameManager.instance.GetNode(target.position);
                //_NodoFinal = GameManager.instance.GetNode(WhereToGo.position);
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
        GotoDest.OnExit += x => { _pathToFollow.Clear(); _GotoDest = false; target = null; };
   #endregion

   #region RETURN
        Return.OnEnter += x => 
        {
            _Return = true;
            UIManager.instance.AssignFoxesFaces(this, "Yendo");
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
        Return.OnExit += x => { _pathToFollow.Clear(); _Return = false; target = null; };
        #endregion

   #region PATROL
        Patrol.OnEnter += x => 
        {
            _Patrol = true; 
            UIManager.instance.AssignFoxesFaces(this, "Normal");
        };
        Patrol.OnUpdate += () =>//IA2-LINQ
        {
            energy -= Time.deltaTime;
            if (energy <= 0) SendInputToSFSM(AgentStates.IDLE);
            Debug.Log("ACTUALIZANDO");
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
            UIManager.instance.AssignFoxesFaces(this, "Enojado");
            var Num = Query()
            .OfType<Boid>()
            .Select(x => x.transform)//Agregué el where
            .Where(x => (x.transform.position - transform.position).magnitude <= pursuitRadius)
            .OrderBy(x => x.position - transform.position).First();
            //_destination = Num.transform.position - transform.position;
            // if(_destination.magnitude <= pursuitRadius) 
                //target = Num.transform;     
        };
        Pursuit.OnUpdate += () =>
        {
            
            energy -= Time.deltaTime;
            if (energy <= 0)
                SendInputToSFSM(AgentStates.IDLE);

            if (target == null)//CAMBIO JULI
                SendInputToSFSM(AgentStates.RETURN);//CAMBIO JULI

            if (target != null)
                if ((target.position - transform.position).magnitude > pursuitRadius)
                    SendInputToSFSM(AgentStates.RETURN);

            AddForce(NowPursuit());

            if (target != null)
            {
                transform.LookAt(target);
                transform.position = Vector3.MoveTowards(transform.position, target.position, pursitSpeed * Time.fixedDeltaTime);
            }
        };
        Pursuit.OnExit += x => { particleEnojo.Stop(); target = null; _Pursuit = false; };

#endregion

        _eventFSM = new EventFSM<AgentStates>(Idle);
    }

    private void Start()
    {
        GameManager.instance.allFoxes.Add(this);
    }
    public override void Update()
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
       // Debug.Log("CheckingSheep");

        //if (GameManager.instance.sheepAlive)
        //{
           // Debug.Log("CheckingSheepWithAlive");

            MoveTest();

            //Debug.Log("ACTUALIZANDO"); //CAMBIO JULI
            var Num = Query().OfType<Boid>()
            .Select(x => x.transform)
            .OrderBy(x => x.position - transform.position)
            .ToList();

        Debug.LogWarning("Query DONE " + gameObject.name);
            
            //Debug.Log(Num);
            _listTransforms = Num;
            
            foreach (var boid in Num)
            {
                Vector3 dist = boid.transform.position - transform.position;
                if (dist.magnitude <= pursuitRadius)
                {
                    target = boid.transform;//CAMBIO JULI
                    Debug.Log(target);
                    SendInputToSFSM(AgentStates.PURSUIT);
                }
            }

            //foreach(GridEntity boid in Query().ToList())
            //{
            //    if (boid!= this && boid.GetComponent<Boid>())
            //    {
            //        Vector3 dist = boid.transform.position - transform.position;
            //        if(dist.magnitude <= pursuitRadius)
            //        {
            //            target = boid.transform;
            //            SendInputToSFSM(AgentStates.PURSUIT);
            //        }
            //    }
            //}
            //return;
        //}
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
        if (boid != null) return new Vector3();

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

    public void UpdateKillStreak(int value)
    {
        UIManager.instance.UpdateFoxKillStreak(this, value);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var X = collision.collider.GetComponent<Boid>();
        if (X != null) 
        {
            UIManager.instance.UpdateFoxKillStreak(this, 1);

            X.isAlive = false;
        }
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