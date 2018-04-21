using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {

	private bool dragging = false;
	private float dragTime = 0f;
	private Vector3 dragPoint;

	public CardHolder currentHolder;

	private Vector3 startPoint;
	private Vector3 originalScale;

	private Collider2D coll;

	public LayerMask areaMask;
	private SortingLayer defaultLayer;

	public SpriteRenderer sprite;

	private float height = 0f;

	private Vector3 fromPosition, toPosition;
	private float moveDuration = -1f;

	private float moveSpeed = 3.5f;
	private float normalMoveSpeed = 3.5f;

	public SpriteRenderer shadow;

	public bool isMatrix = true;
	private int operation = -1;
	public SpriteRenderer operationSprite;

	private Vector3 shadowScale;

	public TextMesh numberText;

	public int number;

	public Card leftLink, rightLink;

	public LineRenderer line;

	public List<Vector3> moves;
	private int nextMove = -1;

	// Use this for initialization
	void Start () {
		originalScale = transform.localScale;
		coll = GetComponent<Collider2D> ();

		shadowScale = shadow.transform.localScale;

		number = Random.Range (1, 99);
		numberText.text = number.ToString();

		moves = new List<Vector3> ();

//		sprite.color = new Color (Random.Range (0.5f, 1f), Random.Range (0.5f, 1f), Random.Range (0.5f, 1f));
	}

	public void SetSpeed(float s) {
		moveSpeed = s;
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 lastPos = transform.position;

		if (dragging) {

			dragTime += Time.deltaTime;

			Vector3 mousePos = Input.mousePosition;
			mousePos.z = -Camera.main.transform.position.z + height;
			mousePos = Camera.main.ScreenToWorldPoint (mousePos);

			transform.position = new Vector3 (mousePos.x, mousePos.y, height * 0.5f) + dragPoint;

			if (LeftArea (1f) && currentHolder) {
				currentHolder.RemoveCard (this);
			}
		} else {
			transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.Euler (Vector3.zero), 1f);
		}

		if (moveDuration >= 0f && moveDuration <= 1f) {
			moveDuration += Time.deltaTime * moveSpeed;
			transform.position = Vector3.Lerp (fromPosition, toPosition, moveDuration);
		}

		Tilt (lastPos, transform.position);
		float offset = dragging ? 0.1f : 0f;

		shadow.transform.position = new Vector3 (transform.position.x, transform.position.y, dragging ? -0.5f : 0f);
		shadow.transform.localScale = dragging ? shadowScale * 1.1f : shadowScale;

		line.SetPosition (0, transform.position);
	}

	private void Tilt(Vector3 prevPos, Vector3 curPos) {
		float maxAngle = 10f;

		float xdiff = curPos.x - prevPos.x;
		xdiff = Mathf.Clamp (xdiff * 1000f, -maxAngle, maxAngle);

		float ydiff = curPos.y - prevPos.y;
		ydiff = Mathf.Clamp (ydiff * 1000f, -maxAngle, maxAngle);

		transform.rotation = Quaternion.RotateTowards (transform.rotation, Quaternion.Euler (new Vector3 (-ydiff, xdiff, 0)), 0.5f);
	}

	public void OnMouseEnter() {
//		CursorManager.Instance.hovering = true;
	}

	public void OnMouseExit() {
//		CursorManager.Instance.hovering = false;
	}

	public void OnMouseDown() {

		moveSpeed = normalMoveSpeed + Random.Range(-0.5f, 0.5f);

		if (!Manager.Instance.CanInteract ()) {
			return;
		}

//		AudioManager.Instance.PlayEffectAt (3, transform.position, 0.5f);

		dragTime = 0f;
		SetHeight (true);

		startPoint = transform.position;

		Vector3 point = Camera.main.ScreenPointToRay (Input.mousePosition).GetPoint(-Camera.main.transform.position.z - height);
		point.z = 0;

		dragPoint = Vector3.zero;

		sprite.sortingLayerName = "Lifted";
	}

	public void SetHeight(bool raised) {
		dragging = raised;
		dragTime = 0f;
		height = raised ? -1f : 0f;
	}

	public void OnMouseUp() {

		if (!Manager.Instance.CanInteract ()) {
			return;
		}

		SetHeight (false);
			
		currentHolder.AddCard (this, false);

		sprite.sortingLayerName = "Default";
	}

	public void UseCard(float delay = 0f) {
		Invoke ("UseCard", delay);
		Invoke ("UseSound", delay);
	}

	public void UseSound() {
//		AudioManager.Instance.PlayEffectAt (3, transform.position, 0.5f);
	}

	public void UseCard() {
//		int type = isMatrix ? 0 : 1;
//		currentHolder.RemoveCard (this);
//		currentHolder.targetHolders[type].AddCard (this, false);

//		AudioManager.Instance.PlayEffectAt (6, transform.position, 0.5f);
	}

	private bool LeftArea(float distance) {
		return (transform.position - startPoint).magnitude > distance;
	}

	public void Move(Vector3 pos) {
		fromPosition = transform.position;
		toPosition = pos;
		moveDuration = 0f;
	}

	public int GetOperation() {
		return operation;
	}

	public Card AddLink(Card c) {
		Card parent = null;

		c.AddMove (transform.position, true);

		c.SetLineRoot (transform.position);

		if (c.number < number) {
			parent = leftLink ? leftLink.AddLink (c) : this;
			leftLink = leftLink ? leftLink : c;
		} else {
			parent = rightLink ? rightLink.AddLink (c) : this;
			rightLink = rightLink ? rightLink : c;
		}

		return parent;
	}

	public void AddMove(Vector3 pos, bool raised = false) {
		moves.Add (pos + new Vector3(0, 0, raised ? -1f : 0));
	}

	public void SetLineRoot(Vector3 pos) {
		line.SetPosition (1, pos);
	}

	public void DoMove() {
		NextMove (nextMove);
	}

	public void NextMove(int i) {
//		transform.position = moves [i];

		Tweener.Instance.MoveTo (transform, moves [i], 0.3f, 0f, TweenEasings.QuadraticEaseIn);

		nextMove = i + 1;
		if (nextMove < moves.Count) {
			Invoke ("DoMove", 0.3f);
		} else {
			line.enabled = true;
			shadow.enabled = false;
		}
	}


}
