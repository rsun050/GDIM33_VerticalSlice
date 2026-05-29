using System;
using TMPro;
using UnityEngine;

public class PlayerController : Character {
    [SerializeField] private CapsuleCollider2D col;
    [SerializeField] private TMP_Text debugText;

    [Header("Movement")]
    [SerializeField] private float maxSpeed;
    [Tooltip("Airspeed should probably be lower than maxspeed. lol")]
    [SerializeField] private float airSpeed;
    [SerializeField] private float jumpPower;
    private int horizDir;
    private bool jumpPressed;
    private bool canJump;

    [Header("Raycast")]
    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;

    [Header("Pain")] // me too
    [SerializeField] private LayerMask groundPainLayer;
    [SerializeField] private float stunTime; // how long player is stunned when taking damage
    private float stunTimeRemaining;
    public event Action playerDies;

    protected override void Start() {
        base.Start();
        stunTimeRemaining = 0f;
    }

    // Update is called once per frame
    new void Update() {
        base.Update();

        GetInputs();
        UpdateStun();
        DebugText();
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -1 * groundCheckDistance * transform.up);

        // wallcheck
        Gizmos.DrawCube(transform.position + Mathf.Clamp(transform.localScale.x, -1, 1) * wallCheckDistance * transform.right, new Vector3(col.size.x, col.size.y, 0.01f));

        // groundcheck
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Mathf.Clamp(transform.localScale.x, -1, 1) * groundCheckDistance * transform.up * -1);
    }

    public void GetInputs() {
        horizDir = 0;
        // jumpPressed = false;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
            horizDir--;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
            horizDir++;
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            jumpPressed = true;
        }
    }

    public void RunPlayerHorizontalMovement() {
        bool walking = false;
        float speed = (canJump) ? maxSpeed : airSpeed;

        if (horizDir != 0) {
            walking = true;

            transform.localScale = new Vector3(horizDir * Math.Abs(transform.localScale.x), transform.localScale.y, 1);

            RaycastHit2D wallHit = WallCheck(horizDir);
            if (wallHit.collider == null) {
                rb.AddForce(transform.right * horizDir * speed, ForceMode2D.Impulse);
            }
        }

        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -1 * speed, speed), rb.velocity.y);
        animator.SetBool("walking", walking);
    }

    public void RunPlayerVerticalMovement() {
        RaycastHit2D groundHit = groundCheck();

        if (jumpPressed && canJump) {
            rb.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);
            jumpPressed = false;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
            // TODO: platform drop logic
        }

        animator.SetBool("in air", !canJump);
    }

    private RaycastHit2D groundCheck() {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, -1 * transform.up, groundCheckDistance, groundLayers);
        canJump = hitInfo.collider != null;

        return hitInfo;
    }

    // direction: 1 for right, -1 for left
    private RaycastHit2D WallCheck(int direction) {
        RaycastHit2D boxHit = Physics2D.BoxCast(transform.position, col.size, 0f, direction * transform.right, wallCheckDistance, groundLayers);

        return boxHit;
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("PainGround")) {
            TakeDamage(9999); // TODO: change this lol
        }
    }

    public override void TakeDamage(float amt) {
        if(canBeHurt) {
            animator.SetTrigger("hurt");
            
            if(!Stunned()) {
                Stun();
            }

            base.TakeDamage(amt);            
        }
    }

    protected override void Die() {
        // TODO REFACTOR THIS SH*
        Debug.Log("Player die invoked");
        playerDies?.Invoke();

        // rb.freezeRotation = false;
        // rb.excludeLayers = LayerMask.;
    }
    private void Stun() {
        canBeHurt = false;
        stunTimeRemaining = stunTime;
        rb.velocity = Vector2.zero;
    }

    public bool Stunned() {
        return stunTimeRemaining > 0f;
    }

    private void UpdateStun() {
        if(stunTimeRemaining > 0f) {
            stunTimeRemaining -= Time.deltaTime;
            if(stunTimeRemaining <= 0f) {
                canBeHurt = true;
            }
        }
    }

    private void DebugText() {
        debugText.text = $"canJump: {canJump}\njumpPressed: {jumpPressed}\nspd: {rb.velocity}";
    }
}
