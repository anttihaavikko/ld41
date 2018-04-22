using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpMessage : MonoBehaviour {

	Vector3 fullSize;

	// Use this for initialization
	void Awake () {
		fullSize = transform.localScale;
		transform.localScale = Vector3.zero;
//		gameObject.SetActive (false);
	}
	
	public void Show(float delay = 0f) {
		Tweener.Instance.ScaleTo (transform, fullSize, 0.3f, delay, TweenEasings.QuadraticEaseIn);
		Invoke ("DoSound", delay);
	}

	public void Hide(float delay = 0f) {
		Tweener.Instance.ScaleTo (transform, Vector3.zero, 0.3f, delay, TweenEasings.QuadraticEaseIn);
		Invoke ("DoSound", delay);
	}

	void DoSound() {
		AudioManager.Instance.PlayEffectAt (20, transform.position, 1.0f);
		AudioManager.Instance.PlayEffectAt (22, transform.position, 1.0f);
	}
}
