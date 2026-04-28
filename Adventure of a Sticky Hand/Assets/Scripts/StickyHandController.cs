using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public enum HandBehaviour { Move, Aim }
public class StickyHandController : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] private Camera cam;
    [SerializeField] private LineRenderer arm;
    [SerializeField] private GameObject anchor; // local space
    [SerializeField] private float maxDistance;
    [SerializeField] private float maxSpeed;
    private HandBehaviour behaviour;
    private Vector3 worldAnchor;

    [Header("Colliders")]
    [SerializeField] Collider2D col;
    private Dictionary<int, GameObject> touching;
    private GameObject holding;

    [SerializeField] private TMP_Text debug;

    // Start is called before the first frame update
    void Start() {
        behaviour = HandBehaviour.Move;
        touching = new Dictionary<int, GameObject>();
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(worldAnchor, maxDistance);
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.CompareTag("Item")) {
            touching.Add(col.gameObject.GetInstanceID(), col.gameObject);
            Debug.Log($"can interact with {col.gameObject.name}");
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.CompareTag("Item")) {
            touching.Remove(col.gameObject.GetInstanceID());
            Debug.Log($"can NOT interact with {col.gameObject.name}");
        }
    }

    // Update is called once per frame
    void Update() {
        worldAnchor = anchor.transform.position;
        behaviour = (Input.GetKey(KeyCode.LeftShift)) ? HandBehaviour.Aim : HandBehaviour.Move;

        if (behaviour == HandBehaviour.Move) {
            Move();
        }
        MoveRotate();
        MoveClamp();
        UpdateArm();

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            LClick();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            RClick();
        }
    }

    private void Move() {
        // https://www.youtube.com/watch?v=2DHy_l4Ffe0
        transform.position = Vector2.MoveTowards(transform.position, cam.ScreenToWorldPoint(Input.mousePosition), maxSpeed * Time.deltaTime);
    }

    // clamp to anchor
    private void MoveClamp() {
        Vector3 direction = transform.position - worldAnchor;
        direction.z = 0;
        direction = Vector3.Normalize(direction);

        Vector3 furthestAway = worldAnchor + direction * maxDistance; // don't move further than this from anchor

        float minX, maxX, minY, maxY;
        if (direction.x >= 0) { // mouse is right of hand
            minX = worldAnchor.x;
            maxX = furthestAway.x;
        }
        else { // mouse is left of hand
            minX = furthestAway.x;
            maxX = worldAnchor.x;
        }

        if (direction.y >= 0) { // mouse above hand
            minY = worldAnchor.y;
            maxY = furthestAway.y;
        }
        else { // mouse below hand
            minY = furthestAway.y;
            maxY = worldAnchor.y;
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);
    }

    // TODO: FIX
    private void MoveRotate() {
        // Debug.Log("ROTATE");
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        Quaternion rotation = Quaternion.LookRotation(
            mousePos - transform.position,
            transform.TransformDirection(Vector3.up)
        );
        transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
    }

    // update stick hand's arm
    private void UpdateArm() {
        arm.SetPositions(
            new Vector3[] {
                worldAnchor,
                transform.position
            }
        );
    }

    // LClick: trigger actuators, pickup items
    private void LClick() {
        if (holding == null) {
            Debug.Log("LClick");
            List<GameObject> gameObjsInContact = new List<GameObject>(touching.Values);
            gameObjsInContact.Sort(CompareGameObjs);

            if (gameObjsInContact.Count < 1) return;

            GameObject clickedObj = gameObjsInContact[0];
            PickUp(clickedObj);
        }
    }

    private void PickUp(GameObject obj) {
        if (obj.CompareTag("Item")) {
            holding = obj;

            // lock object to sticky hand
            obj.transform.SetParent(transform);
            obj.transform.localPosition = new Vector3(0.5f, 0, 0);

            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

            // stop held object from moving/colliding
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.rotation = 0f;
            EventBus.Trigger(EventNames.DisableColliderEvent, obj.GetComponent<Item>().allColliders);
        }
    }

    // RClick: drop items
    private void RClick() {
        if (holding != null) {
            Debug.Log("RClick");
            GameObject obj = holding;

            holding = null;
            obj.transform.SetParent(null);

            obj.GetComponent<Rigidbody2D>().isKinematic = false;
            EventBus.Trigger(EventNames.EnableColliderEvent, obj.GetComponent<Item>().allColliders);
        }
    }

    // custom comparator: want closest gameobj
    private int CompareGameObjs(GameObject g1, GameObject g2) {
        float g1dist = (g1.transform.position - transform.position).magnitude;
        float g2dist = (g2.transform.position - transform.position).magnitude;

        return (int)(g1dist - g2dist);
    }
}
