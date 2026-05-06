using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Item {
	[SerializeField] private SpriteRenderer sprite;
	[SerializeField] private GameObject bulletPrefab;

	// "essentially" infinite... unless you're gonna fire like... what, some quintillion bullets? idk
	[Tooltip("If maxAmmo is set to -1, this will be treated as 'infinite ammo'")]
	[SerializeField] private float maxAmmo = -1;
	[SerializeField] private float remainingAmmo = -1;
	[SerializeField] private float firesAmount = 1;

	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, transform.right * 10);
	}

	private void Start() {
	}

	private void LateUpdate() {
		switch (itemState) {
			case ItemState.Held:
				Vector3 pos = transform.localPosition;
				if (transform.position.x < GameController.Instance.Player.transform.position.x) {
					pos.x = Mathf.Abs(pos.x) * -1;
					sprite.flipX = true;
				}
				else {
					pos.x = Mathf.Abs(pos.x);
					sprite.flipX = false;
				}

				transform.localPosition = pos;
				break;
		}
	}

	public override void PickUp() {
		SetAmmoUI();
		UIController.Instance.ammoUI.gameObject.SetActive(true);

		base.PickUp();
	}

	public override void Drop() {
		UIController.Instance.ammoUI.gameObject.SetActive(false);

		base.Drop();
	}

	public override void Use() {
		Fire();
	}

	private void Fire() {
		if (maxAmmo == -1 || remainingAmmo >= firesAmount) {
			for (int i = 0; i < firesAmount; i++) {
				GameObject bullet = Instantiate(bulletPrefab, transform);
				bullet.transform.parent = null;
				bullet.SetActive(true);

				remainingAmmo--;
			}
		}
		SetAmmoUI();
	}

	public void SetAmmoUI() {
		if (maxAmmo == -1) {
			UIController.Instance.ammoUI.text = $"Ammunition: infinite";
		}
		else {
			UIController.Instance.ammoUI.text = $"Ammunition: {remainingAmmo} / {maxAmmo}";
		}
	}
}