using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "noiseSettings", menuName = "Data/NoiseSettings")]

public class NoiseSettings : ScriptableObject
{
    public Vector3 noiseZoom = new Vector3(0.01f, 0.01f, 0.01f);
    
    [FormerlySerializedAs("perlinOffset")]
    public Vector3Int localOffset;
    public Vector3Int worldOffset;
    public float amplitudeMultiplyer = 0.5f;
    public float frequencyMultyplier = 2f;
    public uint octaves = 5;
    public float exponent = 1f;
    public float redistributionModifier = 1f;    
    public Vector3Int minDimension = new Vector3Int(0, -64, 0);
    public Vector3Int maxDimension = new Vector3Int(1, 64, 1);
}
