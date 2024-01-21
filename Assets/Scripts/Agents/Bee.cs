using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BeeStates
{
    Moving,
    Spawn,
    Defend,
    DIE
}

public class Bee : MonoBehaviour
{
    //[Header("Flocking")]
    //Agregar el flocking acá los valores que se usan (copiar y pegar lo que se encuentra en el TP-IA2 (o en el Parcial 1 de IA1))

    [Header("NormalValues")] //Lo siguiente se puede tener normalmente pero usarlo de esta manera es más óptimo
    [SerializeField] Transform _myTransform;
    [SerializeField] Rigidbody _myRgbd;

    [Header("Values")]

    [SerializeField] float _randomValueForAngle;
    [SerializeField] Vector3 _newVector3Rotation;
    public Vector3 TargetPlant;

    [Header("ImportantValues")]
    [SerializeField] float _life;
    [SerializeField] float _lifeOriginal, _lifeToLose;

    public float speed, speedOriginal;
    [SerializeField] float _speedMultiplier, _speedMultiplierOriginal;

    [SerializeField] float _jumpForce, _jumpForceOriginal;

    [SerializeField] bool _canCountTimeLife, _restartLifeTime;
    [SerializeField] float _timeLife, _timerLife;

    [SerializeField] bool _canSpawnObject;
    [SerializeField] Transform _spawnerPos;
    [SerializeField] GameObject _objectToSpawn;
    [SerializeField] bool _canCountTimeSpawner, _restartSpawnerTime;
    [SerializeField] float _timeSpawner, _timerSpawner;

    [SerializeField] bool _canCountTimeFlying, _restartFlyingTime;
    [SerializeField] float _timeFlying, _timerFlying;

    [SerializeField] Vector3 _vector3ToAddForce;

    public ParticleSystem particleDeath;
    public ParticleSystem particleAnger;

    [SerializeField] GameObject _particleDeathObject;

    Vector3 _velocity;

    //[Header("State")] //Uso 'bool' en vez de 'estados', reemplazar los 'bool' por los estados que va a tener
    public EventFSM<BeeStates> _MyFSM;

    void Awake()
    {
        #region something
        //Agregarlo a una lista antes de que haga algo? -> fijarse si poner código

        //_firstPosToFly = _myTransform.position;
        //_secondPosToFly = new Vector3(_myTransform.position.x, _valueYSecondPosToFly, _myTransform.position.z);


        //SetValueRandom(_randomValueForAngle, 0, 361);

        _randomValueForAngle = Random.Range(0, 361);

        ChangeTransformRotationY(_randomValueForAngle);

        _vector3ToAddForce = Vector3.up;

        _myRgbd.AddForce(_vector3ToAddForce * _jumpForce * Time.fixedDeltaTime, ForceMode.Impulse);
        #endregion

        var Moving = new State<BeeStates>("Moving");
        var SpawnPlant = new State<BeeStates>("SpawnPlant");
        var GotoPlant = new State<BeeStates>("GotoPlant");
        var Death = new State<BeeStates>("Death");

        StateConfigurer.Create(Moving)
            .SetTransition(BeeStates.Spawn, SpawnPlant)
            .SetTransition(BeeStates.Defend, GotoPlant)
            .SetTransition(BeeStates.DIE, Death).Done();

        StateConfigurer.Create(SpawnPlant)
            .SetTransition(BeeStates.Moving, Moving)
            .SetTransition(BeeStates.Defend, GotoPlant)
            .SetTransition(BeeStates.DIE, Death).Done();

        StateConfigurer.Create(GotoPlant)
        .SetTransition(BeeStates.Moving, Moving)
        .SetTransition(BeeStates.Spawn, SpawnPlant)
        .SetTransition(BeeStates.DIE, Death).Done();

        StateConfigurer.Create(Death).Done();

        Moving.OnFixedUpdate += () =>
        {
            if (_life > 0)
            {
                CountTimerLife();
                CountTimerSpawner();

                _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * speed * Time.fixedDeltaTime);

                if (_myRgbd.useGravity)
                    _myRgbd.useGravity = false;
                CountTimerFlying();
            }
            else
                SentToFSM(BeeStates.DIE);
        };
        SpawnPlant.OnFixedUpdate += () =>
         {//SPAWN PLANT
             if (!_myRgbd.useGravity)
                 _myRgbd.useGravity = true;

             if (_canCountTimeFlying)
                 _canCountTimeFlying = false;

             if (!_canSpawnObject)
                 _canSpawnObject = true;
         };
        GotoPlant.OnEnter += x => 
        {
            Debug.Log("MUST Go To PLANT");

            particleAnger.Play();
        };
        GotoPlant.OnFixedUpdate += () =>
        {
            Vector3 dir = TargetPlant - transform.position;
            dir.y = 0;

            if (dir.magnitude >= 0.3f)
            {
                transform.forward = dir;
                transform.position += transform.forward * (speed + _speedMultiplier) * Time.fixedDeltaTime;
            }
            else SentToFSM(BeeStates.Moving);
        };
        GotoPlant.OnExit += x => 
        { 
            Debug.Log("STOP PROTECTING PLANT");
            particleAnger.Stop();
        };

