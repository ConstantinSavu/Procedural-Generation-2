using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimescaleChange : MonoBehaviour
{
    [SerializeField] float timeScale = 1f;

    void OnValidate()
    {
        Time.timeScale = timeScale;
    }
}
