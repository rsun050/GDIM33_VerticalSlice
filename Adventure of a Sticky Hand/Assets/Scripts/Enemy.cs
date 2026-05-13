using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStatus { Idle, Pursue, Attack }
public class Enemy : Character {

    [Header("Movement")]
    [SerializeField] private GameObject pos;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float patrolSpeed;
    [SerializeField] private float pursueSpeed;

    [Header("Combat")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float targetSearchDistance;
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackCooldown;

    [Header("Components")]
    [SerializeField] private CapsuleCollider2D col;


    private EnemyStatus status;
    private float spd;
    private float dir = 1;
    private bool atCliff;
    private bool atWall;

    void OnDrawGizmos() {
        Gizmos.color = Color.red;

        // player (target)check
        Gizmos.DrawCube((pos.transform.position + transform.right * targetSearchDistance * dir) / 2, new Vector3(targetSearchDistance, 2, 1));

        Gizmos.color = Color.blue;

        // groundcheck
        Gizmos.DrawRay(pos.transform.position, (dir * 0.1f * transform.right + -1 * transform.up).normalized * targetSearchDistance);

        // wallcheck
        Gizmos.DrawCube((pos.transform.position + transform.right * dir * 1f) / 2, new Vector3(1.5f, 2, 1));
    }

    // Start is called before the first frame update
    void Start() {

    }

    new void Update() {
        if(TargetCheck()) {
            status = EnemyStatus.Pursue;
            spd = pursueSpeed;
        } else {
            status = EnemyStatus.Idle;
            spd = patrolSpeed;
        }

        atCliff = !GroundCheck();
        atWall = WallCheck();
    }

    void FixedUpdate() { // make this shit a visual scripting graph MWAHAHAHAHAA!!!
        if(atCliff || atWall) {
            TurnAround();
        } else {
            Walk();
        }

        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -1 * patrolSpeed, spd), rb.velocity.y);
    }

    protected override void Die() {
        if(transform.position.y < GameController.Instance.KillLevel) {
            Destroy(gameObject);
            return;
        }

        animator.SetBool("dead", true);
        col.enabled = false;        
    }

    void Walk() {
        rb.AddForce(transform.right * dir * patrolSpeed, ForceMode2D.Impulse);
    }

    void TurnAround() {
        dir *= -1;
        sprite.flipX = !sprite.flipX;
    }


    // look for ground ahead to walk on
    bool GroundCheck() {
        return Physics2D.Raycast(pos.transform.position, (dir * 0.1f * transform.right + -1 * transform.up).normalized, targetSearchDistance, groundLayer);
    }

    // look if we're colliding into a wall lmao
    bool WallCheck() {
        return Physics2D.BoxCast(pos.transform.position, col.size, 0f, dir * transform.right, 0.3f, groundLayer);
    }

    // is an attackable character in range?
    bool TargetCheck() {
        return Physics2D.BoxCast(pos.transform.position, col.size, 0f, dir * transform.right, targetSearchDistance, targetLayer);
    }

    float DistanceToTarget() {
        return Vector2.Distance(pos.transform.position, GameController.Instance.Player.transform.position);
    }

    void Attack() {
        animator.SetTrigger("attack");
    }
}
