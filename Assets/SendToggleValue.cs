using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SendToggleValue : MonoBehaviour
{
    private Toggle toggle;

    [SerializeField] UnityEvent<bool> sendToggleValue;

    private void Awake() {
        toggle = GetComponent<Toggle>();
    }
    public void SendToggleState(bool needLessBool){
        if(toggle == null){
            toggle = GetComponent<Toggle>();
        }

        sendToggleValue.Invoke(toggle.isOn);
    }
}
