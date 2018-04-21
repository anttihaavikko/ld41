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
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.Space)) {
			ProcessNext ();
		}

		if (Application.isEditor && Input.GetKeyDown (KeyCode.R)) {
			SceneManager.LoadScene ("Main");
		}
	}

	void ProcessNext() {
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

		c.AddMove (pos, false);
		c.NextMove (0);
	}

	public bool CanInteract() {
		return !processing;
	}


}
