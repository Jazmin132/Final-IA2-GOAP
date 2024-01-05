using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerManager : MonoBehaviour
{
    List<Bee> _BeeTotal = new List<Bee>();
    List<Plant> _PlantTotal = new List<Plant>();

    public static FlowerManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    public void AddBee(Bee b)
    {
        if (!_BeeTotal.Contains(b)) _BeeTotal.Add(b);
    }
    public void RemoveBee(Bee b)
    {
        if (_BeeTotal != null) _BeeTotal.Remove(b);
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
        for (int i = 0; i < _BeeTotal.Count; i++)
        {
            _BeeTotal[i].TargetPlant = plant.transform.position;
            _BeeTotal[i].SentToFSM(BeeStates.Defend);
        }
    }
}
