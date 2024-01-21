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

    public IEnumerator WaitCounter(float num)
    {
        yield return new WaitForSeconds(num);
    }
}
