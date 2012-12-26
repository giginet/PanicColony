using UnityEngine;
using System.Collections;

public class LogoController : MonoBehaviour {
    
    private float alpha = 0.0f;
    private float timer = 0.0f;
    private Texture2D logoTexture = null;

    // Use this for initialization
    void Start () {
        this.logoTexture = (Texture2D)Resources.Load("UI/kawaz", typeof(Texture2D));
    }
    
    // Update is called once per frame
    void Update () {
        this.timer += Time.deltaTime;
        if (this.timer < 2.0f) {
            this.alpha += 1.0f * Time.deltaTime / 2.0f;
        } else if (4.0f < this.timer && this.timer < 6.0f) {
            this.alpha -= 1.0f * Time.deltaTime / 2.0f;
        } else if (8.0f < this.timer) {
            Application.LoadLevel("TitleScene");
        }
        if (Input.anyKeyDown) {
            Application.LoadLevel("TitleScene");
        }
    
    }
    
    void OnGUI () {
        Color previous = GUI.color;
        Color color = new Color(previous.r, previous.g, previous.b, alpha);
        GUI.color = color;
        float width = this.logoTexture.width;
        float height = this.logoTexture.height;
        GUI.DrawTexture(new Rect((Screen.width - width) / 2.0f - 20, (Screen.height - height) / 2.0f + 20, width, height), this.logoTexture);
        GUI.color = previous;
    }
}
