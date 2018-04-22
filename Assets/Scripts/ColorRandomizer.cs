using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRandomizer : MonoBehaviour {

	public SpriteRenderer sprite;
	float hue;
	float dir;

	// Use this for initialization
	void Awake () {
		hue = Random.value;
		dir = Random.value < 0.5f ? -1f : 1f;
		UpdateColor ();
	}
	
	// Update is called once per frame
	void Update () {
		hue += Time.deltaTime * dir * 0.0025f;

		if (hue <= 0f || hue >= 1f) {
			dir = -dir;
			hue = Mathf.Clamp (hue, 0f, 1f);
		}

		UpdateColor ();
	}

	void UpdateColor() {
		Color c = Color.HSVToRGB (hue, 1f, 1f);
		c.a = 0.05f;
		sprite.color = c;
	}
}
