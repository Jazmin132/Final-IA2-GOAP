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
    int biggest = 1;

    public static UIManager instance;

    public Text[] textFoxKills;
    public int[] foxKillsNum;

    public Text[] foodTexts;

    public Text AllFoodText;

    [SerializeField] GameManager _gameManager;

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
            {//Cuando la kill es del zorro que recibí
                foxKillsNum[i] += value;

                //Actualizo la tupla con las nuevas kills
                tupla = Tuple.Create(tupla.Item1, foxKillsNum);
                //Actualizo el texto del zorro
                textFoxKills[i].text = "X " + tupla.Item2[i];
            }

            if (foxKillsNum[i] >= biggest)
            {//Si la kill de este zorro es mayor o igual al número más grande de Kills  
                biggest = foxKillsNum[i];//Actualizo el número mas grande de Kills
                Crowns[i].gameObject.SetActive(true);//Ativo la corona correspondiente
            }
            else
                Crowns[i].gameObject.SetActive(false);
        }
    }
}
