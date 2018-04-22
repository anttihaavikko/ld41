using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour {

	private float totalTime = 0f;
	public int level = 0;
	public int totalFails = 0;
	public bool altEnding = false;

	/******/

	private static StatsManager instance = null;
	public static StatsManager Instance {
		get { return instance; }
	}

	void Awake() {
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
			return;
		} else {
			instance = this;
		}

		DontDestroyOnLoad(instance.gameObject);
	}

	public void AddTime(float amount) {
		totalTime += amount;
	}

	public string GetTime() {
		System.TimeSpan t = System.TimeSpan.FromSeconds(totalTime);
		return string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes,t.Seconds);
	}

	public void ResetStats() {
		totalTime = 0f;
		level = 0;
		totalFails = 0;
		altEnding = false;
	}
}
