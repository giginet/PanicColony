using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public int rotateThreshold = 150;
    private GameObject bomb = null;

    void Start () {
    
    }
    
    void Update () {
        if (Input.GetButtonDown("Bomb") ) {
            if (bomb == null) {
                // place Bomb
                GameObject bombPrefab = (GameObject)Resources.Load("Prefabs/bombPrefab");
                bomb = (GameObject)Instantiate(bombPrefab, this.transform.position, Quaternion.identity);
            } else {
                bomb.SendMessage("Explode");
                bomb = null;
            }
        }
        Vector3 mouse = Input.mousePosition;
        mouse.z = Camera.main.nearClipPlane;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(this.transform.position);
        Ray ray = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100)) {
            LineRenderer renderer = this.GetComponent<LineRenderer>();
            renderer.enabled = true;
            renderer.SetVertexCount(2);
            renderer.SetPosition(0, this.transform.position);
            Vector3 target = hit.point;
            renderer.SetPosition(1, target);
            if (hit.collider.gameObject.CompareTag("Enemy")) {
                renderer.material.color = Color.red;   
            } else {
                renderer.material.color = Color.yellow;   
            }
            if (Mathf.Abs(screenPoint.x - mouse.x) > rotateThreshold ) {
                this.transform.LookAt(Vector3.Lerp(this.transform.position + this.transform.forward * hit.distance, hit.point, 0.1f + Time.deltaTime));
            }
            this.transform.eulerAngles = new Vector3(0, this.transform.eulerAngles.y, 0);
            //Vector3 sub = hit.point - transform.position;
            //sub.y = transform.position.y;
            //transform.forward = sub;
        }   
    }
}

