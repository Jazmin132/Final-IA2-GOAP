using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntiddadEjemplo : MonoBehaviour
{
    public enum EntityStates
    {
        Idle,
        Accion1,
        Accion2
    }
    public EventFSM<EntityStates> _MyFSM;

    void Awake()
    {
        //Crear los estados
        var idle = new State<EntityStates>("Idle");
        var accion1 = new State<EntityStates>("Accion1");
        var accion2 = new State<EntityStates>("Accion2");
        //Crear las transiciones
        StateConfigurer.Create(idle)
            .SetTransition(EntityStates.Accion1, accion1)
            .SetTransition(EntityStates.Accion2, accion2);

        StateConfigurer.Create(accion1)
            .SetTransition(EntityStates.Idle, idle)
            .SetTransition(EntityStates.Accion2, accion2);

        StateConfigurer.Create(accion2)
            .SetTransition(EntityStates.Idle, idle)
            .SetTransition(EntityStates.Accion1, accion1);

        //Para editar que pasa en cada estado
        idle.OnEnter += x =>
        {
            Debug.Log("En enter");
        };
        idle.OnUpdate += () =>
        {
            bool Ejemplo = true;

            if (Ejemplo) //Para trancicionar de un estado a otro
                SentToFSM(EntityStates.Accion1);
            else
                SentToFSM(EntityStates.Accion2);
        };
        //Indicás en que estado arranca al poner el play
        _MyFSM = new EventFSM<EntityStates>(idle);
    }
    void SentToFSM(EntityStates states)
    {//Solo para tener un atajo al cambiar de estado
        _MyFSM.SendInput(states);
    }
    private void Update()
    {
        _MyFSM.Update();
    }//Para que funcione el Update

    private void FixedUpdate()
    {//Para que funcione el FixedUpdate
     //no es necesario ponerlo si no lo vas a usar
        _MyFSM.FixedUpdate();
    }
}
