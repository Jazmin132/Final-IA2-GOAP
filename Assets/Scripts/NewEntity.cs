using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace IA2
{
    public class NewEntity : MonoBehaviour
    {
        public event Action<Entity, Item, bool> UsarBate = delegate { };
        public event Action<Entity, Item> OnHitItem = delegate { };
        Vector3 _velocity;

        void Start()
        {

        }

        public void GoTo(Vector3 destination)
        {
            //Calcular ir a tal lugar
        }

        public void Stop()
        {
            _velocity = Vector3.zero;
            //Que pare en seco
        }
    }
}
