using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartView : MonoBehaviour {

	private bool canGo = false;
	public Dimmer dimmer;

	// Use this for initialization
	void Start () {
		Invoke ("EnableGo", 1.5f);
		SceneManager.LoadSceneAsync ("Options", LoadSceneMode.Additive);
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
	}
}
