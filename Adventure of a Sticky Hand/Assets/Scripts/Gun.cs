using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Item {
	[SerializeField] private GameObject bulletPrefab;

	// "essentially" infinite... unless you're gonna fire like... what, some quintillion bullets? idk
	[Tooltip("If maxAmmo is set to -1, this will be treated as 'infinite ammo'")]
	[SerializeField] private float maxAmmo = -1;
	[SerializeField] private float remainingAmmo = -1;
	[SerializeField] private float firesAmount = 1;

	private void Start() {
	}

	private void LateUpdate() {
		switch (itemState) {
			case ItemState.Held:
				Vector3 pos = transform.localPosition;
				Vector3 localScale = transform.localScale;

				if (transform.position.x < GameController.Instance.Player.transform.position.x) {
					pos.x = Mathf.Abs(pos.x) * -1;
					localScale.x = Mathf.Abs(localScale.x) * -1;
				}
				else {
					pos.x = Mathf.Abs(pos.x);
					localScale.x = Mathf.Abs(localScale.x);
				}

				transform.localPosition = pos;
				transform.localScale = localScale;
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