using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StructurePositions")]
public class StructurePositionsSO : ScriptableObject
{
    public List<Vector3Int> localStructurePositions;
    public List<Vector3Int> worldStructurePositions;

    public int groundPosition;
}
