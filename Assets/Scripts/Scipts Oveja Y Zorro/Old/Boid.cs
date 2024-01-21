using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Plant;
using Unity.VisualScripting;

public class Boid : GridEntity
{
    List<Food> _closestFood;
    //public GameObject hunter;
    //public Agent agent;

    private Vector3 _velocity;
    public float maxSpeed;
    [Range(0f, 0.5f)]
    public float maxForce;
    public float viewRadius;
    public float separationRadius;
    public float arriveRadius;
    public float eatRadius;
    public float hunterRadius; // ESTO ES NUEVO PARA EL DESTROY CAMBIO JULI
    public SpatialGrid targetGrid;// CAMBIO JULI
    public ParticleSystem particleHungry;
    public ParticleSystem particleScared;

    [SerializeField] float _randomValueForAngle;
    [SerializeField] Vector3 _newVector3Rotation;

    [SerializeField] Transform _myTransform;
    [SerializeField] Rigidbody _myRgbd;

    [SerializeField] bool _canCountTimeMove, _restartMoveTime;
    [SerializeField] float _timeMove, _timerMove;
    [SerializeField] float _timerToMove;
    public bool isAlive = true;

    [Range(0f, 3f)]
    public float separationWeight;
    [Range(0f, 3f)]
    public float cohesionWeight;
    [Range(0f, 3f)]
    public float arriveWeight;
    [Range(0f, 3f)]
    public float evadeWeight;

    [SerializeField] GameManager _gameManager;


    [SerializeField] Agent _selectedAgent;

    [SerializeField] int _sheepValue;

    public enum BoidStates
    {
        ALIGNMENT,
        EVADE,
        ARRIVE,
        DIE
    }
    public EventFSM<BoidStates> _MyFSM;
    private void Awake()
    {
        if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();

        //if(gameObject.transform.parent != _gameManager.gridObject.transform)
        //{
        //    gameObject.transform.parent = _gameManager.gridObject.transform;
        //}

        var _Alignment = new State<BoidStates>("Idle");
        var _Evade = new State<BoidStates>("Move");
        var _Arrive = new State<BoidStates>("Arrive");
        var _Death = new State<BoidStates>("DIE");

        StateConfigurer.Create(_Alignment)
            .SetTransition(BoidStates.EVADE, _Evade)
            .SetTransition(BoidStates.ARRIVE, _Arrive)
            .SetTransition(BoidStates.DIE, _Death) .Done();

        StateConfigurer.Create(_Evade)
            .SetTransition(BoidStates.ALIGNMENT, _Alignment)
            .SetTransition(BoidStates.DIE, _Death) .Done();

        StateConfigurer.Create(_Arrive)
            .SetTransition(BoidStates.EVADE, _Evade)
            .SetTransition(BoidStates.ALIGNMENT, _Alignment)
            .SetTransition(BoidStates.DIE, _Death) .Done();

        StateConfigurer.Create(_Death);

        _Alignment.OnEnter += x => { DoTransformRotationYWithRandomValue(); };
        _Alignment.OnFixedUpdate += () => 
        {
            //AddForce(Alignment() * alignmentWeight);
            _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * maxSpeed * Time.fixedDeltaTime);
            CountTimerMoving();

            if(_gameManager.allFoxes.Count > 0)
            {
                for (int i = 0; i < _gameManager.allFoxes.Count; i++)
                {
                    if (Vector3.Distance(transform.position, _gameManager.allFoxes[i].transform.position) <= viewRadius)
                    {
                        //Debug.Log("Allignment to Evade");

                        _selectedAgent = _gameManager.allFoxes[i];

                        SentToFSM(BoidStates.EVADE);

                        //Debug.Log("Allignment to Evade IS DONE");
                    }
                    else if ((GameManager.instance.food.transform.position - transform.position).magnitude <= arriveRadius)
                        SentToFSM(BoidStates.ARRIVE);
                }
                for (int i = 0; i < FlowerManager.instance.BeeTotal.Count; i++)
                {
                    if (Vector3.Distance(transform.position, FlowerManager.instance.BeeTotal[i].transform.position) > viewRadius)
                        SentToFSM(BoidStates.ALIGNMENT);
                }
            }
                
            if(isAlive == false) SentToFSM(BoidStates.DIE);
        };

        _Evade.OnEnter += x => {
            //Debug.Log("ASUSTADO");
            particleScared.Play(); };
        _Evade.OnFixedUpdate += () => 
        {
            AddForce(Evade() * evadeWeight);
            
            if(_gameManager.allFoxes.Count > 0)
            {
                for (int i = 0; i < _gameManager.allFoxes.Count; i++)
                {
                    if (Vector3.Distance(transform.position, _gameManager.allFoxes[i].transform.position) > viewRadius)
                        SentToFSM(BoidStates.ALIGNMENT);
                }
            }

            if (isAlive == false) SentToFSM(BoidStates.DIE);
        };
        _Evade.OnExit += x => { particleScared.Stop(); };

