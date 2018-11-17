using System;
using UnityEngine;

public class HookBehaviour : MonoBehaviour {

    Tether m_tetherInstance;

    public void SetTetherInstance(Tether instance)
    {
        m_tetherInstance = instance;
    }

    private void OnTriggerEnter(Collider collision)
    {
        m_tetherInstance.OnCollision(collision);
    }
}
