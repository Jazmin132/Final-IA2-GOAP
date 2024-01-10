using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public Text[] TextMeshPro;
    int _SheepCount;
    int _BeeCount;

    private void Awake()
    {
        GameManager.instance.UI = this;
        FlowerManager.instance.UI = this;
    }

    private void Start()
    {
        _SheepCount = GameManager.instance.allBoids.Count();
        _BeeCount = FlowerManager.instance.BeeTotal.Count();

        TextMeshPro[0].text = "X "+ _SheepCount;
        Debug.Log("Sheep number "+ _SheepCount);

        TextMeshPro[1].text = "X " + _BeeCount;
        Debug.Log("Bee number "+ _BeeCount);
    }
    public void ChangeText(int newvalue, string forwho)
    {
        if (forwho == "sheep")
        {
            _SheepCount += newvalue;
            TextMeshPro[0].text = "X " + _SheepCount;
        }
        else if (forwho == "bee")
        {
            _BeeCount += newvalue;
            TextMeshPro[1].text = "X " + _BeeCount;
        }
    }
}
