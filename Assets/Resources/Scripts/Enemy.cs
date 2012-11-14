using UnityEngine;
using System.Collections;
using Pathfinding;

public class Enemy : AIPath {

    void Start () {
        this.target = GameObject.FindWithTag("Player").transform;
        base.Start();
    }
 
}
