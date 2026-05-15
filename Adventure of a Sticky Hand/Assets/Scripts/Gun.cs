using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// note to self it would be really funny if we had gun reloading
public class Gun : Item {
	[SerializeField] private SpriteRenderer sprite;
	[SerializeField] private GameObject bulletPrefab;

	// "essentially" infinite... unless you're gonna fire like... what, some quintillion bullets? idk
	[Tooltip("If maxAmmo is set to -1, this will be treated as 'infinite ammo'")]
	[SerializeField] private float maxAmmo = -1;
	[SerializeField] private float remainingAmmo = -1;
	[SerializeField] private float firesAmount = 1;
	[SerializeField] private float fireCooldown;
	public int dir { get; private set; }

	private float cooldownTimeRemaining;
	private bool canFire;

	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, transform.right * 10);
	}

	private void Start() {
	}

	private void Update() {
		if(!canFire) {
			cooldownTimeRemaining -= Time.deltaTime;

			if(cooldownTimeRemaining <= 0) {
				canFire = true;
			}
		}
	}

	private void LateUpdate() {
		dir = (transform.position.x < GameController.Instance.Player.transform.position.x) ? -1 : 1;

		switch (itemState) {
			case ItemState.Held:
				sprite.flipX = dir != 1;

				Vector3 pos = transform.localPosition;
				pos.x = Mathf.Abs(pos.x) * dir;
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
		if (canFire && (maxAmmo == -1 || remainingAmmo >= firesAmount)) {
			for (int i = 0; i < firesAmount; i++) {
				GameObject bullet = Instantiate(bulletPrefab, transform);
				bullet.SetActive(true);

				remainingAmmo--;
			}

			GoOnCooldown();
			SetAmmoUI();
		}
	}

	private void GoOnCooldown() {
		cooldownTimeRemaining = fireCooldown;
		canFire = false;
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