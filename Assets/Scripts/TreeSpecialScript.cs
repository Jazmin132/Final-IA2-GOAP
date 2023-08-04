using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpecialScript : MonoBehaviour
{
    public GameObject fullTree;
    public GameObject chopTree;
    public bool choped;
    // Start is called before the first frame update
    void Start()
    {
        fullTree.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (choped)
        {
            fullTree.SetActive(false);
            chopTree.SetActive(true);
        }
    }
}
