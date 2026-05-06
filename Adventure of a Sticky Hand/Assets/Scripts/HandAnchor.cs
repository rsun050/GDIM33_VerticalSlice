using Unity;
using UnityEngine;

public class HandAnchor : MonoBehaviour {
	private float offsetX;
	private float offsetY;
	private float dist;

	void Awake() {
		offsetX = transform.localPosition.x;
		offsetY = transform.localPosition.y;

		dist = Vector3.Magnitude(new Vector2(offsetX, offsetY));
	}
	void LateUpdate() {
		Transform stickyTransform = GameController.Instance.StickyHand.transform;
		Vector3 pos = stickyTransform.position;
		pos += dist * stickyTransform.right;

		transform.position = pos;
	}
}