using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoDotsLevelData
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObject/LevelData", order = 1)]
    public class LevelData : ScriptableObject
    {
        public int level;
        public int rows;
        public int columns;
        public DotType[] spawnableDotTypes;
        public int numberOfMoves;
        public DotTypeRequirement[] dotTypeRequirements;
        public Vector2Int[] blockedCells;
        public float maxGridHeight;
        public float maxGridWidth;
    }
    [Serializable]
    public class DotTypeRequirement
    {
        public DotType dotType;
        public int quantity;
    }

}


