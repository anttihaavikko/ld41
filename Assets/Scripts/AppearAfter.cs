using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearAfter : MonoBehaviour {

	public float delay = 0f;
	private Vector3 fullScale;
	public bool requiresNormalEnding = false;

	void Awake() {
		fullScale = transform.localScale;
	}

	// Use this for initialization
	void Start () {
		transform.localScale = Vector3.zero;

		if (!requiresNormalEnding || !StatsManager.Instance.altEnding) {
			Tweener.Instance.ScaleTo (transform, fullScale, 0.3f, delay, TweenEasings.BounceEaseOut);
			Invoke ("DoSound", delay);
		}
	}

	void DoSound() {
		AudioManager.Instance.PlayEffectAt (20, transform.position, 1.5f);
		AudioManager.Instance.PlayEffectAt (22, transform.position, 1.5f);
		AudioManager.Instance.PlayEffectAt (4, transform.position, 0.5f);
	}
}
