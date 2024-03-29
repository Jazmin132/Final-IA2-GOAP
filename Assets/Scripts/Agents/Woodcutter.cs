using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class Woodcutter : MonoBehaviour
{
    [Header("NormalValues")] //Lo siguiente se puede tener normalmente pero usarlo de esta manera es m�s �ptimo
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
    [SerializeField] IEnumerable <TreeScript> _treeList;
    [SerializeField] bool _doOnce;

    [SerializeField] bool _canCountTimeWood, _restartWoodTime;
    [SerializeField] float _timeWood, _timerWood;
    [SerializeField] TreeScript _treeToCut;

    [Header("State")]
    [SerializeField] bool _LookingForTree, _Cut, _LoadWood;
    [SerializeField] Image[] Faces;

    public ParticleSystem particleCutting;

    [SerializeField] UIManager _uIManager;

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
            _uIManager.ShowFace("LookingFor", Faces);
            #region Old_Code
            //_treeList = LevelManager.instance.trees.Where(x => LevelManager.instance.trees.Contains(x));
            ////if (LevelManager.instance.trees.Where(x => LevelManager.instance.trees.Contains(x)).Any())
            //if (_treeList.Any()) //IA2-LINQ
            //{
            //    if (_shortestDistanceToTree != 10000)
            //        _shortestDistanceToTree = 10000;
            //
            //    var FirstTree = _treeList.Where(x => (x.transform.position - _myTransform.position).sqrMagnitude < _shortestDistanceToTree)
            //    .OrderBy(x => (x.transform.position - _myTransform.position).sqrMagnitude).First();
            //    
            //    //Debug.Log(FirstTree + "Primer arbol");
            //    
            //    _treeToGoTo = FirstTree;
            //
            //    //foreach (var tree in _treeList)
            //    //{
            //    //    if ((tree.transform.position - _myTransform.position).sqrMagnitude < _shortestDistanceToTree && tree != null)
            //    //        _shortestDistanceToTree = (tree.transform.position - _myTransform.position).sqrMagnitude;
            //    //        _treeToGoTo = tree;
            //    //}
            //}
            //else
            //{
            //    Debug.Log("No hay algo en la lista");
            //}
            #endregion
        };
        LookTree.OnFixedUpdate += () => //IA2-LINQ
        {
            _treeList = LevelManager.instance.trees.Where(x => LevelManager.instance.trees.Contains(x));
            //if (LevelManager.instance.trees.Where(x => LevelManager.instance.trees.Contains(x)).Any())
            if (_treeList.Any()) //IA2-LINQ
            {
                if (_shortestDistanceToTree != 100000)
                    _shortestDistanceToTree = 100000;

                var FirstTree = _treeList.Where(x => (x.transform.position - _myTransform.position).sqrMagnitude < _shortestDistanceToTree)
                .OrderBy(x => (x.transform.position - _myTransform.position).sqrMagnitude).First();

                //Debug.Log(FirstTree + "Primer arbol");

                _treeToGoTo = FirstTree;

                //foreach (var tree in _treeList)
                //{
                //    if ((tree.transform.position - _myTransform.position).sqrMagnitude < _shortestDistanceToTree && tree != null)
                //        _shortestDistanceToTree = (tree.transform.position - _myTransform.position).sqrMagnitude;
                //        _treeToGoTo = tree;
                //}
            }
            else
            {
               // Debug.Log("No hay algo en la lista");
            }

            if (_treeToGoTo != null)
            {
                _myTransform.LookAt(new Vector3(_treeToGoTo.transform.position.x, 0, _treeToGoTo.transform.position.z));
                _myRgbd.MovePosition(_myTransform.position + _myTransform.forward * _speed * Time.fixedDeltaTime);
            }
            else
                return;
        };
        LookTree.OnExit += x => { _LookingForTree = false; };

        CutTree.OnEnter += x => 
        {
            _uIManager.ShowFace("Cutting", Faces);
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

        LOADWOOD.OnEnter += x => 
        {
            _uIManager.ShowFace("LoadWood", Faces);
            _Cut = true;
        };
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

        #region Code_Test
        //CountScript.instance.WaitCounter(_timerWood);
        //
        //_treeToCut.RemoveWood(_woodToGain);
        //_wood += _woodToGain;
        //
        //if (_wood >= _woodMaxCapacity)
        //{
        //    if (!_woodsObject.activeSelf)
        //        _woodsObject.SetActive(true);
        //
        //    SentToFSM(CutterStates.LoadWood);
        //}
        #endregion

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
            if(_wood < _woodMaxCapacity)
            {
                if (collision.gameObject.GetComponent<TreeScript>())
                {
                    _treeToCut = collision.gameObject.GetComponent<TreeScript>();

                    SentToFSM(CutterStates.CUT);
                }
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
