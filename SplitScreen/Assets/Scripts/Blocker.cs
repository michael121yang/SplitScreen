using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blocker : MonoBehaviour {
	private Rigidbody2D rb2d;
	public GameObject PowerSource;

	public bool blocker;

	private bool powered = false;
	private bool active = true;

	private float time = Mathf.Infinity;

	// Use this for initialization
	void Start () {
		powered = !(PowerSource == null);

		rb2d = GetComponent<Rigidbody2D> ();

		if (gameObject.layer != LayerMask.NameToLayer("Water")) {
			if (blocker) {
				gameObject.layer = LayerMask.NameToLayer("Blockers");
				gameObject.GetComponent<Renderer>().material.color = new Color(.8f, .2f, .2f);
				gameObject.GetComponent<SpriteRenderer>().color = new Color(.8f, .2f, .2f);
			} else {
				gameObject.layer = LayerMask.NameToLayer("Uncuttables");
				gameObject.GetComponent<Renderer>().material.color = new Color(.7f, .7f, 1);
				gameObject.GetComponent<SpriteRenderer>().color = new Color(.7f, .7f, 1);
			}
		}
	}

	void Update() {
		if (powered) {
			if (PowerSource == null && active) {
				enableCutting();
			}
			if (!active && !GameManager.Instance.cut) {
				GameObject[] objs = GameManager.Instance.cutter.GetComponent<LineMaker>().objects;
				GameObject[] temp = new GameObject[objs.Length + 1];
				objs.CopyTo(temp, 0);
				temp[objs.Length] = gameObject;
				GameManager.Instance.cutter.GetComponent<LineMaker>().objects = temp;
				this.enabled = false;
			}
		}
	}

	public void enableCutting () {
		if (powered) {
			if (active) {
				if (gameObject.layer != LayerMask.NameToLayer("TransparentFX")) {
					gameObject.layer = LayerMask.NameToLayer("Default");
				}
				GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
				GetComponent<Renderer>().material.color = new Color(1, 1, 1);

				gameObject.AddComponent<Split>();
				gameObject.tag = "cuttableLevel";

			}
			active = false;
		}
	}

	// void FixedUpdate () {
	// 	if (rb2d.velocity.magnitude < .1 && rb2d.velocity.magnitude > 0) {
	// 		rb2d.velocity = Vector2.zero;
	// 	} else if (rb2d.velocity.magnitude != 0) {
	// 		rb2d.velocity *= .9f;
	// 	} 
	// }
}