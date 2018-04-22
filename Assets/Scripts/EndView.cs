using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndView : MonoBehaviour {

	public TextMesh totalTime, totalFails, thanksText;
	private bool canGo = false;
	public Dimmer dimmer;
	public EffectCamera cam;

	// Use this for initialization
	void Start () {
		totalTime.text = StatsManager.Instance.GetTime ();
		totalFails.text = StatsManager.Instance.totalFails.ToString();
		Invoke ("EnableGo", 1.5f);

		if (StatsManager.Instance.altEnding) {
			totalTime.text = "FEW MINUTES";
			totalFails.text = "∞";
			thanksText.text = "THANKS FOR \"PLAYING\"!";
		}

		StatsManager.Instance.ResetStats ();
	}

	void EnableGo() {
		canGo = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (canGo && Input.anyKeyDown) {
			CancelInvoke ("ChangeScene");
			Invoke ("ChangeScene", 1.1f);
			dimmer.FadeIn (1f);
			AudioManager.Instance.PlayEffectAt (28, Vector3.zero, 2f);
			cam.Chromate (0.25f * 4f, 0.1f * 4f);
		}
	}

	void ChangeScene() {
		SceneManager.LoadSceneAsync ("Start");
	}
}
