using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actuator : MonoBehaviour {
	[SerializeField] protected Actuatable actuatee;

	public virtual void Trigger() {
		
	}
}