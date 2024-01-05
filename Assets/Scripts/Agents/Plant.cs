using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [Header("NormalValues")] //Lo siguiente se puede tener normalmente pero usarlo de esta manera es más óptimo
    [SerializeField] Transform _myTransform;
    [SerializeField] Rigidbody _myRgbd;

    [Header("Values")]

    [SerializeField] float _viewRadius;
    Bee _BeeCall;
    [SerializeField] float _randomValueForAngle;
    [SerializeField] Vector3 _newVector3Rotation;

    [Header("ImportantValues")]
    [SerializeField] float _life;
    [SerializeField] float _lifeOriginal, _lifeToLose;

    [SerializeField] float _speed, _speedOriginal;

    [SerializeField] bool _canCountTimeLife, _restartLifeTime;
    [SerializeField] float _timeLife, _timerLife;

    [SerializeField] bool _canCountTimeMove, _restartMoveTime;
    [SerializeField] float _timeMove, _timerMove;
    [SerializeField] float _timerToMove;
    [SerializeField] bool _doOnce;

    public ParticleSystem particleDeath;

    [SerializeField] GameObject _particleDeathObject;
    [SerializeField] GameObject _InstanciateTree;

    //[SerializeField] bool _stateIdle, _stateMoving;
    public enum PlantStates
    {
        Idle,
        Move,
        Death
    }
    public EventFSM<PlantStates> _MyFSM;
    void Awake()
    {
        FlowerManager.instance.AddFlower(this);
        //Agregarlo a una lista antes de que haga algo? -> fijarse si poner código
        var Idle = new State<PlantStates>("Idle");
        var Moving = new State<PlantStates>("Move");
        var Death = new State<PlantStates>("DIE");

        StateConfigurer.Create(Idle)
            .SetTransition(PlantStates.Move, Moving)
            .SetTransition(PlantStates.Death, Death).Done();

        StateConfigurer.Create(Moving)
            .SetTransition(PlantStates.Idle, Idle)
            .SetTransition(PlantStates.Death, Death).Done();

        StateConfigurer.Create(Death).Done();

        Idle.OnFixedUpdate += () =>
        {
            if (_life > 0)
            {
                CountTimerLife();
                CountTimerMoving();
            }
            else
                SentToFSM(PlantStates.Death);
        };
        Moving.OnEnter += x =>
        {
            DoTransformRotationYWithRandomValue();
            List<Boid> _Boids = GameManager.instance.allBoids;
            for (int i = 0; i < _Boids.Count; i++)
            {
                if (Vector3.Distance(transform.position,_Boids[i].transform.position) <= _viewRadius)
                {
                    
                }
            }
        };
        Moving.OnFixedUpdate += () =>
        {
            if (_life > 0)
            {
                CountTimerLife();
                CountTimerMoving();
                _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
            }
            else
                SentToFSM(PlantStates.Death);

        };
        Death.OnEnter += x =>
        {
            particleDeath.Play();

            Instantiate(_InstanciateTree, transform.position, Quaternion.identity);

            GameObject effect = Instantiate(_particleDeathObject, transform.position, Quaternion.identity);
            Destroy(effect, 3f);

            FlowerManager.instance.RemoveFlower(this);
            Destroy(gameObject);
        };
        _MyFSM = new EventFSM<PlantStates>(Idle);
    }

    void FixedUpdate()
    {
        _MyFSM.FixedUpdate();
    }

    #region CountTimer
    void CountTimerLife()
    {
        if (!_canCountTimeLife)
        {
            if (!_restartLifeTime)
                _restartLifeTime = true;
        }
        if (_timeLife != 0 && _restartLifeTime)
        {
            _timeLife = 0;
            _restartLifeTime = false;
        }

        _timeLife += Time.fixedDeltaTime;

        if (_timeLife >= _timerLife)
        {
            _life -= _lifeToLose;
            _timeLife = 0;
        }
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
                SentToFSM(PlantStates.Move);
            else
            {
                SentToFSM(PlantStates.Idle);
                _timeMove = 0;
            }
        }
    }
    #endregion

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
    void SentToFSM(PlantStates states)
    {
        _MyFSM.SendInput(states);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _viewRadius);
    }
}
