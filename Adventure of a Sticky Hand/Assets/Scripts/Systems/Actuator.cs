using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actuator : MonoBehaviour {
	[SerializeField] protected Actuatable[] actuatees;

	public virtual void Trigger() {
		
	}
}