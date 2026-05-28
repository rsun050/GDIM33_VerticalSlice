using UnityEngine;

public class Hand : MonoBehaviour {
	[SerializeField] private LayerMask interactableLayers;

	// the layers have to be named [LayerName] and [LayerName]Outline

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.CompareTag("Item") && OnInteractableLayer(col.gameObject.transform.parent.gameObject.layer)) {
			if(!GameController.Instance.StickyHand.touching.ContainsKey(col)) {
				GameController.Instance.StickyHand.touching.Add(col, col.gameObject.transform.parent.gameObject);
				col.gameObject.transform.parent.gameObject.layer = LayerMask.NameToLayer(LayerMask.LayerToName(col.gameObject.transform.parent.gameObject.layer) + "Outline");
				// Debug.Log($"can interact with {col.gameObject.transform.parent.gameObject.name}");				
			}
		}
	}

	void OnTriggerExit2D(Collider2D col) {
		if (col.gameObject.CompareTag("Item") && OnInteractableLayer(col.gameObject.transform.parent.gameObject.layer)) {
			if(GameController.Instance.StickyHand.touching.ContainsKey(col)) {
				GameController.Instance.StickyHand.touching.Remove(col);

				string layerName = LayerMask.LayerToName(col.gameObject.transform.parent.gameObject.layer);
				col.gameObject.transform.parent.gameObject.layer = LayerMask.NameToLayer(layerName.Substring(0, layerName.Length - 7));
				// Debug.Log($"can NOT interact with {col.gameObject.transform.parent.gameObject.name}");				
			}
		}
	}

	private bool OnInteractableLayer(int layerIndex) {
		return ((1 << layerIndex) | interactableLayers) == interactableLayers;
	}
}