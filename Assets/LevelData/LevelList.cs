using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="LevelDataContainer",menuName ="ScriptableObject/LevelDataContainer")]
public class LevelList : ScriptableObject
{
  public LevelData[] levels;
}
