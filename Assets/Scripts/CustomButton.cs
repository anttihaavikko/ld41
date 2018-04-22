using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomButton : MonoBehaviour {

	private SpriteRenderer sprite;
	private Vector3 fullScale;

	public UnityEvent onClick;

	void Start() {
		sprite = GetComponent<SpriteRenderer> ();
		fullScale = transform.localScale;
	}

	public void OnMouseEnter() {
		Tweener.Instance.ScaleTo (transform, fullScale * 1.1f, 0.4f, 0f, TweenEasings.BounceEaseOut);
	}

	public void OnMouseExit() {
		Tweener.Instance.ScaleTo (transform, fullScale, 0.4f, 0f, TweenEasings.BounceEaseOut);
	}

	public void OnMouseDown() {
		sprite.color = new Color (1f, 1f, 1f, 0.5f);
		Tweener.Instance.ScaleTo (transform, fullScale * 1f, 0.25f, 0f, TweenEasings.QuadraticEaseIn, 0);

		AudioManager.Instance.PlayEffectAt (10, transform.position, 1.5f);
		AudioManager.Instance.PlayEffectAt (19, transform.position, 1.5f);
	}

	public void OnMouseUp() {
		AudioManager.Instance.PlayEffectAt (22, transform.position, 1.5f);
		sprite.color = Color.white;

		if (onClick != null)
			onClick.Invoke ();
	}
}
