using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    [field: SerializeField] public Vector3 worldAnchor { get; private set; }


    public GameObject holding { get; private set; }
    [field: SerializeField] public GameObject aimLine { get; private set; }

    [Header("Colliders")]
    // [SerializeField] Collider2D col;
    public Dictionary<Collider2D, GameObject> touching;

    [Header("")]
    [SerializeField] GameObject handAnchor;

    [SerializeField] private TMP_Text debug;
    [field: SerializeField] public float throwForce { get; private set; }

    // Start is called before the first frame update
    void Start() {
        behaviour = HandBehaviour.Move;
        touching = new Dictionary<Collider2D, GameObject>();
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(worldAnchor, maxDistance);

        Gizmos.DrawRay(transform.position, transform.right * 2);
    }

    // Update is called once per frame
    void Update() {
        worldAnchor = armAnchor.transform.position;

        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            StartAiming();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift)) {
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

        if (behaviour == HandBehaviour.Move) {
            rotation = Quaternion.LookRotation(
                worldAnchor - transform.position,
                transform.TransformDirection(Vector3.up)
            );
        }
        else { // aim
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

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
        }
        else {
            // attempt use
            // use cases
            // ball, block, empty gun: throw it
            // loaded gun: fire
            // key: attempt open door
            switch(behaviour) {
                case HandBehaviour.Move:                
                    holding.GetComponent<Item>().Use();
                    break;
                case HandBehaviour.Aim:
                    if(holding.GetComponent<Gun>() != null) {
                        holding.GetComponent<Gun>().Use();
                    } else { // throw that shi
                        Vector3 throwForceVec = throwForce * aimLine.GetComponent<AimDirection>().getDir();
                        Debug.Log($"throwing w/ force {throwForceVec}");

                        GameObject obj = Drop();
                        obj.GetComponent<Item>().Throw(throwForceVec);
                    }
                    break;
            }
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
        if (holding != null) { // reparent held item so it rotates with hand
            holding.transform.rotation = Quaternion.identity;
            holding.transform.parent = transform;
            // holding.transform.localPosition = Vector3.zero;
        }
    }

    private void StopAiming() {
        behaviour = HandBehaviour.Move;
        aimLine.SetActive(false);
        // transform.rotation = Quaternion.identity;

        if (holding != null) { // unparent held item so it doesn't rotate with hand
            holding.transform.parent = handAnchor.transform;
            holding.GetComponent<Item>().PickUp(); // reset the item's position back to handanchor? it's necessary to fix bug of item pos desyncing from hand when moving around while aiming LOL.
            holding.transform.rotation = Quaternion.identity; // reset its rotation too
        }
    }

    // RClick: drop items
    private void RClick() {
        if (holding != null) {
            Drop();
        }
    }

    private GameObject Drop() {
        GameObject obj = holding;

        holding = null;
        obj.transform.SetParent(null);

        obj.GetComponent<Item>().Drop();
        return obj;
    }

    // custom comparator: want closest gameobj
    private int CompareGameObjs(Collider2D c1, Collider2D c2) {
        Vector3 c1pos = c1.gameObject.transform.position + new Vector3(c1.offset.x, c1.offset.y, 0);
        Vector3 c2pos = c2.gameObject.transform.position + new Vector3(c2.offset.x, c2.offset.y, 0);

        float c1dist = (c1pos - transform.position).magnitude;
        float c2dist = (c2pos - transform.position).magnitude;

        if (c1dist <= c2dist) {
            return -1;
        }
        else {
            return 1;
        }
    }
}
