using UnityEngine;

public abstract class Character : MonoBehaviour {
	[SerializeField] protected float maxHP;
	protected float remHP;

	private void Update() {
		if(transform.position.y < GameController.Instance.KillLevel) {
            Die();
        }
	}

	protected void TakeDamage(float amt) {
		remHP -= amt;

		if(remHP <= 0f) {
			Die();
		}
	}

	protected void Heal(float amt, bool overHealOK = false) {
		remHP += amt;
		if(!overHealOK) { remHP = Mathf.Min(remHP, maxHP); }
	}

	protected abstract void Die();
}