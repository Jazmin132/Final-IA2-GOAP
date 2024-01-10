using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerManager : MonoBehaviour
{
    public List<Bee> BeeTotal = new List<Bee>();
    List<Plant> _PlantTotal = new List<Plant>();
    public UIManager UI;

    public static FlowerManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void AddBee(Bee b)
    {
        if (!BeeTotal.Contains(b)) BeeTotal.Add(b);

        UI.ChangeText(1, "bee");
    }

    public void RemoveBee(Bee b)
    {
        if (BeeTotal != null) BeeTotal.Remove(b);

        UI.ChangeText(-1, "bee");
    }

    public void AddFlower(Plant p)
    {
        if (!_PlantTotal.Contains(p)) _PlantTotal.Add(p);
    }

    public void RemoveFlower(Plant p)
    {
        if (_PlantTotal != null) _PlantTotal.Remove(p);
    }

    public void CallBees(Plant plant)//IA2 LINQ
    {
        for (int i = 0; i < BeeTotal.Count; i++)
        {
            BeeTotal[i].TargetPlant = plant.transform.position;
            BeeTotal[i].SentToFSM(BeeStates.Defend);
        }
    }
}
