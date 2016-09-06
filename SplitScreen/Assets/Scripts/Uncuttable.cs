using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Uncuttable : MonoBehaviour {
	private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D>();

		gameObject.layer = LayerMask.NameToLayer("Uncuttables");
		gameObject.GetComponent<SpriteRenderer>().color = new Color(.7f, .7f, 1);
	}

	// void FixedUpdate () {
	// 	if (rb2d.velocity.magnitude < .1 && rb2d.velocity.magnitude > 0) {
	// 		rb2d.velocity = Vector2.zero;
	// 	} else if (rb2d.velocity.magnitude != 0) {
	// 		rb2d.velocity *= .9f;
	// 	} 
	// }
}