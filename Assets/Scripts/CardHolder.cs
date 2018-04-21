using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardHolder : MonoBehaviour {

	private List<Card> cards;
	public int cardMax = 1;

	public int numberOfCards = 1;
	public Card cardPrefab;

	public CardHolder targetHolder;

	// Use this for initialization
	void Start () {
		cards = new List<Card> ();
		SpawnNewHand ();
	}

	public void SpawnNewHand() {
		Manager.Instance.ResetNumbers ();
		for (int i = 0; i < numberOfCards; i++) {
			Invoke ("SpawnCard", i * 0.1f);
		}
	}

	public void ClearHand() {
		foreach (Card c in cards) {
			c.Explode ();
		}
		cards.Clear ();
	}

	public void SpawnCard() {
		if (cards.Count < cardMax) {
			Card c = Instantiate (cardPrefab, transform.position + ((cards.Count + 1) * 0.5f * 1.1f + 5f) * Vector3.up, Quaternion.identity);
			c.transform.localScale = new Vector3 (1f, 1f, 1f);
			AddCard (c, true);
			PositionCards ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Application.isEditor) {
			if (Input.GetKeyDown (KeyCode.Q)) {
				SpawnCard ();
			}

			if (Input.GetKeyDown (KeyCode.W)) {
				PositionCards ();
			}
		}
	}

	public Card RemoveFirst() {
		Card c = null;
		if (cards.Count > 0) {
			c = cards [0];
			cards.RemoveAt (0);
		}
		PositionCards ();
		return c;
	}

	public void RemoveCard(Card c) {

		if (cards.Contains (c)) {
			cards.Remove (c);
		}

		PositionCards ();
	}

	public void AddCard(Card c, bool toEnd) {

		if(!cards.Contains(c)) {

			if (cards.Count >= cardMax) {
				Card swap = cards [0];
				cards.RemoveAt (0);
				swap.currentHolder.targetHolder.AddCard (swap, false);
			}

			int slot = 0;

			if (toEnd) {
				
				cards.Add (c);

			} else {

				for (int i = 0; i < cards.Count; i++) {
					if (c.transform.position.y > cards [i].transform.position.y) {
						slot = i + 1;
					}
				}

				cards.Insert (slot, c);
			}
		}

		c.currentHolder = this;
		PositionCards ();
	}

	public void PositionCards() {
		float areaSize = (cards.Count -1 ) * 0.1f;

		for (int i = 0; i < cards.Count; i++) {
			areaSize += cards [i].transform.localScale.y;
		}

		float curPos = 0f;

		for (int i = 0; i < cards.Count; i++) {
			curPos += cards [i].transform.localScale.y * 0.525f;
			cards [i].Move(transform.position + (-areaSize * 0.5f + curPos) * Vector3.up + Vector3.back * 0.01f);
			curPos += cards [i].transform.localScale.y * 0.5f + 0.1f;
		}
	}

	public int CardCount() {
		return cards.Count;
	}
}
