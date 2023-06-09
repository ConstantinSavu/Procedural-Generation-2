using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPauseCanvas : MonoBehaviour
{

    [SerializeField] GameObject MainMenu;
    void OnEnable()
    {
        MainMenu.SetActive(true);
    }

    void OnDisable()
    {
        foreach( Transform child in transform )
        {
            child.gameObject.SetActive(false);
        }
    }
}
