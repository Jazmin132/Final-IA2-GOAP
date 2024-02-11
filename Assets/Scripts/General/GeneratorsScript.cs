using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorsScript : MonoBehaviour
{
    public IEnumerable<T> IEnumerableCollectionCreator<T>(List<T> Coll1) //IA2-LINQ
    {
        foreach (var item in Coll1)
        {
            yield return item;
        }
    }
}
