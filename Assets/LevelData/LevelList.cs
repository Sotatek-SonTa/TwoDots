using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwoDotsLevelData;

namespace TwoDotsLevelList
{
    [CreateAssetMenu(fileName = "LevelDataContainer", menuName = "ScriptableObject/LevelDataContainer")]
    public class LevelList : ScriptableObject
    {
        public LevelData[] levels;
    }
}


