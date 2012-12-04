using UnityEngine;
using System.Collections;

public class Animation2D {

    private Rect rect;
    private string prefix;
    private int maxFrames;
    private int fps;
    private float time;
    private int currentFrame = 0;
    private bool playing = false;
    private Texture2D currentTexture;

    public Animation2D (Rect rect, string prefix, int maxFrames) {
        this.rect = rect;
        this.prefix = prefix;
        this.maxFrames = maxFrames;
        this.fps = 30;
    }
    
    public void Update() {
        if (this.playing && this.currentFrame < this.maxFrames) {
            time += Time.deltaTime;
            if (time >= 1.0f / (float)this.fps) {
                time = 0;
                this.currentFrame += 1;
                this.LoadNextTexture();
            }
        }
    }
    
    private void LoadNextTexture () {
        string path = prefix + this.currentFrame;
        Texture2D texture = (Texture2D)Resources.Load(path); 
        this.currentTexture = texture;
    }
    
    public Texture2D GetTexture () {
        return this.currentTexture;
    }
    
    public Rect GetRect () {
        return this.rect;
    }
    
    public float GetAspectRatio () {
        return this.rect.width / this.rect.height;
    }
    
    public bool IsPlaying () {
        return this.playing;
    }
    
    public void Play () {
        this.playing = true;
        this.LoadNextTexture();
    }
    
    public void Stop () {
        this.Pause();
        this.Rewind();
    }
    
    public void Rewind () {
        this.currentFrame = 0;
        this.time = 0;
        this.LoadNextTexture();
    }
    
    public void Pause () {
        this.playing = false;
    }
    
}
