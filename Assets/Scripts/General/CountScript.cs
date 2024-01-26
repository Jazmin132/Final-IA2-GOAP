using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CountScript : MonoBehaviour
{
    public static CountScript instance;

    [SerializeField] bool _canActivateAgain;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    //private void Start()
    //{
    //    StartCoroutine(WaitCounter(1, "RandomStuff"));
    //}

    private void FixedUpdate()
    {
        //if (_canActivateAgain)
        //{
        //    StartCoroutine(WaitCounter(1, "RandomStuff"));
        //
        //    _canActivateAgain = false;
        //}
    }   

    public IEnumerator WaitCounter(float num, string voidName)
    { 
        //Debug.Log("WaitCounter called");
        
        yield return new WaitForSeconds(num);

        Invoke(voidName, 0);

        _canActivateAgain = true;
    }

    public void RandomStuff()
    {
        //Debug.Log("RandomStuff called");
    }
}
