using System;
using UnityEngine;

public class Hand : MonoBehaviour {
	[SerializeField] private LayerMask interactableLayers;
	[SerializeField] private string[] interactableTags;

	// the layers have to be named [LayerName] and [LayerName]Outline

	void OnTriggerEnter2D(Collider2D col) {
		// Debug.Log($"hand trigger enter: gameobj {col.gameObject.name}");
		if (HasInteractableTag(col.gameObject.tag) && OnInteractableLayer(col.gameObject.transform.parent.gameObject.layer)) {
			if(!GameController.Instance.StickyHand.touching.ContainsKey(col)) {
				GameController.Instance.StickyHand.touching.Add(col, col.gameObject.transform.parent.gameObject);
				col.gameObject.transform.parent.gameObject.layer = LayerMask.NameToLayer(LayerMask.LayerToName(col.gameObject.transform.parent.gameObject.layer) + "Outline");
				// Debug.Log($"can interact with {col.gameObject.transform.parent.gameObject.name}");
			}
		}
	}

	void OnTriggerExit2D(Collider2D col) {
		// Debug.Log($"something exited");

		if (HasInteractableTag(col.gameObject.tag) && OnInteractableLayer(col.gameObject.transform.parent.gameObject.layer)) {
			if(GameController.Instance.StickyHand.touching.ContainsKey(col)) {
				GameController.Instance.StickyHand.touching.Remove(col);

				string layerName = LayerMask.LayerToName(col.gameObject.transform.parent.gameObject.layer);
				// Debug.Log($"hand trigger exit: gameobj {col.gameObject.name} on layer {col.gameObject.layer}, parent layer {layerName}");

				col.gameObject.transform.parent.gameObject.layer = LayerMask.NameToLayer(layerName.Substring(0, layerName.Length - 7));
				// Debug.Log($"can NOT interact with {col.gameObject.transform.parent.gameObject.name}");
			}
		}
	}

	private bool OnInteractableLayer(int layerIndex) {
		return ((1 << layerIndex) | interactableLayers) == interactableLayers;
	}

	private bool HasInteractableTag(string tag) {
		foreach(string interactableTag in interactableTags) {
			if(tag == interactableTag) {
				return true;
			}
		}

		return false;
	}
}