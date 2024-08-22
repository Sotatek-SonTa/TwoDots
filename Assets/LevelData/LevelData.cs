using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData",menuName ="ScriptableObject/LevelData",order = 1)]
public class LevelData : ScriptableObject
{
    public int level;
    public DotType[] spawnableDotTypes;
    public int numberOfMoves;
    public DotTypeRequirement[] dotTypeRequirements;
}
[Serializable]
public class DotTypeRequirement
{
    public DotType dotType;
    public int quantity;
}

