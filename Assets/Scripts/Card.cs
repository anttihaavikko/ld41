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
	public SpriteRenderer colorSprite;

	private Vector3 shadowScale;

	public TextMesh numberText;

	public int number;

	public Card leftLink, rightLink;

	public LineRenderer line;

	public List<Vector3> moves;
	private int nextMove = -1;
	private bool shaking = false;
	private float shakeAmount = 0.1f;

	public Face face;

	public LayerMask defaultMask, collisionMask;

	public float delayBeforeNext = 0f;

	private Vector2 colliderSize;

	// Use this for initialization
	void Start () {
		originalScale = transform.localScale;
		coll = GetComponent<Collider2D> ();

		shadowScale = shadow.transform.localScale;

		number = Manager.Instance.GenerateNewNumber();
		numberText.text = number.ToString();

		moves = new List<Vector3> ();

		colorSprite.color = Color.HSVToRGB (Random.value, 0.5f, 0.99f);

		colliderSize = GetComponent<BoxCollider2D> ().bounds.size;
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

		shadow.transform.position = new Vector3 (transform.position.x, transform.position.y, dragging ? -0.25f : 0f);
		shadow.transform.localScale = dragging ? shadowScale * 1.1f : shadowScale;

		line.SetPosition (0, transform.position);

		if (shaking) {
			float amt = 0.1f;
			shakeAmount += Time.deltaTime;
			transform.position += new Vector3 (Random.Range (-amt, amt) * shakeAmount, Random.Range (-amt, amt) * shakeAmount, 0f);
			transform.rotation = Quaternion.Euler(new Vector3 (0, 0, Random.Range (-5f, 5f) * shakeAmount));
		}
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

		face.Emote (Face.Emotion.Shocked);
		AudioManager.Instance.PlayEffectAt (11, transform.position, 1f);
	}

	public void SetHeight(bool raised) {
		dragging = raised;
		dragTime = 0f;
		height = raised ? -1f : 0f;
	}

	public void OnMouseUp() {

		if (!Manager.Instance.CanInteract ()) {
			
			if (Application.isEditor && Input.GetKey(KeyCode.K))
				Explode ();
			
			return;
		}

		SetHeight (false);
			
		currentHolder.AddCard (this, false);

		sprite.sortingLayerName = "Default";

		face.Emote (Face.Emotion.Happy, Face.Emotion.Default, 1f);
		AudioManager.Instance.PlayEffectAt (24, transform.position, 1f);
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
		Collider2D hit = Physics2D.OverlapBox (pos, colliderSize, 0, defaultMask);
		raised = hit ? true : raised;
		moves.Add (pos + new Vector3(0, 0, raised ? -1f : -0.01f));
	}

	public void SetLineRoot(Vector3 pos) {
		line.SetPosition (1, pos);
	}

	public void DoMove() {
		NextMove (nextMove);
	}

	public void MoveSound() {
		AudioManager.Instance.PlayEffectAt (23, transform.position, 0.7f);
	}

	public void NextMove(int i) {
//		transform.position = moves [i];

		face.Emote (Face.Emotion.Shocked);

		nextMove = i + 1;

//		Debug.Log ("Moving to " + moves [i]);

//		float lenMod = nextMove < moves.Count ? 1f : Manager.Instance.lengthSlider.value;

		Tweener.Instance.MoveTo (transform, moves [i], 0.3f, 0f, TweenEasings.QuadraticEaseIn);

		if ((transform.position - moves [i]).magnitude > 1f) {
			Invoke ("MoveSound", 0.25f);
		}

		if (nextMove < moves.Count) {
			Invoke ("DoMove", 0.3f);
		} else {

			line.enabled = true;

			Collider2D hit = Physics2D.OverlapBox (moves [i], colliderSize, 0);

			if (hit && hit.tag == "Star") {

				hit.GetComponent<Star> ().Grab ();
				hit = null;
				Manager.Instance.GetStar ();
			}
				
			hit = Physics2D.OverlapBox (moves [i], GetComponent<BoxCollider2D> ().bounds.size, 0, collisionMask);

			if (hit && i > 3) {
//				Debug.Log ("Hit " + hit.gameObject.name);
				Explode ();

				Manager.Instance.ShowRestart ();

				var other = hit.GetComponent<Card> ();
				if (other) {
					other.Explode (0.5f);
				}

				if (hit.tag == "Block") {
					hit.GetComponentInChildren<Face> ().Emote (Face.Emotion.Angry, Face.Emotion.Brag, 1.75f);
				}

			} else {

				float extraDelay = Manager.Instance.ShowMoveTutorial (transform.position, moves [i]) ? 3.5f : 0f;

				shadow.enabled = false;
				Invoke ("Squish", 0.3f);
				Invoke ("NextCard", 0.5f + delayBeforeNext + extraDelay);
				face.Emote (Face.Emotion.Happy, Face.Emotion.Default, 1f);
			}
		}
	}

	public void Squish() {
		EffectManager.Instance.AddEffect (2, transform.position);
		Tweener.Instance.ScaleTo (transform, Vector3.one * 0.9f, 0.2f, 0f, TweenEasings.BounceEaseOut);
		AudioManager.Instance.PlayEffectAt (17, transform.position, 0.75f);
		AudioManager.Instance.PlayEffectAt (24, transform.position, 0.5f);
	}

	private void NextCard() {
		Manager.Instance.ProcessNext ();
	}

	public void Explode(float delay = 0f) {

		face.Emote (Face.Emotion.Angry);

		if (leftLink)
			leftLink.Explode (delay);
		
		if (rightLink)
			rightLink.Explode (delay);

		Invoke ("Shake", 0.3f + delay);
		Invoke ("ExplodeNow", 1.1f + delay);
	}

	public void Shake() {
		shaking = true;
		AudioManager.Instance.PlayEffectAt (7, transform.position, 1.0f);
		AudioManager.Instance.PlayEffectAt (1, transform.position, 1.0f);
		AudioManager.Instance.PlayEffectAt (26, transform.position, 1.0f);

		AudioManager.Instance.Highpass (true);
	}

	public void ExplodeNow() {

		Manager.Instance.cam.BaseEffect (2f);

		AudioManager.Instance.PlayEffectAt (3, transform.position, 1.0f);
		AudioManager.Instance.PlayEffectAt (5, transform.position, 1.0f);
		AudioManager.Instance.PlayEffectAt (0, transform.position, 1.0f);
		
		Vector3 pos = transform.position;
		pos.z = 0;

		EffectManager.Instance.AddEffect (0, pos);
		EffectManager.Instance.AddEffect (1, pos);

		ParticleSystem colorBits = EffectManager.Instance.AddEffect (3, pos).GetComponent<ParticleSystem>();
		ParticleSystem.MainModule mm = colorBits.main;
		mm.startColor = colorSprite.color;

		Destroy (gameObject);
	}

	public void JustRemove() {
		Destroy (gameObject);

		if (leftLink)
			leftLink.JustRemove ();

		if (rightLink)
			rightLink.JustRemove ();
	}
}
