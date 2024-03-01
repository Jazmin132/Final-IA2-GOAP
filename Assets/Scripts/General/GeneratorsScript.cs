using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorsScript : MonoBehaviour
{
    public IEnumerable<T> IEnumerableCollectionCreator<T>(List<T> Coll1, int maxNumNewCollection) //IA2-LINQ
    {
        //foreach (var item in Coll1)
        //{
        //    yield return item;
        //}

        for (int i = 0; i < maxNumNewCollection; i++)
        {
            //Debug.Log(Coll1[i]);

            yield return Coll1[i];
        }

        //Debug.Log("Generator Activated");
    }
}
