using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level {
	private Dictionary<Vector2, char> map;
	private int level = 0;
	private int width = 0;
	private int height = 0;	
	
	public Level (int level, int width, int height, Dictionary<Vector2, char> map) {
		this.map = map;
		this.width = width;
		this.height = height;
	}
	
	public char GetMap (int x, int y) {
		Vector2 v = new Vector2(x, y);
		if (this.map.ContainsKey(v) ) {
			return this.map[v];
		}
		return ' ';
	}
	
	public int GetLevel () {
		return this.level;
	}
	
	public int GetWidth () {
		return this.width;
	}
	
	public int GetHeight () {
		return this.height;
	}
}
