using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private FiniteStateMachine _fsm;
    private Agent _agent;

    public IdleState(Agent agent, FiniteStateMachine fsm)
    {
        _agent = agent; 
        _fsm = fsm;
    }

    public void OnEnter()
    {
        Debug.Log("Entra a Idle/Descanso");
        _agent.ChangeColor(Color.black);
    }

    public void OnUpdate()
    {
        _agent.energy += Time.deltaTime; 
        if(_agent.energy >= _agent.maxEnergy)
        {
            _fsm.ChangeState(AgentStates.PATROL);
        }
    }

    public void OnExit()
    {
        Debug.Log("Sali de Idle/Descanso");
    }
}
