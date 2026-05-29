using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject {
    [SerializeField] public string SceneName;
    [field: SerializeField] public LevelData NextLevel { get; private set; }

    [Tooltip("How far down is out of bounds")] [field: SerializeField] public float KillLevel { get; private set; }

    [SerializeField] public bool UseDefaultColors = true;
    [field: SerializeField] public Color InactiveCheckpointColor { get; private set; } = Color.red;
    [field: SerializeField] public Color ActiveCheckpointColor { get; private set; } = Color.green;
}