        _Arrive.OnEnter += x => { particleHungry.Play(); };
        _Arrive.OnFixedUpdate += () =>
        {
            AddForce(Arrive(GameManager.instance.food) * arriveWeight);

            if(_gameManager.allFoxes.Count > 0)
            {
                for (int i = 0; i < _gameManager.allFoxes.Count; i++)
                {
                    if (Vector3.Distance(transform.position, _gameManager.allFoxes[i].transform.position) <= viewRadius)
                    {
                        //Debug.Log("Arrive to Evade");

                        _selectedAgent = _gameManager.allFoxes[i];

                        //SentToFSM(BoidStates.EVADE);

                        //Debug.Log("Arrive to Evade IS DONE");
                    }
                    else if ((GameManager.instance.food.transform.position - transform.position).magnitude > arriveRadius)
                        SentToFSM(BoidStates.ALIGNMENT);
                }
            }

            if (isAlive == false) SentToFSM(BoidStates.DIE);
        };
        _Arrive.OnExit += x => { particleHungry.Stop(); };

        _Death.OnEnter += x => 
        {
            GameManager.instance.RemoveBoid(this);
            Destroy(gameObject);
        };

       _MyFSM = new EventFSM<BoidStates>(_Alignment);
    }
    private void Start()
    {
        GameManager.instance.AddBoid(this);
        isAlive = true;
        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        randomDir.Normalize();
        randomDir *= maxSpeed;
        AddForce(randomDir);
        //CAMBIO JULI--
        SpatialGrid spatialGrid = FindObjectOfType<SpatialGrid>();

        if (spatialGrid != null)
        {
            targetGrid = spatialGrid;
            spatialGrid.UpdateEntity(this);
            UpdateGrid();
        }
        else
        {
            Debug.LogWarning("No se encontró una instancia de SpatialGrid");
        }
    }   //CAMBIO JULI--

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

    public override void Update()//ESTO ES NUEVO ES EL DESTROY CAMBIO JULI
    {
        //MoveTest();
        UpdateGrid();
        //if(_gameManager.allFoxes.Count > 0)
        //{
        //    for (int i = 0; i < _gameManager.allFoxes.Count; i++)
        //    {
        //        if (Vector3.Distance(transform.position, _gameManager.allFoxes[i].transform.position) <= hunterRadius)
        //        {
        //            //Debug.Log("Yo, oveja, estoy a punto de morir.");
        //
        //            _gameManager.allFoxes[i].UpdateKillStreak(_sheepValue);
        //
        //            //Debug.Log("Yo, oveja, me muero.");
        //
        //            GameManager.instance.RemoveBoid(this);
        //            Destroy(gameObject);
        //            return;
        //        }
        //    }
        //}
    }//CAMBIO JULI

    #region Separation
    Vector3 Separation()
    {
        Vector3 desired = Vector3.zero;
        foreach (Boid boid in GameManager.instance.allBoids)
        {
            Vector3 dist = boid.transform.position - transform.position;
            if (dist.magnitude <= separationRadius)
                desired += dist;
        }
        if (desired == Vector3.zero) return desired; 
        desired *= -1;
        return CalculateSteering(desired);
    }
    #endregion

    #region Alignment

    void ChangeTransformRotationY(float value)
    {
        _newVector3Rotation = new Vector3(_myTransform.rotation.eulerAngles.x, value, _myTransform.rotation.eulerAngles.z);
        _myTransform.localEulerAngles = _newVector3Rotation;
    }
    void DoTransformRotationYWithRandomValue()
    {
        _randomValueForAngle = Random.Range(0, 361);
        ChangeTransformRotationY(_randomValueForAngle);
    }

    void CountTimerMoving()
    {
        if (!_canCountTimeMove)
        {
            if (!_restartMoveTime)
                _restartMoveTime = true;
        }

        if (_timeMove != 0 && _restartMoveTime)
        {
            _timeMove = 0;
            _restartMoveTime = false;
        }

        _timeMove += Time.fixedDeltaTime;

        if (_timeMove >= _timerMove)
        {
            if (_timeMove < _timerToMove)
                SentToFSM(BoidStates.ALIGNMENT);
        }
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
    Vector3 Arrive(Food food)
    {
        Vector3 desired = food.gameObject.transform.position - transform.position;
         float dist = desired.magnitude;
        if (dist <= arriveRadius)
        {
            desired.Normalize();
            desired *= maxSpeed * (dist / arriveRadius);
        }
        if ((food.gameObject.transform.position - transform.position).magnitude <= eatRadius)
            EatFood(food);

        return CalculateSteering(desired);
    #region what
        //if (GameManager.instance.allFoods == null) return Vector3.zero;
        //var closestFood = GameManager.instance.allFoods.Where(x => (x.gameObject.transform.position - transform.position).magnitude <= arriveRadius)
        //    .OrderBy(x => Vector3.Distance(x.transform.position, transform.position)).Take(1).ToList(); //IA 2 P1
        //Debug.Log(closestFood);
        //Foods = closestFood;
        //if (closestFood.Count > 0)
        //{
        #endregion
    }
    #endregion

    Vector3 Evade()
    {
        Vector3 futurePos = _selectedAgent.transform.position + _selectedAgent.GetVelocity();
        Vector3 desired = futurePos + _selectedAgent.transform.position;
        desired.Normalize();
        desired *= _selectedAgent.maxSpeed;
        return CalculateSteering(desired);
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

    void UpdateGrid()//CAMBIO JULI
    {
        SpatialGrid spatialGrid = FindObjectOfType<SpatialGrid>();

        if (spatialGrid != null)
        {
            spatialGrid.UpdateEntity(this);
        }
        else
        {
            Debug.LogWarning("No se encontró una instancia de SpatialGrid");
        }
    }//CAMBIO JULI

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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hunterRadius);

    }

}
