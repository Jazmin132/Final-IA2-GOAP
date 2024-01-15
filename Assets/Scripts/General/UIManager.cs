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

    public Image[] Crowns;

    public Agent[] Foxes;

    int _SheepCount;
    int _BeeCount;
    int biggest = 0;
    int latestsN;

    public static UIManager instance;

    public Text[] textFoxKills;
    public int[] foxKillsNum;

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

    public void UpdateFoxKillStreak(Agent fox, int value)
    {
        Tuple<Agent[], int[]> tupla = Tuple.Create(Foxes, foxKillsNum);

        for (int i = 0; i < Foxes.Length; i++)
        {
            if (fox == tupla.Item1[i])
            {
                foxKillsNum[i] += value;
                //latestsN = foxKillsNum[i];

                Debug.Log("New Biggest: " + biggest);
                Debug.Log("New latestsN: " + foxKillsNum[i]);
                Debug.Log(foxKillsNum[i] >= biggest);
                if (foxKillsNum[i] >= biggest)
                {
                    biggest = foxKillsNum[i];
                    Crowns[i].gameObject.SetActive(true);
                }
                else if (foxKillsNum[i] < biggest)
                {
                    Crowns[i].gameObject.SetActive(false);
                    Debug.Log("Else Biggest: " + biggest);
                    Debug.Log("Else latestsN: " + foxKillsNum[i]);
                }
                Debug.Log(i + " foxKill: " + foxKillsNum[i]);
                //Actualizo el la tupla con las nuevas kills
                tupla = Tuple.Create(tupla.Item1, foxKillsNum);
                //Actualizo el texto del zorro
                textFoxKills[i].text = "X " + tupla.Item2[i];
            }
        }
    }
}
