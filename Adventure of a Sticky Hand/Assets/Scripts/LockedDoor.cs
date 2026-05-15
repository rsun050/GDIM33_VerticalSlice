using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : Door {
	// Key or KeyType: either accepts a specific key or any key of a matching type. If both are specified, specific key will be given priority.
	[field: SerializeField] public GameObject key { get; private set; }
	[field: SerializeField] public KeyType keyType { get; private set; }
}