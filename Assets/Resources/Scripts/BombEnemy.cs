using UnityEngine;
using System.Collections;

public class BombEnemy : Enemy {

    private GameObject robot = null;
    private bool slip = false;

    virtual protected void Start () {
        base.Start();
        this.robot = this.transform.Find("robot").gameObject;
        this.robot.animation.Play("run");
        this.slip = false;
        this.robot.collider.enabled = false;
    }
    
    override public bool Attack () {
        return true;
    }

    // Use this for initialization
    virtual protected void Update () {
        base.Update();
        CharacterController controller = this.GetComponent<CharacterController>();
        if (controller != null && !this.slip) {
            if (Vector3.Distance(Vector3.zero, controller.velocity) > 1.0f) {
                if (!this.robot.animation.IsPlaying("run")) {
                    this.robot.animation.Play("run");
                }
            } else {
                if (!this.robot.animation.IsPlaying("idle")) {
                    this.robot.animation.Play("idle");
                }
            }
        }
    }
    
    override public void Shock () {
        this.aiPath.canMove = false;
        if (!this.slip) { 
            this.Death();
        } 
    }
    
    private IEnumerator BombRoom () {
        Room room = null;
        yield return new WaitForSeconds(1.0f);
        for (int i = 0; i < 15; ++i) {     
            room = this.levelManager.GetRoom(this.transform.position);
            if (room != null && !room.IsProtect() && room.IsEnable()) {
                this.audio.Play();
                float second = Mathf.Max(0.1f, 1.0f - i * 0.1f);
                yield return new WaitForSeconds(second);
            }
        }
        room = this.levelManager.GetRoom(this.transform.position);   
        if (room != null && !room.IsProtect() && room.IsEnable()) {
            this.SendMessage("Explode");
        }
    }
    
    override public void Death () {
        GameObject radar = GameObject.FindWithTag ("Radar");
        radar.SendMessage ("DestroyChip", this.gameObject);
        GameObject controller = GameObject.FindWithTag("GameController");
        controller.SendMessage("DestroyEnemy", this.gameObject);
        this.slip = true;
        this.robot.animation.Play("slip");
        this.gameObject.GetComponent<CharacterController>().radius = 0.01f;
        this.gameObject.GetComponent<CharacterController>().height = 0.01f;
        this.tag = "";
        this.StartCoroutine(this.BombRoom());
    }
}
