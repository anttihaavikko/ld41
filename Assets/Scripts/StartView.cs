using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartView : MonoBehaviour {

	private bool canGo = false;
	public Dimmer dimmer;
	public EffectCamera cam;

	public CardHolder leftHolder, rightHolder;

	// Use this for initialization
	void Start () {
		Invoke ("EnableGo", 1.5f);
		SceneManager.LoadSceneAsync ("Options", LoadSceneMode.Additive);

		AudioManager.Instance.Lowpass (false);

		Invoke ("ExecuteCard", 45f);
		Invoke ("FakeEnd", 110f);
	}
	
	void EnableGo() {
		canGo = true;
	}

	// Update is called once per frame
	void Update () {
		if (canGo && Input.GetKeyDown (KeyCode.Escape)) {
			if (Application.platform != RuntimePlatform.WebGLPlayer) {
				dimmer.FadeIn (1f);
				CancelInvoke ("DoQuit");
				Invoke ("DoQuit", 1.1f);
				return;
			}
		}

		if (Application.isEditor) {
			Time.timeScale = Input.GetKey (KeyCode.Tab) ? 5f : 1f;
		}
	}

	void ChangeScene() {
		SceneManager.LoadSceneAsync ("Main");
	}

	void DoQuit() {
		Application.Quit ();
	}

	public void StartGame() {
		CancelInvoke ("ChangeScene");
		Invoke ("ChangeScene", 1.1f);
		dimmer.FadeIn (1f);
		AudioManager.Instance.PlayEffectAt (28, Vector3.zero, 2f);
		AudioManager.Instance.Lowpass (true);
		cam.Chromate (0.25f * 4f, 0.1f * 4f);
	}

	public void ExecuteCard() {
		var holder = leftHolder.CardCount() > rightHolder.CardCount() ? leftHolder : rightHolder;

		if (holder.CardCount () > 0) {
			Card c = holder.RemoveRandom ();
			c.transform.position = new Vector3 (c.transform.position.x, c.transform.position.y, -0.25f);
			c.shadow.enabled = true;
			c.Explode (-0.25f);
		}

		Invoke ("ExecuteCard", 5f);
	}

	void FakeEnd() {
		dimmer.FadeIn (1f);
		StatsManager.Instance.altEnding = true;
		AudioManager.Instance.PlayEffectAt (28, Vector3.zero, 2f);
		AudioManager.Instance.Lowpass (true);
		cam.Chromate (0.25f * 4f, 0.1f * 4f);
		Invoke ("GoFakeEnd", 1.1f);
	}

	void GoFakeEnd() {
		SceneManager.LoadSceneAsync ("End");
	}
}
