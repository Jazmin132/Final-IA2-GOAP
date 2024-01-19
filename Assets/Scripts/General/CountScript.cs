using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountScript : MonoBehaviour
{
    public static CountScript instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public IEnumerator WaitCounter(int num)
    {
        yield return new WaitForSeconds(num);
    }
}
