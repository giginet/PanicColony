using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public enum PlayerState {
        Normal,
        Shock,
        DeathAnimation,
        Death
    };
   
    public int playerNumber = 0;
    public int rotateThreshold = 150;
    public int rotateSpeed = 5;
    public float speed = 10;
    public GameObject muzzle = null;
    private GameObject bomb = null;
    private GameObject shockEffect = null;
    private bool canControl = true;
    private JoyStickController controller = null;
    private float deathTimer = 0;
    private PlayerState state = PlayerState.Normal;
    private float preYAxis = 0;
    private GameObject head = null;

    void Start () {
        muzzle = this.transform.Find("muzzle").gameObject;
        this.state = PlayerState.Normal;
        this.controller = this.GetComponent<JoyStickController>();
        this.controller.cameraControl.cameraTransform = Camera.main.transform;
        this.controller.cameraControl.cameraTransform.position = this.transform.position;
        this.head = this.transform.Find("Armature/Bone/hip_0/chest_0/head").gameObject;
    }
    
    void Control () {
        if (Input.GetButtonDown("Bomb0") ) {
            if (bomb == null) {
                // place Bomb
                GameObject bombPrefab = (GameObject)Resources.Load("Prefabs/bombPrefab");
                bomb = (GameObject)Instantiate(bombPrefab, this.transform.position + this.transform.forward * -1, Quaternion.identity);
            } else {
                bomb.SendMessage("Explode");
                bomb = null;
            }
        } 
        float yAxis = Input.GetAxis("Vertical");
        if (yAxis <= -0.9f && this.preYAxis > -0.9f) {
            Vector3 wantedAngle = this.controller.GetWantedCameraAngle();
        }
        preYAxis = yAxis;
        Vector3 mouse = Input.mousePosition;
        mouse.z = Camera.main.nearClipPlane;
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(this.transform.position);
        Ray ray = new Ray(this.muzzle.transform.position, this.muzzle.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100)) {
            LineRenderer renderer = this.GetComponent<LineRenderer>(); 
            if (Input.GetButton("Shock")) {
                renderer.enabled = false;
                if (shockEffect == null) {
                    GameObject prefab = (GameObject)Resources.Load("Prefabs/shockEffectPrefab", typeof(GameObject));
                    shockEffect = (GameObject)Instantiate(prefab, this.muzzle.transform.position, Quaternion.identity);
                    LevelManager manager = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>();
                    shockEffect.transform.parent = manager.GetLevelObject().transform;
                }
                if (hit.collider.gameObject.CompareTag("Enemy")) {
                    hit.collider.gameObject.SendMessage("Shock");
                } else if (hit.collider.gameObject.CompareTag("Bomb")) {
                    hit.collider.gameObject.SendMessage("Explode");
                } else if (hit.collider.gameObject.CompareTag("Switch")) {
                    hit.collider.gameObject.SendMessage("Toggle");
                } else if (hit.collider.gameObject.CompareTag("Wall")) {
                    hit.collider.gameObject.SendMessage("Damage");
                }
            } else {
                renderer.enabled = true;
                renderer.SetVertexCount(2);
                renderer.SetPosition(0, this.muzzle.transform.position);
                renderer.SetPosition(1, hit.point);
                if (hit.collider.gameObject.CompareTag("Enemy")) {
                    renderer.material.color = Color.red;   
                } else {
                    renderer.material.color = Color.yellow;   
                }
                if (shockEffect != null) {
                    Destroy(shockEffect);
                    shockEffect = null;
                }
            }
        } else {
            renderer.enabled = false;
        } 
        if (this.shockEffect != null) {
            shockEffect.transform.position = this.muzzle.transform.position;
            shockEffect.transform.LookAt(hit.point);
        }
        CharacterMotor motor = this.gameObject.GetComponent<CharacterMotor>();
        this.audio.volume = Vector3.Distance(motor.GetDirection(), Vector3.zero) / 1.0f;
    }
    
    void Update () {
        //this.controller.SetInputType(Input.GetJoystickNames().Length == 0 ? InputType.Mouse : InputType.JoyStick);
        this.controller.SetInputType(InputType.JoyStick);
        if (this.canControl) {
            this.Control();
        } 
        if (this.state == PlayerState.DeathAnimation) {
            this.head.transform.Rotate(Vector3.right * ((1.0f - deathTimer) * 120));
            this.deathTimer += Time.deltaTime;
            if (deathTimer > 2.9) {
                this.state = PlayerState.Death;
                this.DestroyBody();
                GameObject controller = GameObject.FindWithTag("GameController");
                controller.SendMessage("Miss", this.playerNumber);
            }
        }
        if (this.transform.position.y < -10) {
            this.Death();
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
    
    void Death () {
        if (this.state != PlayerState.Normal) return;
        Destroy(shockEffect);
        shockEffect = null;
        LineRenderer renderer = this.GetComponent<LineRenderer>(); 
        renderer.enabled = false;  
        this.SetControl(false);
        this.audio.volume = 0;
        this.state = PlayerState.DeathAnimation;
        GameObject.FindWithTag("GameController").SendMessage("PlayMusic", "Sounds/gameover0");
    }
    
    void DestroyBody () {
        /*Destroy(this.GetComponent<PlatformInputController>());
        Destroy(this.GetComponent<CharacterMotor>());
        Destroy(this.GetComponent<CharacterController>());*/
        Destroy(this.transform.Find ("Armature").gameObject);
        foreach (Transform part in this.transform) {
            part.gameObject.AddComponent("Rigidbody");
            if (part.GetComponent<BoxCollider>() != null) {
                part.GetComponent<BoxCollider>().enabled = true;
            }
            part.rigidbody.velocity = Random.onUnitSphere;
        }
    }
    
    void SetControl (bool c) {
        this.canControl = c;
        this.GetComponent<CharacterMotor>().canControl = c;
        this.GetComponent<JoyStickController>().enableLookRotation = c;
        if (!c) this.audio.volume = 0;
    }
}