        Death.OnEnter += x => 
        {
            particleDeath.Play();

            GameObject effect = Instantiate(_particleDeathObject, transform.position, Quaternion.identity);
            Destroy(effect, 3f);

            FlowerManager.instance.RemoveBee(this);
            Destroy(gameObject); 
        };

        _MyFSM = new EventFSM<BeeStates>(Moving);
    }

    void Start()
    {
        FlowerManager.instance.AddBee(this);
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
            if(!_restartLifeTime)
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

    void CountTimerSpawner()
    {
        if (!_canCountTimeSpawner)
        {
            if(!_restartSpawnerTime)
                _restartSpawnerTime = true;
        }

        if (_timeSpawner != 0 && _restartSpawnerTime)
        {
            _timeSpawner = 0;
            _restartSpawnerTime = false;
        }

        _timeSpawner += Time.fixedDeltaTime;

        if (_timeSpawner >= _timerSpawner)
        {
            SentToFSM(BeeStates.Spawn);
            _timeSpawner = 0;
        }
    }

    void CountTimerFlying()
    {
        if (!_canCountTimeFlying)
        {
            if(!_restartFlyingTime)
                _restartFlyingTime = true;

            return;
        }

        if (_timeFlying != 0 && _restartFlyingTime)
        {
            _timeFlying = 0;

            _restartFlyingTime = false;
        }

        _timeFlying += Time.fixedDeltaTime;

        if (_timeFlying >= _timerFlying)
        {
            if (_vector3ToAddForce == Vector3.up)
                _vector3ToAddForce = Vector3.down;
            else
                _vector3ToAddForce = Vector3.up;

            _jumpForce += _jumpForceOriginal;

            _myRgbd.AddForce(_vector3ToAddForce * _jumpForce * Time.fixedDeltaTime, ForceMode.Impulse);
            //_firstPosToFly = _myTransform.position;
            //_secondPosToFly = new Vector3(_myTransform.position.x, _valueYFirstPosToFly, _myTransform.position.z);
            _timeFlying = 0;
        }
    }
    #endregion
    public void SentToFSM(BeeStates states)
    {
        _MyFSM.SendInput(states);
    }

    void ChangeTransformRotationY(float value)
    {
        _newVector3Rotation = new Vector3(_myTransform.rotation.eulerAngles.x, value, _myTransform.rotation.eulerAngles.z);

        _myTransform.localEulerAngles = _newVector3Rotation;
    }

    void SpawnObject(GameObject objectToSpawn)
    {
        Instantiate(objectToSpawn, _spawnerPos.position, _spawnerPos.rotation);
    }

    void AddForce(Vector3 force)
    {
        _velocity += force;

        if (_velocity.magnitude >= speed)
            _velocity = _velocity.normalized * speed;
    }

    public Vector3 GetVelocity()
    {
        return _velocity;
    }

    #region States

    void SpawnPlant()
    {
        SpawnObject(_objectToSpawn);
        _randomValueForAngle = Random.Range(0, 361);

        ChangeTransformRotationY(_randomValueForAngle);

        _jumpForceOriginal = 100;

        _jumpForce = _jumpForceOriginal;
        _canCountTimeFlying = true;
        _canSpawnObject = false;
        SentToFSM(BeeStates.Moving);
    }
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (_canSpawnObject)
            SpawnPlant();
    }
    //Agregar el flocking acá lo que va afuera del FixedUpdate (copiar y pegar lo que se encuentra en el TP-IA2 (o en el Parcial 1 de IA1))
}