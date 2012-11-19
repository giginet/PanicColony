using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    private GameObject bomb = null;

    void Start () {
    
    }
    
    void Update () {
        if (Input.GetKeyDown(KeyCode.Z) ) {
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
        mouse.y = Camera.main.WorldToScreenPoint(transform.position).y + 100;
        Ray ray = Camera.main.ScreenPointToRay(mouse);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100)) {
            LineRenderer renderer = this.GetComponent<LineRenderer>();
            if (Vector3.Distance(this.transform.position, hit.point) >= 3) {
                renderer.enabled = true;
                renderer.SetVertexCount(2);
                renderer.SetPosition(0, this.transform.position);
                Vector3 target = hit.point;
                target.y = this.transform.position.y;
                renderer.SetPosition(1, target);
                this.transform.LookAt(target);
            } else {
                renderer.enabled = false;
            }
            //Vector3 sub = hit.point - transform.position;
            //sub.y = transform.position.y;
            //transform.forward = sub;
        }   
    }
}

