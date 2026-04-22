using Unity.VisualScripting;
using UnityEngine;

public static class EventNames
{
	public static string DisableColliderEvent = "DisableColliderEvent";
}

[UnitTitle("On Disable All Colliders")]
[UnitCategory("Events\\MyEvents")]

public class DisableColliderEvent : EventUnit<Collider2D[]> {
	[DoNotSerialize]
	protected override bool register => true;
	public ValueOutput colliderArray { get; private set; }// The Event output data to return when the Event is triggered.

	public override EventHook GetHook(GraphReference reference) {
		return new EventHook(EventNames.DisableColliderEvent);
	}

	protected override void Definition() {
		base.Definition();
		// Setting the value on our port.
		colliderArray = ValueOutput<Collider2D[]>(nameof(colliderArray));
	}

	// Setting the value on our port.
	protected override void AssignArguments(Flow flow, Collider2D[] colliders) {
		flow.SetValue(colliderArray, colliders);
	}

}

// public void DisableAllColliders() {
//     foreach(Collider2D c in allColliders) {
//         c.enabled = false;
//     }
// }

// public void EnableAllColliders() {
//     foreach(Collider2D c in allColliders) {
//         c.enabled = true;
//     }
// }