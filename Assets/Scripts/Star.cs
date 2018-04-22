using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour {

	private float direction;
	private float speed;
	private float angle;
	private float offset;

	public Transform shine;

	// Use this for initialization
	void Start () {
		direction = Random.value < 0.5f ? -1f : 1f;
		speed = Random.Range (50f, 100f);
		angle = Random.Range (0f, 360f);
		offset = Random.value * 1000f;
		UpdateAngle ();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateAngle ();

		shine.localScale = Vector3.one * (Mathf.Abs (Mathf.Sin (Time.time * 2f + offset)) + 1f) * 0.5f;
	}

	void UpdateAngle() {
		angle += speed * Time.deltaTime * direction;
		transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle));
	}

	public void Grab() {
		Invoke ("Explode", 0.25f);
	}

	public void Explode() {

		Manager.Instance.cam.BaseEffect (2f);

		AudioManager.Instance.PlayEffectAt (21, transform.position, 1.5f);
		AudioManager.Instance.PlayEffectAt (4, transform.position, 1.5f);
		AudioManager.Instance.PlayEffectAt (12, transform.position, 1.5f);
		AudioManager.Instance.PlayEffectAt (22, transform.position, 1.5f);
		AudioManager.Instance.PlayEffectAt (25, transform.position, 1.5f);

		EffectManager.Instance.AddEffect (0, transform.position);
		EffectManager.Instance.AddEffect (1, transform.position);
		EffectManager.Instance.AddEffect (4, transform.position);

		gameObject.SetActive (false);
		Manager.Instance.enableThese.Add (gameObject);
	}
}
