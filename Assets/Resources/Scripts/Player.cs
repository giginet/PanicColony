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
    }
}
