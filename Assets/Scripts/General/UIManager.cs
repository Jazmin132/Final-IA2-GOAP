using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class UIManager : MonoBehaviour
{
    public Text[] TextCount;

    public Image[] FacesFoxes1;
    public Image[] FacesFoxes2;
    public Image[] FacesFoxes3;

    public Agent[] Foxes;

    int _SheepCount;
    int _BeeCount;

    public static UIManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        _SheepCount = GameManager.instance.allBoids.Count();
        _BeeCount = FlowerManager.instance.BeeTotal.Count();

        TextCount[0].text = "X "+ _SheepCount;

        TextCount[1].text = "X " + _BeeCount;
    }

    public void ChangeText(int newvalue, string forwho)
    {
        if (forwho == "sheep")
        {
            _SheepCount += newvalue;
            TextCount[0].text = "X " + _SheepCount;
        }
        else if (forwho == "bee")
        {
            _BeeCount += newvalue;
            TextCount[1].text = "X " + _BeeCount;
        }
    }

    public void AssignFoxesFaces(Agent fox, string name)
    {
        //Tuple<GameObject[], Agent > Hi = Tuple.Create(FacesFoxes1, Foxes[0]);
        if (fox == Foxes[0]) 
            ShowFace(name, FacesFoxes1);
        else if (fox == Foxes[1])
            ShowFace(name, FacesFoxes2);
        else 
            ShowFace(name, FacesFoxes3);
    }

    public void ShowFace(string name, Image[] Foxfaces)
    {
        foreach (var face in Foxfaces)
        {
            if (name == face.name) 
                face.gameObject.SetActive(true);
            else 
                face.gameObject.SetActive(false);
        }
    }
}
