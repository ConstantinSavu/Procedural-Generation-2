using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class DebugTest : MonoBehaviour
{
    public GameObject Parent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        NavMeshModifierVolume bla = Parent.GetComponent<NavMeshModifierVolume>();
        Debug.Log(bla.area);
        
    }
}
