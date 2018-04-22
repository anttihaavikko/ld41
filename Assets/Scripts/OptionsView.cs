using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsView : MonoBehaviour {

	private bool starting = false;

	public Slider musicSlider, soundSlider;
	public RectTransform options;

	private bool optionsOpen = false;
	private bool canQuit = false;

	void Start() {
		soundSlider.value = AudioManager.Instance.volume;
		musicSlider.value = AudioManager.Instance.curMusic.volume;
		GetComponent<Canvas> ().worldCamera = Camera.main;
		GetComponent<Canvas> ().planeDistance = 1;
	}

	void EnableQuit() {
		canQuit = true;
	}

	void DoInputs() {

		if (Input.GetKeyUp (KeyCode.Escape)) {
			canQuit = true;
			return;
		}

		if (!canQuit) {
			return;
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {

			if (optionsOpen) {
				ToggleOptions (false);
			}
				
			return;
		}

		if (Input.GetKeyDown (KeyCode.O) || Input.GetKeyDown (KeyCode.P)) {
			ToggleOptions ();
			return;
		}
	}
	
	// Update is called once per frame
	void Update () {

		DoInputs ();

		float optionsX = optionsOpen ? 0f : 90f;
		options.anchoredPosition = Vector2.Lerp(options.anchoredPosition, new Vector2(optionsX, 0f), Time.deltaTime * 10f);
	}

	public void ChangeMusicVolume() {
		AudioManager.Instance.curMusic.volume = musicSlider.value;
//		AudioManager.Instance.SaveVolumes ();
	}

	public void ChangeSoundVolume() {
		if (soundSlider.value != AudioManager.Instance.volume && Mathf.Abs(soundSlider.value - AudioManager.Instance.volume) > 0.05f) {
			AudioManager.Instance.volume = soundSlider.value;
			AudioManager.Instance.PlayEffectAt (8, Camera.main.transform.position, 1.5f);
//			AudioManager.Instance.SaveVolumes ();
		}
	}

	public void ToggleOptions() {
		ToggleOptions (!optionsOpen);
	}

	public void ToggleOptions(bool state) {
		AudioManager.Instance.PlayEffectAt (10, Vector3.zero, 1.5f);
		AudioManager.Instance.PlayEffectAt (19, Vector3.zero, 1.5f);
		AudioManager.Instance.PlayEffectAt (23, transform.position, 0.7f);
		optionsOpen = state;
	}
}
