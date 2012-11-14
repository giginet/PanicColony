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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100)) {
            LineRenderer renderer = this.GetComponent<LineRenderer>();
            renderer.SetVertexCount(2);
            renderer.SetPosition(0, this.transform.position);
            renderer.SetPosition(1, hit.point);
        }   
    }
}

