using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using IA2;

public enum ActionJ
{
    Kill,
    GetMoney,
    NextStep,
    FailedStep,
    Sobornar,
    Success
}
public class Julianicky : MonoBehaviour
{
    private EventFSM<ActionJ> _fsm;
    private Item _target;

    private NewEntity _ent;
    IEnumerable<Tuple<ActionJ, Item>> _plan;

    private void UsarBate(Entity us, Item other, bool NotTired)
    {
        Debug.Log("PerformAttack", other.gameObject);
        if (other != _target) return;
        //var mace = _ent.items.FirstOrDefault(it => it.type == ItemType.Mace);
        if (NotTired)
        {
            other.Kill();
            //if (other.type == ItemType.Door)
            //Destroy(_ent.Removeitem(mace).gameObject);Destruir Enemy
            _fsm.Feed(ActionJ.NextStep);
        }
        else
            _fsm.Feed(ActionJ.FailedStep);
    }
    private void GetMoney(Entity us, Item other, bool IsBroke)
    {
        if (other != _target) return;
        var Mochila = other.GetComponent<Door>();
        //Hacer el Component a mochila, cuando ya esté programado
        if (IsBroke && Mochila)
        {
            _fsm.Feed(ActionJ.NextStep);
        }
        else
            _fsm.Feed(ActionJ.FailedStep);
    }
    private void Sobornar(Entity us, Item other, bool HasMoney, bool IsEnemyAlive)
    {
        if (other != _target) return;
    
        // var key = _ent.items.FirstOrDefault(it => it.type == ItemType.Key);
        var door = other.GetComponent<Door>();
        //Cambiar para que le de el dinero al mafioso
        if (HasMoney && IsEnemyAlive)
        {
            door.Open();
            // Destroy(_ent.Removeitem(key).gameObject);
            _fsm.Feed(ActionJ.NextStep);
        }
        else
            _fsm.Feed(ActionJ.FailedStep);
    }
    private void NextStep(Entity ent, Waypoint wp, bool reached)
    {
        _fsm.Feed(ActionJ.NextStep);
    }
    void Awake()
    {
        _ent = GetComponent<NewEntity>();

        var any = new State<ActionJ>("any");
        var idle = new State<ActionJ>("idle");

        var bridgeStep = new State<ActionJ>("planStep");
        var failStep = new State<ActionJ>("failStep");
        var success = new State<ActionJ>("success");

        var kill = new State<ActionJ>("kill");
        var soborno = new State<ActionJ>("Sobornar");
        var money = new State<ActionJ>("GetMoney");

        kill.OnEnter += a => {
            _ent.GoTo(_target.transform.position);
            _ent.UsarBate += UsarBate;
        }; 
        kill.OnExit += a => _ent.UsarBate -= UsarBate;
        failStep.OnEnter += a => { _ent.Stop(); Debug.Log("Plan failed"); };

        StateConfigurer.Create(any)
            .SetTransition(ActionJ.NextStep, bridgeStep)
            .SetTransition(ActionJ.FailedStep, idle)
            .Done();

        StateConfigurer.Create(bridgeStep)
            .SetTransition(ActionJ.Kill, kill)
            .SetTransition(ActionJ.Success, success)
            .Done();
    }

    public void ExecutePlan(List<Tuple<ActionJ, Item>> plan)
    {
        _plan = plan;
        _fsm.Feed(ActionJ.NextStep);
    }

    void Update()
    {
        _fsm.Update();
    }
}
