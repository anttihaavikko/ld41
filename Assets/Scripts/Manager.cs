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

	void AddGrid(Vector3 pos, int count) {
		Instantiate (gridItem, pos, Quaternion.identity);
		if(count < 5) {
			AddGrid (pos + Vector3.left + Vector3.up * 1.5f, count+1);
			AddGrid (pos + Vector3.right + Vector3.up * 1.5f, count+1);
		}
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.Space) && !processing) {
			ProcessNext ();
		}

		if (Application.isEditor) {

			Time.timeScale = Input.GetKey (KeyCode.Tab) ? 5f : 1f;

			if (Input.GetKeyDown (KeyCode.R))
				Restart ();
		}
	}

	public void Restart() {
		stack.ClearHand ();

		if (treeFirst) {
			treeFirst.Explode ();
		}

		Invoke ("DelayedRestart", 1f);
	}

	private void DelayedRestart() {
		processing = false;
		stack.SpawnNewHand ();

		foreach (GameObject g in enableThese) {
			g.SetActive (true);
		}

		enableThese.Clear ();
	}

	public void ProcessNext() {
		processing = true;

		Card c = stack.RemoveFirst();

		if (!c)
			return;

		float height = 1.5f;

		Vector3 pos;

		c.AddMove (guide1.position, true);
		c.AddMove (guide2.position, true);
		c.AddMove (startPoint.position, true);

		if (!treeFirst) {
			treeFirst = c;
			pos = startPoint.position + new Vector3 (0f, height * 0.75f, 0f);
			c.SetLineRoot (startPoint.position);
		} else {
			Card parent = treeFirst.AddLink (c);
			float mod = c.number < parent.number ? -1f : 1f;
			pos = parent.transform.position + new Vector3 (1f * mod, height, 0f);
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
}
