using UnityEngine;

public abstract class Character : MonoBehaviour {
	[Header("Components")]
    [SerializeField] protected Rigidbody2D rb;
	[SerializeField] protected Animator animator;
    [SerializeField] protected SpriteRenderer sprite;

	[Header("")]
	[SerializeField] protected bool canBeHurt = true;
	[SerializeField] protected float maxHP;
	[Tooltip("Leave as -1 to start at maxHP")] [SerializeField] protected float remHP = -1;

	protected virtual void Start() {
		if(remHP == -1) {
			remHP = maxHP;
		}
	}

	protected void Update() {
		if(transform.position.y < GameController.Instance.KillLevel) {
			Debug.Log($"{gameObject.name} FELL OOB: {transform.position.y} VS {GameController.Instance.KillLevel}");
            Die();
        }
	}

	public virtual void TakeDamage(float amt) {
		remHP -= amt;
		Debug.Log($"{gameObject.name} ouch ({amt} dmg, {remHP} remaining)");

		if(remHP <= 0f) {
			Die();
		}
	}

	// alias lol
	public void DealDamage(float amt) {
		TakeDamage(amt);
	}

	public void Kill() {
		remHP = 0f;
		Die();
	}

	public void Heal(float amt, bool overHealOK = false) {
		remHP += amt;
		if(!overHealOK) { remHP = Mathf.Min(remHP, maxHP); }
	}

	public void FullHeal() {
		remHP = maxHP;
	}

	protected abstract void Die();
}

/*
recall:
abstract = child must implement (nothing given)
virtual = child may use what's given, or override
*/