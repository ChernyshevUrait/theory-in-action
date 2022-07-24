using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// INHERITANCE
public class Ball : Resetable
{
    #region Events

    public UnityEvent onFinished;

    public UnityEvent onFallen;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        UnityEvent unityEvent = other.tag switch 
        {  
            "Finish" => onFinished,
            "Respawn" => onFallen,
            _ => null
        };

        if (unityEvent != null)
        {
            unityEvent.Invoke();
        }
    }
}
