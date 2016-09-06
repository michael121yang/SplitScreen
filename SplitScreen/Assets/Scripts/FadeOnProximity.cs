using UnityEngine;
using System.Collections;

public class FadeOnProximity : MonoBehaviour {

	private bool active;
	private Renderer renderer;

	// Use this for initialization
	void Start () {
		renderer = GetComponent<Renderer>();
		active = true;
	}
	
	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.tag == "Player" ) {
			active = false;
		}
	}

	void OnTriggerExit2D(Collider2D col) {
		if (col.gameObject.tag == "Player" ) {
			active = true;
		}
	}

	void OnSpriteSliced(SpriteSlicer2DSliceInfo sliceInfo) {
		foreach (GameObject c in sliceInfo.ChildObjects) {
			c.AddComponent(this.GetType());
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (active) {
			if (renderer.material.color.a < 1) {
				Color c = renderer.material.color;
				c.a += .01f;
				if (c.a > 1)
					c.a = 1;

				renderer.material.color = c;
			}
		} else {
			if (renderer.material.color.a > .2) {
				Color c = renderer.material.color;
				c.a -= .01f;
				if (c.a < .2f)
					c.a = .2f;

				renderer.material.color = c;
			}
		}
	}
}
