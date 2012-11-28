using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public int rotateThreshold = 150;
    public int rotateSpeed = 5;
    public float speed = 10;
    public GameObject muzzle = null;
    private GameObject bomb = null;
    private Vector3 lastVelocity;

    void Start () {
        muzzle = this.gameObject;
    }
    
    void Update () {
        /*if (!this.IsGrounded() && !this.rigidbody.useGravity) {
            this.rigidbody.useGravity = true;
            rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
        } else if (this.IsGrounded() && this.rigidbody.useGravity) {
            this.rigidbody.useGravity = false;
            rigidbody.constraints |= RigidbodyConstraints.FreezePositionY;
        }
        Vector3 gravity = new Vector3(0, this.rigidbody.velocity.y, 0);
        Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
        forward.y = 0;
        forward = forward.normalized;
        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 velocity = h * right + v * forward;
        
        if (Mathf.Abs(h) < 0.1 && Mathf.Abs(v) < 0.1) {
            velocity = Vector3.Lerp(lastVelocity, Vector3.zero, 0.9f);
            if (Mathf.Abs(velocity.x) < 0.1f && Mathf.Abs(velocity.z) < 0.1f) {
                velocity = Vector3.zero;
                lastVelocity = Vector3.zero;
            }
        } else {
            lastVelocity = velocity;
        }
        Vector3 verticalVelocity = Vector3.zero;
        if (Input.GetButtonDown("Jump")) {
            verticalVelocity = Vector3.up * 10f;
            rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
        } else if (this.IsGrounded()) {
            verticalVelocity = Vector3.zero;
        }
        this.rigidbody.velocity = gravity + velocity * this.speed + verticalVelocity;
        
        */
        if (Input.GetButtonDown("Bomb0") ) {
            if (bomb == null) {
                // place Bomb
                GameObject bombPrefab = (GameObject)Resources.Load("Prefabs/bombPrefab");
                bomb = (GameObject)Instantiate(bombPrefab, this.transform.position + this.transform.forward * 1, Quaternion.identity);
            } else {
                bomb.SendMessage("Explode");
                bomb = null;
            }
        }
        float xAxis = Input.GetAxisRaw("CameraX0");
        this.transform.Rotate(new Vector3(0, xAxis * this.rotateSpeed, 0));
        Vector3 mouse = Input.mousePosition;
        mouse.z = Camera.main.nearClipPlane;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(this.transform.position);
        Ray ray = new Ray(this.muzzle.transform.position, this.muzzle.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100)) {
            LineRenderer renderer = this.GetComponent<LineRenderer>();
            renderer.enabled = true;
            renderer.SetVertexCount(2);
            renderer.SetPosition(0, this.muzzle.transform.position);
            renderer.SetPosition(1, hit.point);
            if (hit.collider.gameObject.CompareTag("Enemy")) {
                renderer.material.color = Color.red;   
            } else {
                renderer.material.color = Color.yellow;   
            }
            if (Input.GetButtonDown("Shock")) {
                if (hit.collider.gameObject.CompareTag("Enemy")) {
                    hit.collider.gameObject.SendMessage("Shock");
                } else if (hit.collider.gameObject.CompareTag("Bomb")) {
                    hit.collider.gameObject.SendMessage("Explode");
                } else if (hit.collider.gameObject.CompareTag("Wall")) {
                    hit.collider.gameObject.SendMessage("Damage");
                }
            }
        } else {
            renderer.enabled = false;
        } 
    }
    
    bool IsGrounded () {
        Ray floorRay = new Ray(this.transform.position + Vector3.down * this.transform.localScale.y / 2.0f, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(floorRay, out hit, 0.5f)) {
                return true;
        }
        return false;
    }
}

