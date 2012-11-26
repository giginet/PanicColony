using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public int rotateThreshold = 150;
    public int rotateSpeed = 5;
    public GameObject muzzle = null;
    private GameObject bomb = null;

    void Start () {
        muzzle = this.gameObject;
    }
    
    void Update () {
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
                }
            }
        } else {
            renderer.enabled = false;
        } 
    }
}

