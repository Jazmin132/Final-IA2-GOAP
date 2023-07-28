using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuitState : IState
{
    private Agent _agent;
    private FiniteStateMachine _fsm;

    private Vector3 _velocity;

    public PursuitState(Agent agent, FiniteStateMachine fsm)
    {
        _agent = agent;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        _agent.ChangeColor(Color.red);
        Debug.Log("Entra a Cazar");
    }

    public void OnUpdate()
    {
        _agent.energy -= Time.deltaTime;
        if (_agent.energy <= 0)
        {
            _fsm.ChangeState(AgentStates.IDLE);
        }
        AddForce(Pursuit());
        _agent.transform.position += _velocity * Time.deltaTime;
        _agent.transform.forward = _velocity;
    }

    public void OnExit()
    {
        Debug.Log("Salde de Cazar");
    }

    Vector3 Pursuit()
    {
        foreach (Boid boid in GameManager2.instance.allBoids)
        {
                Vector3 futurePos = boid.transform.position + boid.GetVelocity();
                Vector3 desired = futurePos - _agent.transform.position;
                desired.Normalize();
                desired *= _agent.maxSpeed;
                Vector3 steering = desired - _velocity;
                steering = Vector3.ClampMagnitude(steering, _agent.maxForce);
                return steering;
        }
        return Vector3.zero;
    }

    void AddForce(Vector3 force)
    {
        _velocity += force;

        if (_velocity.magnitude >= _agent.maxSpeed)
        {
            _velocity = _velocity.normalized * _agent.maxSpeed;
        }
    }
    
    public Vector3 GetVelocity()
    {
        return _velocity;
    }

}
