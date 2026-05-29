using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyType { Any, Skeleton, Red, Blue, Green, Yellow, Purple, White, Pink }

[CreateAssetMenu(fileName = "KeyData", menuName = "ScriptableObjects/KeyData", order = 0)]
public class KeyData : ScriptableObject {
    [field: SerializeField] public KeyType keyType { get; private set; }
}
