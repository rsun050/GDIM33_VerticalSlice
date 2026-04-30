using UnityEngine;

public class Controller : MonoBehaviour {
	Rigidbody rb;

	float xInput, zInput;
	public float speed;

	public float jumpforce;

	public LayerMask GroundLayer;

	public SphereCollider Col;

	float fallmult = 2.5f;
	float lowjump = 2f;

	private void Awake() {
		rb = GetComponent<Rigidbody>();
		Col = GetComponent<SphereCollider>();
	}

	void Update() {
		xInput = Input.GetAxis("Horizontal") * speed;
		zInput = Input.GetAxis("Vertical") * speed;
	}

	private void FixedUpdate() {
		rb.velocity = new Vector3(xInput, rb.velocity.y, zInput);

		if (IsGrounded() && Input.GetButtonDown("Jump")) {
			rb.AddForce(Vector3.up * jumpforce * Time.deltaTime, ForceMode.Impulse);
		}

		if (rb.velocity.y < 0) {
			rb.velocity += Vector3.up * Physics.gravity.y * (fallmult - 1) * Time.deltaTime;
		}
		else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) {
			rb.velocity += Vector3.up * Physics.gravity.y * (lowjump - 1) * Time.deltaTime;
		}
	}

	private bool IsGrounded() {
		return Physics.CheckCapsule(Col.bounds.center, new Vector3(Col.bounds.center.x, Col.bounds.min.y, Col.bounds.center.z), Col.radius * 0.9f, GroundLayer);
	}

}