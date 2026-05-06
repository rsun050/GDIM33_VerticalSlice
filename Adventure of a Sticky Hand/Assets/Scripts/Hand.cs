using UnityEngine;

public class Hand : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.CompareTag("Item")) {
			GameController.Instance.StickyHand.touching.Add(col, col.gameObject.transform.parent.gameObject);
			// Debug.Log($"can interact with {col.gameObject.transform.parent.gameObject.name}");
		}
	}

	void OnTriggerExit2D(Collider2D col) {
		if (col.gameObject.CompareTag("Item")) {
			GameController.Instance.StickyHand.touching.Remove(col);
			// Debug.Log($"can NOT interact with {col.gameObject.transform.parent.gameObject.name}");
		}
	}


}