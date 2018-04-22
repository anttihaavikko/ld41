using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour {

	public Card treeFirst;
	public CardHolder stack;
	public Transform startPoint;
	private bool processing = false;
	public Transform guide1, guide2;

	public GameObject gridItem;

	public List<int> numbers;
	public List<GameObject> enableThese;

	public HelpMessage[] tutorials;
	public GameObject restartButton;

	private bool canRestart = true;

	private int level = 0;
	private int stars = 0;

	public Level[] levels;

	public Dimmer dimmer;
	public Slider lengthSlider;

	public CustomButton startButton;

	public EffectCamera cam;

	private static Manager instance = null;
	public static Manager Instance {
		get { return instance; }
	}

	void Awake() {
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
			return;
		} else {
			instance = this;
		}

		numbers = new List<int> ();
		enableThese = new List<GameObject> ();

//		AddGrid (startPoint.position + Vector3.up * 1.5f * 0.75f, 0);
	}

	void Start() {
		level = StatsManager.Instance.level;
		ActivateLevel ();
	}

	void AddGrid(Vector3 pos, int count) {
		Instantiate (gridItem, pos, Quaternion.identity);
		if(count < 5) {
			AddGrid (pos + Vector3.left + Vector3.up * 1.5f, count+1);
			AddGrid (pos + Vector3.right + Vector3.up * 1.5f, count+1);
		}
	}

	void Update() {

		StatsManager.Instance.AddTime (Time.deltaTime);

		if (Input.GetKeyDown (KeyCode.Space)) {
			StartProcessing ();
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			dimmer.FadeIn (1f);
			CancelInvoke ("BackToStart");
			Invoke ("BackToStart", 1.1f);
		}

		if (Input.GetKeyDown (KeyCode.R))
			Restart ();

		if (Application.isEditor) {
			Time.timeScale = Input.GetKey (KeyCode.Tab) ? 5f : 1f;

			if (Input.GetKeyDown (KeyCode.KeypadPlus))
				EndLevel ();
		}
	}

	void BackToStart() {
		SceneManager.LoadSceneAsync ("Start");
	}

	public void GetStar() {
		stars++;
	}

	public void Restart() {

		if (canRestart) {

			StatsManager.Instance.totalFails++;

			stars = 0;

			canRestart = false;

			HideTutorial ();
			stack.ClearHand ();

			if (treeFirst) {
				treeFirst.Explode ();
			}

			CancelInvoke ("DelayedRestart");
			Invoke ("DelayedRestart", 1f);

			Invoke ("EnableRestart", 2f);
		}
	}

	void EnableRestart() {
		canRestart = true;
	}

	private void DelayedRestart() {
		processing = false;
		stack.SpawnNewHand ();

		AudioManager.Instance.Highpass (false);

		startButton.SetEnabled(true);

		foreach (GameObject g in enableThese) {
			g.SetActive (true);
			EffectManager.Instance.AddEffect (1, g.transform.position);
		}

		enableThese.Clear ();
	}

	public void StartProcessing() {
		if (!processing) {

			startButton.SetEnabled(false);

			processing = true;
			ProcessNext ();

			if (level == 0) {
				HideTutorial (0);
				HideTutorial (1);
			}

			if (level == 1) {
				HideTutorial (6);
			}

			if (level == 3) {
				HideTutorial (7);
			}

			if (level == 7) {
				HideTutorial (8);
			}
		}
	}

	void LevelEnds() {
		dimmer.FadeIn (1f);
	}

	void EndLevel() {
		EveryoneEmote (Face.Emotion.Happy);

		Invoke ("NextLevel", 2.5f);
		Invoke ("LevelEnds", 1f);

		cam.Chromate (0.25f * 4f, 0.1f * 4f);

		AudioManager.Instance.Lowpass (true);
		AudioManager.Instance.PlayEffectAt (28, Vector3.zero, 2f);
	}

	public void ProcessNext() {

		Card c = stack.RemoveFirst();

		if (!c) {
			
			if (IsCurrentLevelComplete ()) {

				EndLevel ();

			} else {
				ShowRestart ();
				AudioManager.Instance.Highpass (true);
				AudioManager.Instance.PlayEffectAt (18, Vector3.zero, 0.5f);
				AudioManager.Instance.PlayEffectAt (26, Vector3.zero, 0.5f);
			}

			return;
		}

		float height = 1.5f;

		Vector3 pos;

		c.AddMove (guide1.position, true);
		c.AddMove (guide2.position, true);
		c.AddMove (startPoint.position, true);

		bool addExtraDelay = false;

		if (!treeFirst) {

			if (level == 0) {
				ShowTutorial (3, 0.5f);
				Invoke ("DelayedTutorialHide", 3.5f);
				c.delayBeforeNext = 3f;
			}

			treeFirst = c;
			pos = startPoint.position + new Vector3 (0f, height * 0.75f, 0f);
			c.SetLineRoot (startPoint.position);
		} else {
			Card parent = treeFirst.AddLink (c);
			float mod = c.number < parent.number ? -1f : 1f;
			pos = parent.transform.position + new Vector3 (1f * mod * lengthSlider.value, height * lengthSlider.value, 0f);
		}

		pos.z = 0;

		c.AddMove (pos, false);

		c.NextMove (0);
	}

	public bool CanInteract() {
		return !processing;
	}

	public int GenerateNewNumber() {
		int num = Random.Range (1, 99);
		while (numbers.Contains (num)) {
			num = Random.Range (1, 99);
		}
		numbers.Add (num);
		return num;
	}

	public void ResetNumbers() {
		numbers.Clear ();
	}

	public void ShowTutorial(int i, float delay = 0f) {
		tutorials [i].Show (delay);
	}

	public void HideTutorial(int i = -1, float delay = 0f) {

		if (i == -1) {
			CancelInvoke ("DelayedTutorialHide");

			foreach (HelpMessage hm in tutorials) {
				hm.Hide ();
			}
		} else {
			tutorials [i].Hide (delay);
		}
	}

	public void ShowRestart() {

		Invoke ("EveryoneSad", 1.2f);

		if (!restartButton.activeSelf) {
			restartButton.SetActive (true);
			ShowTutorial (2);
		}
	}

	public void ShowStartTutorials() {
		if (level == 0) {
			Manager.Instance.ShowTutorial (0, 1.5f);
			Manager.Instance.ShowTutorial (1, 3f);
		}
		if (level == 1) {
			Manager.Instance.ShowTutorial (6, 1f);
		}
		if (level == 3) {
			Manager.Instance.ShowTutorial (7, 1f);
		}
		if (level == 7) {
			Manager.Instance.ShowTutorial (8, 1f);
		}
	}

	public bool ShowMoveTutorial(Vector3 pos, Vector3 nextPos) {
		if (level == 0 && Mathf.Abs(pos.x - nextPos.x) > 0.1f) {
			int tutMes = pos.x > nextPos.x ? 5 : 4;
			tutorials [tutMes].transform.position = nextPos + Vector3.right;
			ShowTutorial (tutMes);
			Invoke ("DelayedTutorialHide", 3.5f);
			return true;
		}

		return false;
	}

	public void DelayedTutorialHide() {
		HideTutorial ();
	}

	public bool IsCurrentLevelComplete() {
		return levels [level].IsComplete (stars);
	}

	public void NextLevel() {

		level++;
		if (level >= levels.Length) {
			level = 0;
			SceneManager.LoadSceneAsync ("End");
			return;
		}

		AudioManager.Instance.Lowpass (false);

		dimmer.FadeOut (0.5f);

		HideTutorial ();

		if (treeFirst) {
			treeFirst.JustRemove ();
			treeFirst = null;
		}
		

		processing = false;

		ActivateLevel ();
	}

	public void ActivateLevel() {

		StatsManager.Instance.level = level;

		stars = 0;

		startButton.SetEnabled(true);

		lengthSlider.value = 1f;
		lengthSlider.gameObject.SetActive(levels [level].sliderEnabled);

		foreach (Level l in levels) {
			l.gameObject.SetActive (false);
		}

		levels [level].gameObject.SetActive (true);

		stack.numberOfCards = levels [level].cards;

		stack.ClearHand (true);
		stack.SpawnNewHand ();
	}

	public void EveryoneEmote(Face.Emotion emotion) {
		if (treeFirst) {
			treeFirst.ChainEmote (emotion);
		}

		stack.AllEmote (emotion);
	}

	public void EveryoneSad() {
		EveryoneEmote (Face.Emotion.Sad);
	}
}
