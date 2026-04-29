using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public enum HandBehaviour { Move, Aim }
public class StickyHandController : MonoBehaviour {
    [Header("Movement")]
    [SerializeField] private Camera cam;
    [SerializeField] private LineRenderer arm;
    [SerializeField] private GameObject armAnchor; // local space
    [SerializeField] private float maxDistance;
    [SerializeField] private float maxSpeed;
    private HandBehaviour behaviour;
    private Vector3 worldAnchor;

    [Header("Objects")]
    [SerializeField] GameObject aimLine;
    public GameObject holding { get; private set; }


    [Header("Colliders")]
    [SerializeField] Collider2D col;
    private Dictionary<Collider2D, GameObject> touching;

    [Header("")]
    [SerializeField] GameObject handAnchor;

    [SerializeField] private TMP_Text debug;

    // Start is called before the first frame update
    void Start() {
        behaviour = HandBehaviour.Move;
        touching = new Dictionary<Collider2D, GameObject>();
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(worldAnchor, maxDistance);
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.CompareTag("Item")) {
            touching.Add(col, col.gameObject.transform.parent.gameObject);
            // Debug.Log($"can interact with {col.gameObject.transform.parent.gameObject.name}");
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.CompareTag("Item")) {
            touching.Remove(col);
            // Debug.Log($"can NOT interact with {col.gameObject.transform.parent.gameObject.name}");
        }
    }

    // Update is called once per frame
    void Update() {
        worldAnchor = armAnchor.transform.position;

        if(Input.GetKeyDown(KeyCode.LeftShift)) {
            StartAiming();
        } else if(Input.GetKeyUp(KeyCode.LeftShift)) {
            StopAiming();
        }

        MoveRotate();
        if (behaviour == HandBehaviour.Move) {
            Move();
        }
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
        handAnchor.transform.position = transform.position;
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
        // https://discussions.unity.com/t/lookat-2d-equivalent/88118
        Quaternion rotation;

        if(behaviour == HandBehaviour.Move) {
            rotation = Quaternion.LookRotation(
                worldAnchor - transform.position,
                transform.TransformDirection(Vector3.up)
            );
        } else { // aim
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            rotation = Quaternion.LookRotation(
                transform.position - mousePos,
                transform.TransformDirection(Vector3.up)
            );
        }
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
        if (holding == null) { // attempt pickup
            List<Collider2D> gameObjsInContact = new List<Collider2D>(touching.Keys);
            if (gameObjsInContact.Count < 1) return;

            gameObjsInContact.Sort(CompareGameObjs);
            PickUp(touching[gameObjsInContact[0]]);
        } else { 
            // attempt use
            // use cases
            // ball, block, empty gun: do nothing
            // loaded gun: fire
            // key: attempt open door
            holding.GetComponent<Item>().Use();
        }
    }

    private void PickUp(GameObject obj) {
        if (obj.CompareTag("Item")) {
            holding = obj;

            // lock object to sticky hand
            obj.transform.parent = handAnchor.transform;
            obj.GetComponent<Item>().PickUp();
        }
    }

    private void StartAiming() {
        behaviour = HandBehaviour.Aim;
        aimLine.SetActive(true);
        if(holding != null) { // reparent held item so it rotates with hand
            holding.transform.parent = transform;
        }
    }

    private void StopAiming() {
        behaviour = HandBehaviour.Move;
        aimLine.SetActive(false);
        if(holding != null) { // unparent held item so it doesn't rotate with hand
            holding.transform.parent = handAnchor.transform;
            holding.transform.rotation = Quaternion.identity; // reset its rotation too
        }
    }

    // RClick: drop items
    private void RClick() {
        if (holding != null) {
            GameObject obj = holding;

            holding = null;
            obj.transform.SetParent(null);

            obj.GetComponent<Item>().Drop();
        }
    }

    // custom comparator: want closest gameobj
    private int CompareGameObjs(Collider2D c1, Collider2D c2) {
        Vector3 c1pos = c1.gameObject.transform.position + new Vector3(c1.offset.x, c1.offset.y, 0);
        Vector3 c2pos = c2.gameObject.transform.position + new Vector3(c2.offset.x, c2.offset.y, 0);

        float c1dist = (c1pos - transform.position).magnitude;
        float c2dist = (c2pos - transform.position).magnitude;

        if(c1dist <= c2dist) {
            return -1;
        } else {
            return 1;
        }
    }
}
