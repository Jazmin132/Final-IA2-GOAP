using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Woodcutter : MonoBehaviour
{
    [Header("NormalValues")] //Lo siguiente se puede tener normalmente pero usarlo de esta manera es más óptimo
    [SerializeField] Transform _myTransform;
    [SerializeField] Rigidbody _myRgbd;

    [Header("ImportantValues")]
    [SerializeField] float _wood;
    [SerializeField] float _woodMaxCapacity, _woodToGain;
    [SerializeField] Store _storeToLoad;
    [SerializeField] GameObject _woodsObject;

    [SerializeField] float _speed, _speedOriginal;

    [SerializeField] float _shortestDistanceToTree;
    [SerializeField] TreeScript _treeToGoTo;
    [SerializeField] bool _doOnce;

    [SerializeField] bool _canCountTimeWood, _restartWoodTime;
    [SerializeField] float _timeWood, _timerWood;
    [SerializeField] TreeScript _treeToCut;

    [Header("State")]
    [SerializeField] bool _LookingForTree, _Cut, _LoadWood;

    public ParticleSystem particleCutting;

    public enum CutterStates
    {
        LookingForTree,
        CUT,
        LoadWood
    }
    public EventFSM<CutterStates> _MyFSM;
    void Awake()
    {
    #region States & Transitions
        var LookTree = new State<CutterStates>("LookTree");
        var CutTree = new State<CutterStates>("CUT");
        var LOADWOOD = new State<CutterStates>("LoadWood");

        StateConfigurer.Create(LookTree)
           .SetTransition(CutterStates.CUT, CutTree)
           .SetTransition(CutterStates.LoadWood, LOADWOOD).Done();

        StateConfigurer.Create(CutTree)
            .SetTransition(CutterStates.LookingForTree, LookTree)
            .SetTransition(CutterStates.LoadWood, LOADWOOD).Done();

        StateConfigurer.Create(LOADWOOD)
           .SetTransition(CutterStates.CUT, CutTree)
            .SetTransition(CutterStates.LookingForTree, LookTree).Done();
        #endregion

        LookTree.OnEnter += x =>
        {
            _LookingForTree = true;
            if (LevelManager.instance.trees.Count > 0)
            {
                if (_shortestDistanceToTree != 10000)
                    _shortestDistanceToTree = 10000;

                foreach (var tree in LevelManager.instance.trees)
                {
                    if ((tree.transform.position - _myTransform.position).sqrMagnitude < _shortestDistanceToTree && tree != null)
                    {
                        _shortestDistanceToTree = (tree.transform.position - _myTransform.position).sqrMagnitude;
                    
                        _treeToGoTo = tree;
                    }
                    
                } 
            }
        };
        LookTree.OnFixedUpdate += () => 
        {
            _myTransform.LookAt(new Vector3(_treeToGoTo.transform.position.x, 0, _treeToGoTo.transform.position.z));
            _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
        };
        LookTree.OnExit += x => { _LookingForTree = false; };

        CutTree.OnEnter += x => 
        {
            particleCutting.Play();
            _Cut = true;
        };
        CutTree.OnFixedUpdate += () =>
        { 
            if (_treeToCut == null) 
                SentToFSM(CutterStates.LookingForTree);
            else
                CountTimerWood();
        };
        CutTree.OnExit += x => 
        { 
            _Cut = false;
            particleCutting.Stop();
        };

        LOADWOOD.OnEnter += x => { _Cut = true; };
        LOADWOOD.OnFixedUpdate += () => 
        {
            _myTransform.LookAt(new Vector3(_storeToLoad.transform.position.x, 0, _storeToLoad.transform.position.z));
            _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
        };
        LOADWOOD.OnExit += x => { _Cut = false; };

        _MyFSM = new EventFSM<CutterStates>(LookTree);
    }

    void FixedUpdate()
    {
        _MyFSM.FixedUpdate();
    }

    #region CountTimer
    void CountTimerWood()
    {
        if (!_canCountTimeWood)
        {
            if (!_restartWoodTime)
                _restartWoodTime = true;
        }
        if (_timeWood != 0 && _restartWoodTime)
        {
            _timeWood = 0;
            _restartWoodTime = false;
        }

        _timeWood += Time.fixedDeltaTime;

        if (_timeWood >= _timerWood)
        {
            _treeToCut.RemoveWood(_woodToGain);
            _wood += _woodToGain;

            if (_wood >= _woodMaxCapacity)
            {
                if (!_woodsObject.activeSelf)
                    _woodsObject.SetActive(true);

                SentToFSM(CutterStates.LoadWood);
            }

            _timeWood = 0;
        }
    }
    #endregion

    void SentToFSM(CutterStates states)
    {
        _MyFSM.SendInput(states);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 7)
        {
            if (collision.gameObject.GetComponent<TreeScript>())
            {
                _treeToCut = collision.gameObject.GetComponent<TreeScript>();

                SentToFSM(CutterStates.CUT);
            }
        }
        else if (collision.gameObject.layer == 8)
        {
            _storeToLoad.AddWood(_wood);

            _wood = 0;

            if (_woodsObject.activeSelf)
                _woodsObject.SetActive(false);

            SentToFSM(CutterStates.LookingForTree);
        }
    }
}
