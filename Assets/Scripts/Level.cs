using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {
	public int stars = 0;
	public int cards = 1;

	public bool IsComplete(int s) {
		return (s >= stars);
	}
}
