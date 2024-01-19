using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class UIManager : MonoBehaviour
{
    public Text[] TextCount;
    public string[] FoxesNames;
    public string[] foxesNamesAndKills;

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

    public int AllFoodValue, AllFruitsValue, AllLegumesValue, AllApplesValue, AllCoconutsValue, AllBeansValue;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        _SheepCount = GameManager.instance.allBoids.Count();
        _BeeCount = FlowerManager.instance.BeeTotal.Count();

        TextCount[0].text = _SheepCount.ToString();

        TextCount[1].text = _BeeCount.ToString();

        foxesNamesAndKills = FoxesNames.Zip(foxKillsNum, (FN, FA) => FN + " " + FA).ToArray(); //IA-P2

        for (int i = 0; i < Foxes.Length; i++)
        {
            textFoxKills[i].text = foxesNamesAndKills[i];
        }
    }

    public void ChangeText(int newvalue, string forwho)
    {
        if (forwho == "sheep")
        {
            _SheepCount += newvalue;
            TextCount[0].text = _SheepCount.ToString();
        }
        else if (forwho == "bee")
        {
            _BeeCount += newvalue;
            TextCount[1].text = _BeeCount.ToString();
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
        //Tuple<Agent[], int[]> tupla = Tuple.Create(Foxes, foxKillsNum);
        for (int i = 0; i < Foxes.Length; i++)
        {
            //if (fox == tupla.Item1[i])
            if (fox == Foxes[i])
            {//Cuando la kill es del zorro que recibí
                foxKillsNum[i] += value;
                /*
                Actualizo la tupla con las nuevas kills
                tupla = Tuple.Create(tupla.Item1, foxKillsNum);
                Actualizo el texto del zorro
                textFoxKills[i].text = "X " + tupla.Item2[i];
                */
                foxesNamesAndKills = FoxesNames.Zip(foxKillsNum, (FN, FA) => FN + " " + FA).ToArray(); //IA-P2
                textFoxKills[i].text = foxesNamesAndKills[i];
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

    public void UpdateFoodValue()
    {
        if(_gameManager.allFood.Count != AllFoodValue)
        {
            AllFoodValue = _gameManager.allFood.Count;

            AllFoodText.text = AllFoodValue.ToString();
        }

        if(_gameManager.allApples.Count != AllApplesValue)
        {
            AllApplesValue = _gameManager.allApples.Count;

            foodTexts[0].text = AllApplesValue.ToString();
        }

        if (_gameManager.allCoconuts.Count != AllCoconutsValue)
        {
            AllCoconutsValue = _gameManager.allCoconuts.Count;

            foodTexts[1].text = AllCoconutsValue.ToString();
        }

        if (_gameManager.allBeans.Count != AllBeansValue)
        {
            AllBeansValue = _gameManager.allBeans.Count;

            foodTexts[2].text = AllBeansValue.ToString();
        }
    }
}
