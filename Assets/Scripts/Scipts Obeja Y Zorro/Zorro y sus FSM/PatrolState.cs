using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{

    private Agent _agent;
    private FiniteStateMachine _fsm;
    private int _currentWaypoint;

    public PatrolState(Agent agent, FiniteStateMachine fsm)
    {
        _agent = agent;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        Debug.Log("Entra a Patrullar");
        _agent.ChangeColor(Color.green);
    }

    public void OnUpdate()
    {
        _agent.energy -= Time.deltaTime;
        if(_agent.energy <= 0)_fsm.ChangeState(AgentStates.IDLE);
        Patrol();
        foreach (Boid boid in GameManager2.instance.allBoids)
        {
            Vector3 dist = boid.transform.position - _agent.transform.position;
            if (dist.magnitude <= _agent.pursuitRadius) _fsm.ChangeState(AgentStates.PURSUIT);
        }
    }

    void Patrol()
    {
        GameObject waypoint = _agent.allWaypoints[_currentWaypoint];
        Vector3 dir = waypoint.transform.position - _agent.transform.position;
        dir.y = 0;
        _agent.transform.forward = dir;
        _agent.transform.position += _agent.transform.forward * _agent.speed * Time.deltaTime;
        if (dir.magnitude <= 0.3f)
        {
            _currentWaypoint++;
            if (_currentWaypoint >= _agent.allWaypoints.Length)
            {
                _currentWaypoint = 0;
            }
        }
    }

    public void OnExit()
    {
        Debug.Log("Sale de Patrullar");
    }
}
