using UnityEngine;
using System.Collections;

public enum InputType {
    Mouse,
    JoyStick
}; 
    
[RequireComponent(typeof(CharacterController))]
public class JoyStickController : MonoBehaviour {
    private InputType type = InputType.Mouse; 
    
    [System.Serializable]
    public class CameraControl {
        public bool enabled = true;
        public bool rigidbodyEnabled = false;
        public Transform cameraTransform = null;
        public float distance = 3.0f;
        public float height = 5.0f;
        public float damping = 1.0f;
        public float rotationDamping = 3.0f;
        public CameraRotationControl horizontalControl;
        public CameraRotationControl verticalControl;
    }
    
    [System.Serializable]
    public class CharacterControl {
        public bool enabled = true;
        public AnimationClip idle = null;
        public AnimationClip walk = null;
        public float speed = 5.0f;
    } 
    
    [System.Serializable]
    public class CameraRotationControl {
        public bool controllEnabled = true;
        public bool inverse = false;
        public float speed = 3.0f;
        public string axisName = "";
        public float minAngle = -75.0f;
        public float maxAngle = 75f;
    }
    
    public int mouseRotationHorizontalThreshold = 400;
    public int mouseRotationVerticalThreshold = 400;
    public float mouseRotationHorizontalSpeed = 1.0f;
    public float mouseRotationVerticalSpeed = 1.0f;
    public bool enableLookRotation = true;
    
    public CameraControl cameraControl;
    public CharacterControl characterControl;
    private Vector3 wantedCameraAngle = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    
    // Use this for initialization
    void Start () {
        this.cameraControl.cameraTransform.position = this.transform.position + Vector3.up * this.cameraControl.height + Vector3.forward * -this.cameraControl.distance;
        this.cameraControl.cameraTransform.collider.enabled = this.cameraControl.rigidbodyEnabled;
    }
    
    void Update () {
        if (this.characterControl.enabled) {
            this.ControlCharacter();
        }
    }
    
    // Update is called once per frame
    void FixedUpdate () {
        if (this.cameraControl.enabled) {
            this.ControlCamera();
        }
        if (this.enableLookRotation) {
            Vector3 lookTarget = this.transform.position + Quaternion.Euler(this.wantedCameraAngle) * Vector3.forward;
            lookTarget.y = this.transform.position.y;
            this.transform.LookAt(lookTarget);
        }
    }
    
    void ControlCharacter () {
        Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
        forward.y = 0;
        forward = forward.normalized;
        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");
        this.velocity = Vector3.Normalize(forward * v + right * h) * this.characterControl.speed * Time.deltaTime;
        CharacterController controller = this.GetComponent<CharacterController>();
        controller.Move (velocity); 
    }
    
    void ControlCamera () {
        // Calculate the current rotation angles
        Vector3 wantedCameraPosition = this.transform.position + Quaternion.Euler(this.wantedCameraAngle) * (Vector3.forward * -this.cameraControl.distance) + Vector3.up * this.cameraControl.height;
        Vector3 currentCameraPosition = this.cameraControl.cameraTransform.position;
        Quaternion wantedCameraRotation = Quaternion.LookRotation(this.transform.position - wantedCameraPosition);
        Quaternion currentCameraRotation = this.cameraControl.cameraTransform.rotation;

        float xAxis = Input.GetAxisRaw(this.cameraControl.horizontalControl.axisName);
        float yAxis = Input.GetAxisRaw(this.cameraControl.verticalControl.axisName);
        if (this.type == InputType.Mouse) {
            Vector3 mousePoint = Input.mousePosition;
            if (Mathf.Abs(mousePoint.x - Screen.width / 2) > this.mouseRotationHorizontalThreshold) {
                xAxis += ((mousePoint.x - Screen.width / 2.0f) / (Screen.width / 2.0f)) * mouseRotationHorizontalSpeed;
            }
            if (Mathf.Abs(mousePoint.y - Screen.height / 2) > this.mouseRotationVerticalThreshold) {
                yAxis += ((mousePoint.y - Screen.width / 2.0f) / (Screen.height / 2.0f)) * mouseRotationVerticalSpeed;
            }
        }
        if (this.cameraControl.horizontalControl.controllEnabled && Mathf.Abs (xAxis) > 0.1) {
            this.wantedCameraAngle.y += xAxis * this.cameraControl.horizontalControl.speed * (this.cameraControl.horizontalControl.inverse ? 1 : -1);
            float min = this.cameraControl.horizontalControl.minAngle;
            float max = this.cameraControl.horizontalControl.maxAngle;
            if (min > -180 || max < 180) {
                this.wantedCameraAngle.y = Mathf.Clamp(this.wantedCameraAngle.y, min, max);
            }
        }
        if (this.cameraControl.verticalControl.controllEnabled && Mathf.Abs (yAxis) > 0.1) {
            this.wantedCameraAngle.x += yAxis * this.cameraControl.verticalControl.speed * (this.cameraControl.verticalControl.inverse ? 1 : -1);
            float min = this.cameraControl.verticalControl.minAngle;
            float max = this.cameraControl.verticalControl.maxAngle;
            this.wantedCameraAngle.x = Mathf.Clamp(this.wantedCameraAngle.x, min, max);
        }
        
        
        Vector3 cameraPosition = Vector3.Lerp(currentCameraPosition, wantedCameraPosition, this.cameraControl.damping * Time.deltaTime);
        Quaternion cameraRotation = Quaternion.Lerp(currentCameraRotation, wantedCameraRotation, this.cameraControl.rotationDamping * Time.deltaTime);
        
        if (this.cameraControl.rigidbodyEnabled && this.cameraControl.cameraTransform.gameObject.rigidbody != null) {
            this.cameraControl.cameraTransform.gameObject.rigidbody.MovePosition(cameraPosition);
            this.cameraControl.cameraTransform.gameObject.rigidbody.MoveRotation(cameraRotation);
        } else {
            if (this.cameraControl.rigidbodyEnabled) {
                this.cameraControl.cameraTransform.rigidbody.MovePosition(cameraPosition); 
                this.cameraControl.cameraTransform.rigidbody.MoveRotation(cameraRotation); 
            } else {
                this.cameraControl.cameraTransform.position = cameraPosition; 
                this.cameraControl.cameraTransform.rotation = cameraRotation;
            }
        } 
    }
    
    public void SetWantedCameraAngle (Vector3 v) {
        this.wantedCameraAngle = v;
    }
    
    public Vector3 GetWantedCameraAngle () {
        return this.wantedCameraAngle;
    }
    
    public void SetInputType (InputType t) {
        this.type = t;
    }
    
    public Vector3 GetVelocity () {
        return this.velocity;
    }
}
