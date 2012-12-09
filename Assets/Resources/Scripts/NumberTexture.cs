using UnityEngine;
using System.Collections;

public class NumberTexture {
    private Texture2D numberAtlasTexture = null;
    private Texture2D numberTexture = null;
    private int width = 0;
    private int height = 0;
    private Rect rect;
    private int number = 0;
    private float scale = 1.0f;
    
    public NumberTexture (string atlasName, int width, int height) {
        this.numberAtlasTexture = (Texture2D)Resources.Load(atlasName, typeof(Texture2D));
        this.width = width;
        this.height = height;    
        this.scale = 1.0f;
    }
    
    public Texture2D GetTexture () {
        return this.numberTexture;
    }
    
    public Rect GetRect () {
        return this.rect;
    }
    
    public float GetAspectRatio () {
        return this.rect.width / this.rect.height;
    }
    
    public void SetNumber (int n) {
        this.number = n;
    }
    
    public void Draw(Vector2 position, int number) {
        this.SetNumber(number);
        this.Draw(position);
    }
    
    public void SetScale (float scale) {
        this.scale = scale;
    }
    
    public void Draw(Vector2 position) {
        string num = this.number.ToString();
        int order = num.Length;
        for (int i = 0; i < order; ++i) {
            int n = (int)char.GetNumericValue(num[i]);
            Rect dest = new Rect(position.x + this.width * i, position.y, this.width * this.scale, this.height * this.scale);
            Rect src = new Rect(n * 0.1f, 0.0f, 0.1f, 1.0f);
            GUI.DrawTextureWithTexCoords(dest, this.numberAtlasTexture, src);
        }
    } 
}
